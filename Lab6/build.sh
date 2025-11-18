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

echo "Компіляція завершена успішно!"
