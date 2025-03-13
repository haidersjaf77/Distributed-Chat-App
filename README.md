# Distributed-Chat-App

## Overview  
This project implements a **real-time chat application** using **C# and the .NET framework**, enabling multiple clients to communicate via a centralized server. The server handles concurrent client connections using **TCP sockets** and **asynchronous programming**, ensuring scalability and responsiveness.  

## Features  
- **Multi-Client Support**: Allows multiple users to connect and exchange messages in real-time.  
- **Asynchronous Communication**: Ensures smooth messaging without blocking operations.  
- **TCP Socket-Based Networking**: Reliable client-server communication using `TcpListener` and `TcpClient`.  
- **User Join/Leave Notifications**: Notifies all users when a new client connects or disconnects.  

## Technologies Used  
- **C#** (.NET Framework)  
- **TCP Sockets** (`TcpListener`, `TcpClient`)  
- **Asynchronous Programming** (`async/await`)  

## How to Run  
### **Server**  
1. Open the project in **Visual Studio** or any C# IDE.  
2. Run the **ChatServer** program:  
   ```sh
   dotnet run --project ChatServer
