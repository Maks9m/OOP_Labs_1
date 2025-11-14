#!/bin/bash

cd "$(dirname "$0")/Lab6/bin/Release/net9.0"

if [ -f "Lab6.exe" ]; then
    ./Lab6.exe
elif [ -f "Lab6" ]; then
    ./Lab6
else
    echo "Помилка: Lab6 не знайдено"
    echo "Спочатку запустіть build.sh для компіляції проектів"
    exit 1
fi
