FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UsDbDownloader/UsDbDownloader.csproj", "UsDbDownloader/"]
COPY . .
WORKDIR "/src/UsDbDownloader"
RUN dotnet restore "UsDbDownloader.csproj"
RUN dotnet build "UsDbDownloader.csproj" -c Release -o /app/build
RUN dotnet publish "UsDbDownloader.csproj" -c Release -o /app/publish

FROM ubuntu:jammy AS ytdlp-downloader
RUN apt update && apt install -y wget
RUN wget -O /yt-dlp https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp
RUN chmod +x /yt-dlp

FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN echo "**** update package list ****" && \
  apt update && \
  echo "**** install ffmpeg ****" && \
  apt install -y ffmpeg python3 && \
  echo "**** cleanup ****" && \
  apt-get autoclean && \
  rm -rf \
    /var/lib/apt/lists/* \
    /var/tmp/* \
    /tmp/* && \
  echo "**** Prepare data directory ****" && \
  install -m 777 -d /data

WORKDIR /app
EXPOSE 8080
VOLUME /data
ENV ASPNETCORE_URLS http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT	Production
ENV XDG_CACHE_HOME /data

COPY "UsDbDownloader/entrypoint.sh" "./"
COPY --from=build /app/publish .
COPY --from=ytdlp-downloader /yt-dlp /usr/bin/

ENTRYPOINT ["/app/entrypoint.sh"]
CMD ["dotnet", "UsDbDownloader.dll"]
