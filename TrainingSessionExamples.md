# Ejemplos de JSON para Training Sessions API

## 1. Ejemplo: Crear sesión de PUSH-UPS

### Endpoint: `POST /api/trainingSessions`
**Headers:**
- `Authorization: Bearer <tu_token_jwt>`

**Body:**
```json
{
  "exerciseId": "6915a0637c585f65f4556f5b",
  "exerciseType": "pushup",
  "exerciseName": "Push-ups",
  "startTime": "2025-01-12T10:00:00Z",
  "endTime": "2025-01-12T10:05:00Z",
  "durationSeconds": 300,
  "totalReps": 15,
  "repsData": [
    {
      "repNumber": 1,
      "classification": "pushup_correcto",
      "confidence": 0.95,
      "probabilities": {
        "pushup_correcto": 0.95,
        "pushup_codos_abiertos": 0.03,
        "pushup_cadera_caida": 0.02
      },
      "timestamp": "2025-01-12T10:00:15Z"
    },
    {
      "repNumber": 2,
      "classification": "pushup_correcto",
      "confidence": 0.93,
      "probabilities": {
        "pushup_correcto": 0.93,
        "pushup_codos_abiertos": 0.05,
        "pushup_cadera_caida": 0.02
      },
      "timestamp": "2025-01-12T10:00:30Z"
    },
    {
      "repNumber": 3,
      "classification": "pushup_codos_abiertos",
      "confidence": 0.88,
      "probabilities": {
        "pushup_correcto": 0.10,
        "pushup_codos_abiertos": 0.88,
        "pushup_cadera_caida": 0.02
      },
      "timestamp": "2025-01-12T10:00:45Z"
    }
  ],
  "metrics": {
    "techniquePercentage": 86.7,
    "consistencyScore": 92.5,
    "averageConfidence": 0.92,
    "controlScore": 88.0,
    "stabilityScore": 90.5,
    "repsPerMinute": 3.0
  }
}
```

---

## 2. Ejemplo: Crear sesión de SQUATS

### Endpoint: `POST /api/trainingSessions`
**Headers:**
- `Authorization: Bearer <tu_token_jwt>`

**Body:**
```json
{
  "exerciseId": "6915a0637c585f65f4556f5c",
  "exerciseType": "squat",
  "exerciseName": "Sentadillas",
  "startTime": "2025-01-12T11:00:00Z",
  "endTime": "2025-01-12T11:06:00Z",
  "durationSeconds": 360,
  "totalReps": 20,
  "repsData": [
    {
      "repNumber": 1,
      "classification": "squat_correcto",
      "confidence": 0.96,
      "probabilities": {
        "squat_correcto": 0.96,
        "squat_rodillas_adelante": 0.02,
        "squat_profundidad_incorrecta": 0.02
      },
      "timestamp": "2025-01-12T11:00:18Z"
    },
    {
      "repNumber": 2,
      "classification": "squat_correcto",
      "confidence": 0.94,
      "probabilities": {
        "squat_correcto": 0.94,
        "squat_rodillas_adelante": 0.04,
        "squat_profundidad_incorrecta": 0.02
      },
      "timestamp": "2025-01-12T11:00:36Z"
    },
    {
      "repNumber": 3,
      "classification": "squat_profundidad_incorrecta",
      "confidence": 0.87,
      "probabilities": {
        "squat_correcto": 0.10,
        "squat_rodillas_adelante": 0.03,
        "squat_profundidad_incorrecta": 0.87
      },
      "timestamp": "2025-01-12T11:00:54Z"
    }
  ],
  "metrics": {
    "techniquePercentage": 90.0,
    "consistencyScore": 88.5,
    "averageConfidence": 0.92,
    "controlScore": 87.0,
    "stabilityScore": 89.0,
    "alignmentScore": 91.5,
    "balanceScore": 88.0,
    "depthScore": 85.5,
    "repsPerMinute": 3.33
  }
}
```

---

## 3. Ejemplo: Crear sesión de PLANCHA (PLANK)

### Endpoint: `POST /api/trainingSessions`
**Headers:**
- `Authorization: Bearer <tu_token_jwt>`

**Body:**
```json
{
  "exerciseId": "6915a0637c585f65f4556f5d",
  "exerciseType": "plank",
  "exerciseName": "Plancha",
  "startTime": "2025-01-12T12:00:00Z",
  "endTime": "2025-01-12T12:01:00Z",
  "durationSeconds": 60,
  "totalSeconds": 60,
  "secondsData": [
    {
      "secondNumber": 1,
      "classification": "plank_correcto",
      "confidence": 0.97,
      "probabilities": {
        "plank_correcto": 0.97,
        "plank_cadera_caida": 0.02,
        "plank_cadera_elevada": 0.01
      },
      "timestamp": "2025-01-12T12:00:01Z"
    },
    {
      "secondNumber": 30,
      "classification": "plank_correcto",
      "confidence": 0.93,
      "probabilities": {
        "plank_correcto": 0.93,
        "plank_cadera_caida": 0.05,
        "plank_cadera_elevada": 0.02
      },
      "timestamp": "2025-01-12T12:00:30Z"
    },
    {
      "secondNumber": 50,
      "classification": "plank_cadera_caida",
      "confidence": 0.85,
      "probabilities": {
        "plank_correcto": 0.12,
        "plank_cadera_caida": 0.85,
        "plank_cadera_elevada": 0.03
      },
      "timestamp": "2025-01-12T12:00:50Z"
    },
    {
      "secondNumber": 60,
      "classification": "plank_cadera_caida",
      "confidence": 0.88,
      "probabilities": {
        "plank_correcto": 0.09,
        "plank_cadera_caida": 0.88,
        "plank_cadera_elevada": 0.03
      },
      "timestamp": "2025-01-12T12:01:00Z"
    }
  ],
  "metrics": {
    "techniquePercentage": 83.3,
    "consistencyScore": 87.0,
    "averageConfidence": 0.91,
    "hipScore": 82.0,
    "coreScore": 88.5,
    "armPositionScore": 92.0,
    "resistanceScore": 85.0
  }
}
```

