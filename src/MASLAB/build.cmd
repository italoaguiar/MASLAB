@echo off

dotnet tool install --global dotnet-zip
dotnet tool install --global dotnet-tarball
dotnet tool install --global dotnet-rpm
dotnet tool install --global dotnet-deb

echo Compilando versao do Windows
dotnet publish -c Release -f netcoreapp3.1 -r win-x64 --self-contained true
dotnet zip -c Release -f netcoreapp3.1 -r win-x64

echo Compilando versao do Linux
dotnet publish -c Release -f netcoreapp3.1 -r linux-x64 --self-contained true
dotnet deb -c Release -f netcoreapp3.1 -r linux-x64
dotnet rpm -c Release -f netcoreapp3.1 -r linux-x64
dotnet zip -c Release -f netcoreapp3.1 -r linux-x64
dotnet tarball -c Release -f netcoreapp3.1 -r linux-x64

echo Compilando versao do macOS
dotnet publish -c Release -f netcoreapp3.1 -r osx-x64 --self-contained true
dotnet zip -c Release -f netcoreapp3.1 -r osx-x64


pause

