#!/bin/bash

PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "📂 Entrando al proyecto: $PROJECT_DIR"
cd "$PROJECT_DIR"

echo "📥 Pulling últimos cambios del repo..."
git pull

echo "🐳 Levantando contenedores..."
docker compose up -d --build

echo "🤖 Iniciando contenedor del modelo ML..."
docker start fit-tracker-container

echo ""
echo "✅ Deploy completado!"
echo "🔗 API corriendo en: http://localhost:8081"
echo ""
echo "📋 Estado de los contenedores:"
docker compose ps
docker ps --filter "name=fit-tracker-container" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
