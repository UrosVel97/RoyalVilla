#!/usr/bin/env bash
set -euo pipefail

cd /workspaces/RoyalVilla

dotnet tool restore
dotnet restore RoyalVilla.slnx
dotnet build RoyalVilla.slnx --no-restore