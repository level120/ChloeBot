#!/bin/sh

cd /GameBot/Core
git fetch && git pull origin master

chmod 755 /GameBot/Core/Core/bin/Release/netcoreapp3.0/ubuntu.18.04-x64/ChloeBot
/GameBot/Core/Core/bin/Release/netcoreapp3.0/ubuntu.18.04-x64/ChloeBot
