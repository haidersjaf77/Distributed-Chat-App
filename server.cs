using System; 
using System.Collections.Concurrent; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading.Tasks; 
 
class ChatServer 
{ 
    private static ConcurrentBag<TcpClient> clients = new ConcurrentBag<TcpClient>(); 
 
    static async Task Main(string[] args) 
    { 
        TcpListener listener = new TcpListener(IPAddress.Any, 8888); 
        listener.Start(); 
 
        Console.ForegroundColor = ConsoleColor.Green; 
        Console.WriteLine("====================================="); 
        Console.WriteLine("       Chat Server Started           "); 
        Console.WriteLine("====================================="); 
        Console.ResetColor(); 
        Console.WriteLine("Waiting for clients..."); 
 
        while (true) 
        { 
            TcpClient client = await listener.AcceptTcpClientAsync(); 
            clients.Add(client); 
            _ = HandleClientAsync(client); 
        } 
    } 
 
    static async Task HandleClientAsync(TcpClient client) 
    { 
        Console.ForegroundColor = ConsoleColor.Yellow; 
        Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected."); 
        Console.ResetColor(); 
 
        try 
        { 
            using (NetworkStream stream = client.GetStream()) 
            { 
                string username = await ReceiveMessageAsync(stream); 
                if (username != null) 
                { 
                    BroadcastMessage($"{username} has joined the chat."); 
 
                    while (true) 
                    { 
                        string message = await ReceiveMessageAsync(stream); 
                        if (message == null || message.Equals("/exit", StringComparison.OrdinalIgnoreCase)) 
                        { 
                            clients.TryTake(out _); 
                            BroadcastMessage($"{username} has left the chat."); 
                            break; 
 
                        } 
                        BroadcastMessage($"{username}: {message}"); 
                    } 
                } 
            } 
        } 
        catch (Exception ex) 
        { 
            Console.ForegroundColor = ConsoleColor.Red; 
            Console.WriteLine($"Error with client {client.Client.RemoteEndPoint}: {ex.Message}"); 
            Console.ResetColor(); 
        } 
        finally 
        { 
            client.Close(); 
            clients.TryTake(out _); 
            Console.ForegroundColor = ConsoleColor.Yellow; 
            Console.WriteLine($"Client {client.Client.RemoteEndPoint} disconnected."); 
            Console.ResetColor(); 
        } 
    } 
 
    static async Task<string> ReceiveMessageAsync(NetworkStream stream) 
    { 
        try 
        { 
            byte[] buffer = new byte[1024]; 
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); 
            if (bytesRead == 0) 
                return null; 
            return Encoding.UTF8.GetString(buffer, 0, bytesRead); 
        } 
        catch (Exception ex) 
        { 
            Console.ForegroundColor = ConsoleColor.Red; 
            Console.WriteLine($"ReceiveMessageAsync Error: {ex.Message}"); 
            Console.ResetColor(); 
            return null; 
        } 
    } 
 
    static async void BroadcastMessage(string message) 
    { 
        string timestamp = DateTime.Now.ToString("HH:mm:ss"); 
        string formattedMessage = $"[{timestamp}] {message}"; 
 
        Console.ForegroundColor = ConsoleColor.Cyan; 
        Console.WriteLine($"Broadcasting: {formattedMessage}"); 
        Console.ResetColor(); 
 
        byte[] buffer = Encoding.UTF8.GetBytes(formattedMessage); 
 
        foreach (TcpClient client in clients) 
        { 
            if (client.Connected) 
            { 
                NetworkStream stream = client.GetStream(); 
                try 
9 
 
                { 
                    await stream.WriteAsync(buffer, 0, buffer.Length); 
                } 
                catch (Exception ex) 
                { 
                    Console.ForegroundColor = ConsoleColor.Red; 
                    Console.WriteLine($"Error broadcasting to {client.Client.RemoteEndPoint}: {ex.Message}"); 
                    Console.ResetColor(); 
                } 
            } 
        } 
 
        Console.ForegroundColor = ConsoleColor.Magenta; 
        Console.WriteLine($"Currently connected clients: {clients.Count}"); 
        Console.ResetColor(); 
    } 
} 