---

## 4. Obtener sesión por ID

### Endpoint: `GET /api/trainingSessions/{id}`
**Headers:**
- `Authorization: Bearer <tu_token_jwt>`

**Ejemplo:**
```
GET /api/trainingSessions/6915a0637c585f65f4556f5e
```

**Respuesta esperada:**
```json
{
  "data": {
    "id": "6915a0637c585f65f4556f5e",
    "userId": "6915a0637c585f65f4556f5b",
    "exerciseId": "6915a0637c585f65f4556f5c",
    "exerciseType": "pushup",
    "exerciseName": "Push-ups",
    "startTime": "2025-01-12T10:00:00Z",
    "endTime": "2025-01-12T10:05:00Z",
    "durationSeconds": 300,
    "totalReps": 15,
    "repsData": [...],
    "metrics": {...},
    "createdAt": "2025-01-12T10:05:30Z"
  }
}
```

---

## 5. Listar sesiones del usuario

### Endpoint: `GET /api/trainingSessions?page=1&pageSize=10`
**Headers:**
- `Authorization: Bearer <tu_token_jwt>`

**Query Parameters:**
- `page` (opcional, default=1): Número de página
- `pageSize` (opcional, default=10, max=100): Cantidad de resultados
- `exerciseId` (opcional): Filtrar por ejercicio específico

**Ejemplos:**
```
GET /api/trainingSessions?page=1&pageSize=20
GET /api/trainingSessions?page=1&pageSize=10&exerciseId=6915a0637c585f65f4556f5c
```

**Respuesta esperada:**
```json
{
  "data": [
    {
      "id": "...",
      "userId": "...",
      "exerciseId": "...",
      "exerciseType": "pushup",
      "exerciseName": "Push-ups",
      "startTime": "2025-01-12T10:00:00Z",
      "endTime": "2025-01-12T10:05:00Z",
      "durationSeconds": 300,
      "totalReps": 15,
      "metrics": {...}
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "total": 5
  }
}
```

---

## 6. Obtener progreso semanal

### Endpoint: `GET /api/trainingSessions/weekly-progress/{exerciseId}`
**Headers:**
- `Authorization: Bearer <tu_token_jwt>`

**Ejemplo:**
```
GET /api/trainingSessions/weekly-progress/6915a0637c585f65f4556f5c
```

**Respuesta esperada:**
```json
{
  "data": {
    "exerciseId": "6915a0637c585f65f4556f5c",
    "exerciseName": "Push-ups",
    "currentWeek": {
      "totalSessions": 5,
      "totalReps": 75,
      "totalSeconds": 0,
      "averageTechniquePercentage": 88.5,
      "averageConsistencyScore": 90.2,
      "averageConfidence": 0.92
    },
    "previousWeek": {
      "totalSessions": 3,
      "totalReps": 45,
      "totalSeconds": 0,
      "averageTechniquePercentage": 82.0,
      "averageConsistencyScore": 85.5,
      "averageConfidence": 0.88
    },
    "comparison": {
      "sessionsChange": 66.67,
      "repsChange": 66.67,
      "secondsChange": 0,
      "techniqueChange": 7.93,
      "consistencyChange": 5.50,
      "confidenceChange": 4.55
    }
  }
}
```

---

## Notas importantes:

1. **Autenticación**: Todos los endpoints requieren el token JWT en el header `Authorization: Bearer <token>`

2. **UserId**: El userId se extrae automáticamente del token JWT, no necesitas enviarlo en el body

3. **Validación de propiedad**: El sistema valida que solo puedas acceder a tus propias sesiones

4. **Tipos de ejercicio**: Los valores válidos para `exerciseType` son:
   - `"pushup"` - Para flexiones
   - `"squat"` - Para sentadillas
   - `"plank"` - Para plancha

5. **Campos condicionales**:
   - Para push-ups y squats: usa `totalReps` y `repsData`
   - Para plancha: usa `totalSeconds` y `secondsData`

6. **Métricas**: Las métricas se calculan en la app Flutter y se envían ya calculadas al API

7. **Timestamps**: Usa formato ISO 8601 (UTC) para todas las fechas: `"2025-01-12T10:00:00Z"`

8. **Progreso semanal**: 
   - Semana actual = últimos 7 días
   - Semana anterior = días 8-14 atrás
   - Comparison muestra el % de cambio (positivo = mejora, negativo = disminución)

