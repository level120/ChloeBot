#!/bin/bash

# Pulling files
cd /GameBot/Core
git fetch && git pull origin master

# Build project for docnet core
dotnet clean
rm -rf /GameBot/Core/Core/bin
dotnet build -c Release -r ubuntu.18.04-x64

# Change permission to runable
cd /GameBot/Core/Core/bin/Release/netcoreapp2.1/ubuntu.18.04-x64
chmod 755 ChloeBot

# Run discord bot
./ChloeBot
