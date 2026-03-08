using Godot;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// AutoLoad Singleton for sending/receiving OSC messages to/from an Arduino over UDP.
/// Keeps ports open continuously to avoid port conflicts and dropped messages.
/// </summary>
public partial class ArduinoUDP : Node
{
    // ── Configure these ──────────────────────────────────────
    private const string ARDUINO_IP   = "172.20.10.3";
    private const int    ARDUINO_PORT = 7001;
    private const int    LISTEN_PORT  = 7002;
    private const string OSC_ADDRESS  = "/text";        // OSC address pattern
    // ─────────────────────────────────────────────────────────
    public int tm_uses = 0;
    public int nos = 0;
    public bool justUsed = false;
    public static ArduinoUDP Instance { get; private set; }

    // Event fired whenever the Arduino sends a boolean via UDP
    public event Action<bool> OnBooleanReceived;

    private UdpClient _receiveClient;
    private UdpClient _sendClient;
    private TaskCompletionSource<bool> _pendingReceive;

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }
        Instance = this;

        // Initialize persistent sender
        _sendClient = new UdpClient();

        // Initialize persistent listener
        try
        {
            _receiveClient = new UdpClient(LISTEN_PORT);
            GD.Print($"[ArduinoUDP] Listening for Arduino messages on port {LISTEN_PORT}...");
            
            // Start the background listener loop
            _ = ListenForArduinoLoop();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[ArduinoUDP] Failed to open listen port {LISTEN_PORT}: {ex.Message}");
        }
    }

    public override void _ExitTree()
    {
        _receiveClient?.Close();
        _sendClient?.Close();
    }

    // ── Public API ───────────────────────────────────────────

    /// <summary>
    /// Sends an OSC string and blocks until the Arduino responds with a boolean.
    /// Uses the persistent background listener to handle the response.
    /// Waits indefinitely until a response is received.
    /// </summary>
    public async Task<bool> SendAndReceiveAsync(string text)
    {
        // Cancel any old pending receive
        _pendingReceive?.TrySetCanceled();
        _pendingReceive = new TaskCompletionSource<bool>();

        // Send the string
        SendStringAsync(text);

        // Wait indefinitely for the background listener to resolve _pendingReceive
        return await _pendingReceive.Task;
    }

    /// <summary>
    /// Sends a valid OSC message containing a single string argument.
    /// Fire and forget.
    /// </summary>
    public void SendStringAsync(string text)
    {
        byte[] packet = BuildOscMessage(OSC_ADDRESS, text);
        try
        {
            _sendClient.SendAsync(packet, packet.Length, ARDUINO_IP, ARDUINO_PORT);
            GD.Print($"[ArduinoUDP] Sent OSC {OSC_ADDRESS} \"{text}\" ({packet.Length} bytes)");
        }
        catch (Exception ex)
        {
             GD.PrintErr($"[ArduinoUDP] Send error: {ex.Message}");
        }
    }

    // ── Background Listener Loop ─────────────────────────────

    private async Task ListenForArduinoLoop()
    {
        while (IsInsideTree()) // Loop while the game is running
        {
            try
            {
                var result = await _receiveClient.ReceiveAsync();
                byte[] data = result.Buffer;

                if (data.Length > 0)
                {
                    bool value = ParseOscBoolean(data);
                    GD.Print($"[ArduinoUDP] Received from Arduino: {value}");
                    
                    // Fire the event for anyone listening generally
                    OnBooleanReceived?.Invoke(value);

                    // If a specific SendAndReceiveAsync call is waiting, hand it the value
                    _pendingReceive?.TrySetResult(value);
                }
            }
            catch (ObjectDisposedException) { /* Ignored, happens on exit */ }
            catch (Exception ex)
            {
                GD.PrintErr($"[ArduinoUDP] Receive error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Basic OSC parser that looks for a boolean or integer argument.
    /// Example packet: "/booleans\0\0\0,i\0\0\0\0\0\1"
    /// </summary>
    private bool ParseOscBoolean(byte[] data)
    {
        // 1. Find the type tag string separator ','
        int commaIndex = -1;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == (byte)',')
            {
                commaIndex = i;
                break;
            }
        }

        if (commaIndex == -1 || commaIndex + 1 >= data.Length)
            return false; // No type tags found

        char typeTag = (char)data[commaIndex + 1];

        // OSC True / False types (no argument bytes following)
        if (typeTag == 'T') return true;
        if (typeTag == 'F') return false;

        GD.Print("PAPPLE");
        return false;
    }

    // ── OSC message builder ──────────────────────────────────

    public static byte[] BuildOscMessage(string address, string argument)
    {
        var buf = new List<byte>();
    
        // 1. Address pattern – must start with '/'
        WriteOscString(buf, address);

        // 2. Type tag string – ",s" means one string argument
        WriteOscString(buf, ",s");

        // 3. The string argument itself
        WriteOscString(buf, argument);

        return buf.ToArray();
    }

    private static void WriteOscString(List<byte> buf, string s)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        buf.AddRange(bytes);

        // Add 1–4 null bytes to reach the next 4-byte boundary
        int nullCount = 4 - (bytes.Length % 4);
        for (int i = 0; i < nullCount; i++)
            buf.Add(0x00);
    }
}
