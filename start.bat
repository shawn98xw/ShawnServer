@echo off
start "ChatServer" dotnet ./ChatServer/bin/Debug/net6.0/ChatServer.dll
start "SceneServer" dotnet ./SceneServer/bin/Debug/net6.0/SceneServer.dll
pause