using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Static helper for UDP communication with the Arduino Uno R4 WiFi.
///
/// Protocol:
///   SEND    → Arduino port 8888: exactly 32 bytes (null-padded UTF-8 string)
///   RECEIVE ← Arduino port 8889: exactly 1 byte  (0x01 = true, 0x00 = false)
/// </summary>
public static class ArduinoUDP
{
    // ── Configure these ──────────────────────────────────────
    private const string ARDUINO_IP   = "192.168.X.X"; // ← Replace with Arduino's IP from Serial Monitor
    private const int    ARDUINO_PORT = 8888;           // Port Arduino listens on
    private const int    LISTEN_PORT  = 8889;           // Port we listen on for responses
    private const int    STRING_LENGTH = 32;            // Exact byte length enforced by protocol
    private const int    TIMEOUT_MS   = 3000;           // How long to wait for a response
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Sends a 32-byte null-padded string to the Arduino via UDP.
    /// </summary>
    public static async Task SendStringAsync(string text)
    {
        byte[] packet = BuildPacket(text);

        using var client = new UdpClient();
        await client.SendAsync(packet, packet.Length, ARDUINO_IP, ARDUINO_PORT);
        GD.Print($"[ArduinoUDP] Sent: \"{text}\" ({text.Length} chars, padded to {STRING_LENGTH} bytes)");
    }

    /// <summary>
    /// Listens on LISTEN_PORT for a 1-byte boolean response from the Arduino.
    /// Returns true (0x01) or false (0x00). Times out after TIMEOUT_MS.
    /// </summary>
    public static async Task<bool> ReceiveBooleanAsync()
    {
        using var listener = new UdpClient(LISTEN_PORT);

        var receiveTask = listener.ReceiveAsync();
        var completed = await Task.WhenAny(receiveTask, Task.Delay(TIMEOUT_MS));

        if (completed == receiveTask)
        {
            var result = receiveTask.Result;
            bool value = result.Buffer.Length > 0 && result.Buffer[0] == 0x01;
            GD.Print($"[ArduinoUDP] Received response: {value}");
            return value;
        }

        GD.PrintErr("[ArduinoUDP] Timed out waiting for Arduino response.");
        return false;
    }

    /// <summary>
    /// Convenience: sends the string and waits for the boolean response.
    /// </summary>
    public static async Task<bool> SendAndReceiveAsync(string text)
    {
        // Open listener BEFORE sending so we don't miss the reply
        using var listener = new UdpClient(LISTEN_PORT);

        // Send the padded string
        byte[] packet = BuildPacket(text);
        using (var sender = new UdpClient())
        {
            await sender.SendAsync(packet, packet.Length, ARDUINO_IP, ARDUINO_PORT);
            GD.Print($"[ArduinoUDP] Sent: \"{text}\" ({text.Length} chars, padded to {STRING_LENGTH} bytes)");
        }

        // Wait for response with timeout
        var receiveTask = listener.ReceiveAsync();
        var completed = await Task.WhenAny(receiveTask, Task.Delay(TIMEOUT_MS));

        if (completed == receiveTask)
        {
            var result = receiveTask.Result;
            bool value = result.Buffer.Length > 0 && result.Buffer[0] == 0x01;
            GD.Print($"[ArduinoUDP] Received response: {value}");
            return value;
        }

        GD.PrintErr("[ArduinoUDP] Timed out waiting for Arduino response.");
        return false;
    }

    /// <summary>
    /// Builds a 32-byte null-padded packet from the input string.
    /// </summary>
    private static byte[] BuildPacket(string text)
    {
        byte[] packet = new byte[STRING_LENGTH];
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        int copyLen = Math.Min(textBytes.Length, STRING_LENGTH);
        Array.Copy(textBytes, packet, copyLen);
        return packet;
    }
}
