#!/bin/sh

if [ -z "$1" ]
  then
    echo "No migration name supplied"
    exit 1
fi

dotnet ef migrations add $1 -s WebServer/WebServer.csproj -p DAL/DAL.csproj