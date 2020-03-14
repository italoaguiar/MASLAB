@echo off
echo Compilando versao do Windows
dotnet publish -c Release -f netcoreapp3.1 -r win-x64 --self-contained true

echo Compilando versao do Linux
dotnet publish -c Release -f netcoreapp3.1 -r linux-x64 --self-contained true

echo Compilando versao do macOS
dotnet publish -c Release -f netcoreapp3.1 -r osx-x64 --self-contained true


pause

