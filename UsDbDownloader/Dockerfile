FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["UsDbDownloader/UsDbDownloader.csproj", "UsDbDownloader/"]
COPY . .
WORKDIR "/src/UsDbDownloader"
RUN dotnet restore "UsDbDownloader.csproj"
RUN dotnet build "UsDbDownloader.csproj" -c Release -o /app/build
RUN dotnet publish "UsDbDownloader.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:7.0

RUN echo "**** update package list ****" && \
  apt update && \
  echo "**** install ffmpeg ****" && \
  apt install -y ffmpeg && \
  echo "**** cleanup ****" && \
  apt-get autoclean && \
  rm -rf \
    /var/lib/apt/lists/* \
    /var/tmp/* \
    /tmp/*

WORKDIR /app
EXPOSE 8080
VOLUME /data
ENV ASPNETCORE_URLS http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT	Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UsDbDownloader.dll"]
