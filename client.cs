using System; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading.Tasks; 
 
class ChatClient 
{ 
    static async Task Main(string[] args) 
    { 
        Console.ForegroundColor = ConsoleColor.Green; 
        Console.WriteLine("====================================="); 
        Console.WriteLine("         Welcome to ChatClient       "); 
        Console.WriteLine("====================================="); 
        Console.ResetColor(); 
        Console.WriteLine("Please enter your username to join the chat."); 
 
        Console.Write("Enter your username: "); 
        string username = Console.ReadLine(); 
 
        Console.WriteLine("Connecting to the server..."); 
        try 
        { 
            using (TcpClient client = new TcpClient("127.0.0.1", 8888)) 
            { 
                Console.ForegroundColor = ConsoleColor.Green; 
                Console.WriteLine("Connected to the server. You can start chatting now."); 
                Console.ResetColor(); 
                NetworkStream stream = client.GetStream(); 
 
                 
                await SendMessageAsync(stream, username); 
 
                 
                _ = ReceiveMessagesAsync(stream); 
 
                 
                while (true) 
                { 
                    string message = Console.ReadLine(); 
                    if (string.IsNullOrWhiteSpace(message)) continue; 
                    if (message.Equals("/exit", StringComparison.OrdinalIgnoreCase)) 
                    { 
                        await SendMessageAsync(stream, $"{username} has left the chat."); 
                        break; 
                    } 
 
                    await SendMessageAsync(stream, $"{username}: {message}"); 
                } 
            } 
        } 
        catch (Exception ex) 
        { 
            Console.ForegroundColor = ConsoleColor.Red; 
            Console.WriteLine($"Error: {ex.Message}"); 
            Console.ResetColor(); 
        } 
        finally 
        { 
            Console.ForegroundColor = ConsoleColor.Yellow; 
            Console.WriteLine("Disconnected from the server. Press any key to exit."); 
            Console.ResetColor(); 
            Console.ReadKey(); 
        } 
    } 
 
    static async Task SendMessageAsync(NetworkStream stream, string message) 
    { 
        byte[] buffer = Encoding.UTF8.GetBytes(message); 
        await stream.WriteAsync(buffer, 0, buffer.Length); 
    } 
 
    static async Task ReceiveMessagesAsync(NetworkStream stream) 
    { 
        while (true) 
        { 
            byte[] buffer = new byte[1024]; 
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); 
            if (bytesRead == 0) 
            { 
                Console.ForegroundColor = ConsoleColor.Red; 
                Console.WriteLine("Disconnected from server."); 
                Console.ResetColor(); 
                break; 
            } 
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead); 
            PrintMessage(receivedMessage); 
        } 
    } 
 
    static void PrintMessage(string message) 
    { 
        string timestamp = DateTime.Now.ToString("HH:mm:ss"); 
        Console.ForegroundColor = ConsoleColor.Cyan; 
        Console.WriteLine($"[{timestamp}] {message}"); 
        Console.ResetColor(); 
    } 
}