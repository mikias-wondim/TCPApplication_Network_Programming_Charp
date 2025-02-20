# **TCP Client-Server Communication Application**

This project demonstrates a simple **TCP Client-Server Communication** application using C#. It leverages **Serilog** for logging and implements best practices for building a TCP-based communication system with a background service that handles continuous operations for the client.

## **Project Structure**

### **1. Solution Structure**
```
TCP-Communication/
│
├── TCP.Server/
│   ├── Services/
│   │   └── ITcpServer.cs
│   │   └── TcpServer.cs
│   ├── Commands/
│   │   └── CommandProcessor.cs
│   │   └── TempratureCommands.cs
│   │   └── StatusCommand.cs
│   └── Program.cs
│   └── appsettings.json
│
├── TCP.Client/
│   ├── Services/
│   │   └── ITcpClientService.cs
│   │   └── TcpClientService.cs
│   ├── ClientWorker.cs
│   ├── Program.cs
│   └── appsettings.json

```

### **2. Main Components**
- **TCP.Server**
    - **TcpServer.cs**: This file contains the logic for starting and stopping the TCP server. It accepts client connections, reads incoming requests, processes commands, and sends responses.
    - **Program.cs**: Initializes the server and logging configuration.
  
- **TCP.Client**
    - **TcpClientService.cs**: This service is responsible for connecting to the TCP server and sending commands.
    - **TcpClientWorker.cs**: A background service that runs indefinitely and sends commands to the server periodically.
    - **Program.cs**: Initializes the TCP client service, starts the background worker, and sets up logging.
  
- **Common**
    - **appsettings.json**: Stores configuration settings like server IP, port, and logging paths (both for client and server).

---

## **Configuration Files**

### **appsettings.json (Common for both client and server)**

```json
{
  "Logging": {
    "FilePath": "logs/log.txt"
  },
  "Server": {
    "Port": 5000
  },
  "Client": {
    "ServerIp": "127.0.0.1",
    "ServerPort": 5000
  }
}
```

- **Logging**: Specifies the file path where logs will be saved.
- **Server**: Configures the server's listening port.
- **Client**: Specifies the IP address and port of the server to connect to.

---

## **Approaches & Design**

### **1. TCP Server**
The TCP server is designed to:
- Listen on a specified port.
- Accept client connections using `TcpListener`.
- Process requests in an asynchronous loop.
- Log all connections and requests, including the client's IP address.
- Gracefully shut down when requested.

#### **Key Features**
- The server listens indefinitely for client connections.
- Each incoming client is processed asynchronously in the `HandleClientAsync` method.
- The IP address of each client is logged when they connect and disconnect.

### **2. TCP Client**
The TCP client is designed to:
- Connect to a specified server.
- Periodically send predefined commands to the server (e.g., `"GET_TEMP"` and `"GET_STATUS"`).
- Log responses received from the server.

#### **Key Features**
- The client is implemented as a background worker service (`TcpClientWorker`), which keeps running until stopped.
- Commands are sent every 5 seconds.
- The connection to the server is automatically established when needed and closed after each command.

### **3. Logging with Serilog**
- **Serilog** is configured to log messages to both the console and a log file (as specified in `appsettings.json`).
- Logs include connection and disconnection details, received requests, and responses.

### **4. Use of `BackgroundService` for the Client Worker**
- The client worker runs in the background and sends commands periodically until the application is stopped.
- The worker handles shutdown gracefully and logs all events (such as error handling or when the service is shut down).

---

## **Running the Application**

### **1. Start the Server**
To start the server:
1. Navigate to the `TCP.Server` project.
2. Run the application:

   ```bash
   dotnet run
   ```

   This will start the server and begin listening on the port specified in the configuration (default is `5000`).

### **2. Start the Client**
To start the client:
1. Navigate to the `TCP.Client` project.
2. Run the application:

   ```bash
   dotnet run
   ```

   The client will automatically connect to the server, send predefined commands (`"GET_TEMP"` and `"GET_STATUS"`), and log the server's responses.

---

## **Expected Output**

### **1. Server Logs**
The server will log client connections, received commands, and responses:
```
[INFO] Client connected from 192.168.1.10:53245
[INFO] Received from 192.168.1.10:53245: GET_TEMP
[INFO] Processing command GET_TEMP
[INFO] Sent response: Temperature = 25.3°C
[INFO] Client 192.168.1.10:53245 disconnected.
```

### **2. Client Logs**
The client will log its connection attempts, the commands it sends, and the responses it receives:
```
[INFO] Connected to 127.0.0.1:5000
[INFO] Sending command: GET_TEMP
[INFO] Server Response: Temperature = 25.3°C
[INFO] Waiting for the next cycle...
[INFO] Sending command: GET_STATUS
[INFO] Server Response: Status = Active
```

---

## **Conclusion**
This project demonstrates how to implement a TCP client-server communication system using C#. It emphasizes:
- **Asynchronous communication** for scalable handling of multiple clients.
- **Logging** using Serilog to track all interactions with clients and servers.
- **Background services** for long-running tasks like continuously sending commands from the client.
