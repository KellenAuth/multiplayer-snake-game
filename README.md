# Multiplayer Snake Game with Stats Dashboard

A real-time multiplayer Snake game built with C# featuring TCP networking, persistent player statistics, and a web-based stats dashboard. Players compete in a shared arena, collecting powerups while avoiding walls and other snakes, with all game sessions and player performance tracked in a MySQL database.

**Team Project:** Built collaboratively with Ben Homer for CS3500 - Software Practice at the University of Utah (Fall 2024)

## 🎮 Project Overview

This project consists of three interconnected components:

1. **Game Client** - Blazor WebAssembly application with real-time rendering
2. **TCP Game Server** - Handles multiplayer connections and game state synchronization
3. **HTTP Stats Server** - Web dashboard displaying game history and player statistics

The system supports multiple simultaneous players, tracks detailed statistics (scores, session times, max scores), and provides a web interface for viewing historical game data.

## ✨ Key Features

### Multiplayer Gameplay
- Real-time networked multiplayer using TCP sockets
- Support for multiple concurrent players in the same game world
- Dynamic player join/leave handling with session tracking
- Collision detection for snakes, walls, and powerups
- Death and respawn system with visual feedback

### Game Mechanics
- Classic Snake gameplay with growing body segments
- Powerup collection system that increases score and snake length
- Dynamic wall placement creating maze-like arenas
- Score tracking with persistent high score records
- Player disconnection handling with graceful cleanup

### Statistics & Persistence
- MySQL database integration for persistent game data
- Tracks all game sessions with start/end timestamps
- Per-player statistics: scores, max scores, session duration
- Web-based dashboard for viewing historical data
- RESTful-style HTTP routes for game and player queries

### Technical Architecture
- **Client-Server Model:** Clean separation between game client and server logic
- **Model-View Pattern:** Game state (World, Snake, Powerups, Walls) separate from rendering
- **Multi-threaded Server:** Each client connection handled in separate thread
- **JSON Communication:** Game state serialized and transmitted as JSON objects
- **Asynchronous UI:** JavaScript interop for keyboard input handling

## 🛠️ Technologies Used

### Backend
- **Language:** C# (.NET 7/8)
- **Networking:** TCP sockets with custom protocol
- **Database:** MySQL with MySql.Data client
- **Threading:** Multi-threaded server architecture
- **Serialization:** System.Text.Json for game state

### Frontend
- **Framework:** Blazor WebAssembly
- **Rendering:** HTML5 Canvas via JavaScript Interop
- **Styling:** Custom CSS
- **Input Handling:** JavaScript event listeners with C# callbacks

### Game Server
- **HTTP Server:** Custom HTTP/1.1 server implementation
- **Routing:** Regex-based URL pattern matching
- **Database Queries:** Dynamic SQL with MySqlCommand
- **HTML Generation:** StringBuilder for dynamic table generation

## 📦 Project Structure

```
Snake/
├── GUI.Client/                    # Blazor WebAssembly game client
│   ├── Models/
│   │   ├── World.cs              # Game world state manager
│   │   ├── Snake.cs              # Snake entity with body segments
│   │   ├── Powerups.cs           # Collectible powerup objects
│   │   ├── Walls.cs              # Wall obstacles
│   │   └── Point2D.cs            # 2D coordinate representation
│   ├── Controllers/
│   │   └── [Network controllers] # TCP client connection handling
│   ├── Pages/
│   │   └── SnakeGUI.razor        # Main game UI component
│   └── wwwroot/
│       └── SnakeGUI.razor.js     # JavaScript for input/rendering
├── Server/
│   ├── Server.cs                 # TCP server for game connections
│   └── [Game logic]              # Server-side game state management
└── WebServer/
    ├── WebServer.cs              # HTTP server for stats dashboard
    └── Program.cs                # Web server entry point
```

## 🎯 Technical Highlights

### Real-Time Networking
```csharp
// Multi-threaded TCP server handling concurrent connections
public static void StartServer(Action<NetworkConnection> handleConnect, int port)
{
    TcpListener listener = new(IPAddress.Any, port);
    listener.Start();
    while (true)
    {
        TcpClient client = listener.AcceptTcpClient();
        NetworkConnection connection = new NetworkConnection(client);
        new Thread(() => handleConnect(connection)).Start();
    }
}
```

