FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PlaylistCreator/PlaylistCreator.csproj", "PlaylistCreator/"]
RUN dotnet restore "PlaylistCreator/PlaylistCreator.csproj"
COPY . .
WORKDIR "/src/PlaylistCreator"
RUN dotnet build "PlaylistCreator.csproj" -c Release -o /app/build
RUN dotnet publish "PlaylistCreator.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 8080
VOLUME /data
ENV ULTRASTAR_DB /data/Ultrastar.db
ENV ASPNETCORE_URLS http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT	Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PlaylistCreator.dll"]
