# UltraStar Tools

This Repository contains mainly two tools:

- ScanSongLibraryToJson: Indexes local songs to be used by UsDbDownloader
- UsDbDownloader: 
  - Shows indexed songs
  - Downloading of songs (mp3 + mp4 + txt) using <https://usdb.animux.de>
  - Creation and managment of playlists for UltraStar Deluxe

## ScanSongLibraryToJson

### Build

Requirements: .net 7 sdk

```
dotnet build -c release ScanSongLibraryJson/ScanSongLibraryJson.csproj
```

### Usage

Requirements: .net 7 runtime

```
ScanSongLibraryJson <path to song folder> <path to ultrastar db> > songs.json
```

## UsDbDownloader

### Build

Requirements: docker

```
docker compose build
```

### Run (local built)

- Enter user and password in `docker-compose.yml`
- Create data folder (or bind another folder in the container)
- Put song index in data folder as `songs.json` (or adapt path to it accordingly in the `docker-compose.yml`)
- Optional: adapt db path (used to store playlists) and downlaod destination location


```
docker compose up
```

### Run (built from github container registry)

#### `docker-compose.yml`

```yaml
services:
  singstar:
    image: ghcr.io/pagdot/ultrastartools:main
    container_name: ultrastartools
    restart: unless-stopped
    volumes:
      - ./data:/data
    environment:
      - LOGIN__USER=AzureDiamond # usdb.animux.de username
      - LOGIN__PASSWORD=hunter2 # usdb.animux.de password  (http://bash.org/?244321)
      - SETTINGS__DESTINATION=/data/songs/
      - SONGLISTFILE=/data/songs.json
      - DBPATH=/data/UltraTools.db
```

### Usage

#### Songs page

This page lists all indexed songs and allows filtering them and adding them to playlists

#### Requests page

- This page lists all songs indexed from <https://usdb.animux.de>
- More information of a song (required for download) can be queried by clicking on the "refresh" button on the right of a song entry. This unlocks if the query succeeds following buttons/icons:
  - YouTube icon: Open the referenced YouTube video by clicking on the icon
  - green download icon: Download the song (txt + mp3 + mp4) on the server. The song may need further adjustments
  - yellow, crossed out download icon: no YouTube video found -> Download not possible
  - light blue, crossed out download icon: song was already downloaded -> Download neither required nor possible
  - light blue check icon: song is already in indexed song list -> Download neither required nor possible

#### Playlists page

- Playlists can be created and removed
- Playlists can be downloaded
- Songs can be added to playlists
- Songs can be removed from playlists
