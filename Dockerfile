FROM mcr.microsoft.com/dotnet/core/sdk:3.0

WORKDIR /

RUN apt-get update && \
	apt-get install -y git &&\
	git clone https://github.com/level120/GameBot.git &&\
	apt-get remove --purge -yq git

WORKDIR /GameBot/Core

#RUN dotnet build -c Release -r ubuntu.18.04-arm
RUN dotnet build -c Release -r ubuntu.18.04-x64

WORKDIR /GameBot

ENTRYPOINT ["./start.sh"]
