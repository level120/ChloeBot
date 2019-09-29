FROM mcr.microsoft.com/dotnet/core/sdk:3.0

WORKDIR /

RUN apt-get update && \
	apt-get install -y git &&\
	git clone https://github.com/level120/GameBot.git &&\
	apt-get remove --purge -yq git

WORKDIR /GameBot/Core

RUN dotnet build -c Release -r ubuntu.18.04-x64

WORKDIR /GameBot
RUN chmod 755 /GameBot/start.sh

#ENTRYPOINT /GameBot/start.sh
