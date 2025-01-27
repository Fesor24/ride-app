# Chat System

## Add Chat to DB
dbcontext.cs
```c#
public DbSet<ChatSession> ChatSession { get; set; }
public DbSet<MessagesSession> ChatMessages { get; set; }
```
## Add WebSocket Support
terminal or nuget
```c#
dotnet add package Microsoft.AspNetCore.WebSockets
```
In your Startup.cs file, configure WebSocket middleware in the Configure method. 
```c#
app.UseWebSockets();
app.MapWebSocketManager("/chat", app.ApplicationServices.GetService<ChatWebSocketHandler>())
```


