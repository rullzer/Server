#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )/../" >/dev/null 2>&1 && pwd )"

# Restore Nuget
cd $DIR/Server/ && dotnet restore
cd $DIR/Client/ && dotnet restore
cd $DIR/Client.Tests/ && dotnet restore
cd $DIR/Server.Tests/ && dotnet restore
cd $DIR/Shared/ && dotnet restore

# Install NPM dependencies and compile CSS
cd $DIR/Client && npm install
cd $DIR/Client && gulp sass