### Database Integration
- CRUD operations for game sessions and player records
- Real-time updates during gameplay (score tracking)
- Timestamp recording for session analytics
- Transaction handling for data consistency

### Game State Management
```csharp
// Thread-safe game world state updates
public void UpdateSnake(Snake snake)
{
    lock (snakes)
    {
        // Handle new player joins
        if (!snakes.ContainsKey(snake.snake) && !deadSnakes.ContainsKey(snake.snake))
            SQLAddSnake(snake);
        
        // Track disconnections and update database
        if (snake.dc)
            DisconnectedSnakeUpdate(snake, DateTime.Now);
        
        // Manage alive/dead state transitions
        // Update max scores in real-time
    }
}
```

### Custom HTTP Server
- Built from scratch without ASP.NET framework
- Implements HTTP/1.1 protocol basics
- Dynamic HTML generation from database queries
- Route handling with regex pattern matching

## 🚀 How to Run

### Prerequisites
- .NET 7.0 or 8.0 SDK
- MySQL Server (or access to MySQL instance)
- Visual Studio 2022 or VS Code with C# extension

### Database Setup
1. Create MySQL database and tables:
```sql
CREATE DATABASE Sanke-Game;

CREATE TABLE Games (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    StartTime DATETIME,
    EndTime DATETIME
);

CREATE TABLE Players (
    PlayerID INT,
    GameID INT,
    Name VARCHAR(255),
    MaxScore INT,
    EnterTime DATETIME,
    LeaveTime DATETIME,
    PRIMARY KEY (PlayerID, GameID),
    FOREIGN KEY (GameID) REFERENCES Games(ID)
);
```

2. Configure connection string in `World.cs` and `WebServer.cs` 

### Running the Game

**Option 1: Visual Studio**
1. Open `Snake.sln`
2. Set startup projects:
   - Right-click solution → Properties → Multiple startup projects
   - Set `Server` and `GUI.Client` to Start
3. Press F5 to run

**Option 2: Command Line**
```bash
# Terminal 1: Start game server
cd Server
dotnet run

# Terminal 2: Start web stats server
cd WebServer
dotnet run

# Terminal 3: Start game client
cd GUI.Client
dotnet run
```

### Connecting to the Game
1. Game client will prompt for server address (e.g., `localhost`)
2. Enter your player name
3. Use arrow keys or WASD to control your snake
4. Visit `http://localhost:80` for stats dashboard

## 🎓 What I Learned

### Network Programming
- TCP socket programming with asynchronous connections
- Protocol design for real-time game state synchronization
- Multi-threaded server architecture and thread safety
- Connection management (handling joins, disconnects, timeouts)

### Database Systems
- SQL database design with relational tables
- CRUD operations with MySqlCommand
- Real-time data updates during active gameplay
- Query optimization for retrieving game statistics

### Full-Stack Development
- Client-server architecture with clear separation of concerns
- JSON serialization for network communication
- Building HTTP servers from scratch
- Dynamic web content generation

### Software Engineering
- Model-View separation for maintainable code
- Thread-safe collection management with locks
- Event-driven programming with Blazor
- JavaScript interop for browser capabilities

### Game Development
- Real-time game loop and state management
- Collision detection algorithms
- Entity management (players, powerups, obstacles)
- Multiplayer synchronization challenges

## 🎮 How to Play

1. **Join Game:** Enter server address and your player name
2. **Movement:** Use arrow keys or WASD to control your snake
3. **Collect Powerups:** Eat the colored powerups to grow and increase score
4. **Avoid Obstacles:** Don't hit walls or other snakes (including yourself!)
5. **Respawn:** After death, you'll respawn automatically
6. **View Stats:** Visit the web dashboard to see game history and leaderboards

## 👥 Development

**Developers:** Kellen Auth ([@KellenAuth](https://github.com/KellenAuth)) & Other Student  
**Course:** CS3500 - Software Practice, University of Utah  
**Semester:** Fall 2024  

## 📄 License

This project was created for educational purposes as part of CS3500 at the University of Utah.

## 🙏 Acknowledgments

- University of Utah CS3500 teaching staff for project guidance
- Course assignment specifications and starter code
- Other Student for collaborative development and problem-solving

---

**A comprehensive multiplayer game demonstrating network programming, database integration, and full-stack development skills.**
