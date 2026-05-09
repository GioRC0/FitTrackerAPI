#!/bin/bash

PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$PROJECT_DIR"

echo "⏹ Deteniendo contenedor del modelo ML..."
docker stop fit-tracker-container

echo "🐳 Deteniendo contenedores de Docker Compose..."
docker compose down

echo ""
echo "✅ Todos los contenedores detenidos."

