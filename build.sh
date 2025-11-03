#!/bin/bash

echo "Компіляція Lab6..."
cd Lab6
dotnet build -c Release
if [ $? -ne 0 ]; then
    echo "Помилка компіляції Lab6"
    exit 1
fi
cd ..

echo "Компіляція Object2..."
cd Object2
dotnet build -c Release
if [ $? -ne 0 ]; then
    echo "Помилка компіляції Object2"
    exit 1
fi
cd ..

echo "Компіляція Object3..."
cd Object3
dotnet build -c Release
if [ $? -ne 0 ]; then
    echo "Помилка компіляції Object3"
    exit 1
fi
cd ..

echo "Копіювання файлів..."
LAB6_BIN="Lab6/bin/Release/net9.0"

# Копіюємо Object2
cp -f Object2/bin/Release/net9.0/Object2.dll "$LAB6_BIN/"
cp -f Object2/bin/Release/net9.0/Object2.exe "$LAB6_BIN/" 2>/dev/null || cp -f Object2/bin/Release/net9.0/Object2 "$LAB6_BIN/"

# Копіюємо Object3
cp -f Object3/bin/Release/net9.0/Object3.dll "$LAB6_BIN/"
cp -f Object3/bin/Release/net9.0/Object3.exe "$LAB6_BIN/" 2>/dev/null || cp -f Object3/bin/Release/net9.0/Object3 "$LAB6_BIN/"

echo "Компіляція завершена успішно!"
echo "Виконувані файли знаходяться в: $LAB6_BIN"
