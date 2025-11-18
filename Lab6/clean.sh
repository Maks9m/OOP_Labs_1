#!/usr/bin/env bash
# clean.sh - clean Lab6 workspace build artifacts
# Usage: ./clean.sh
set -euo pipefail
IFS=$'\n\t'

echo "Cleaning Lab6 workspace..."
ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT_DIR"

# Helper to remove bin/obj inside a project if it exists
clean_proj() {
  local proj="$1"
  if [ -d "$proj" ]; then
    echo " - Cleaning project: $proj"
    if [ -d "$proj/bin" ]; then echo "   rm -rf $proj/bin"; rm -rf "$proj/bin"; fi
    if [ -d "$proj/obj" ]; then echo "   rm -rf $proj/obj"; rm -rf "$proj/obj"; fi
  else
    echo "   (skip) $proj not found"
  fi
}

# Projects to clean
clean_proj "Lab6"
clean_proj "Object2"
clean_proj "Object3"

# dotnet clean for each project (safe even if already cleaned)
for p in "Lab6" "Object2" "Object3"; do
  if [ -f "$p/$p.csproj" ]; then
    echo "Running dotnet clean for $p"
    dotnet clean "$p/$p.csproj" -c Release || true
    dotnet clean "$p/$p.csproj" -c Debug || true
  else
    echo "No csproj for $p, skipping dotnet clean"
  fi
done

# Remove aggregated out/ folder if present
if [ -d "out" ]; then
  echo "Removing out/"
  rm -rf out
fi

# Remove parameters.txt files in project directories
for p in "Lab6" "Object2" "Object3"; do
  f="$p/parameters.txt"
  if [ -f "$f" ]; then
    echo "Removing $f"
    rm -f "$f"
  fi
done

echo "Clean finished. You may also run: dotnet restore if you want to refresh packages."
