@echo off
start "ChatServer" dotnet ./ChatServer/bin/Debug/net6.0/ChatServer.dll
start "SceneServer" dotnet ./SceneServer/bin/Debug/net6.0/SceneServer.dll 8891
start "SceneServer" dotnet ./SceneServer/bin/Debug/net6.0/SceneServer.dll 8892
start "Client" dotnet ./ShawnClient/bin/Debug/net6.0/ShawnClient.dll
rem pause