#!/bin/bash

echo Update yt-dlp
yt-dlp -U

echo "Check if login is necessary"

if [[ $SETTINGS__YTUSEOAUTH -eq "true" ]]
  until yt-dlp --simulate https://www.youtube.com/watch?v=dQw4w9WgXcQ --username oauth2 --password ''
  do
    sleep 30
  done
fi

$@
