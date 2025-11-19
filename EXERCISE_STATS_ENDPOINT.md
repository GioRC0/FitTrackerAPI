# 📊 Endpoint de Estadísticas Completas del Ejercicio

## ✅ IMPLEMENTACIÓN COMPLETA

Se ha implementado exitosamente el endpoint de estadísticas completas del ejercicio que combina el resumen semanal y las sesiones recientes.

---

## 🚀 ENDPOINT

### **GET /api/trainingSessions/exercise/{exerciseId}/stats**

**Descripción:** Obtiene estadísticas completas de un ejercicio para el usuario autenticado, incluyendo resumen de la semana actual y las últimas sesiones realizadas.

**Autenticación:** Requiere JWT Bearer token

**Parámetros de Ruta:**
- `exerciseId` (string, requerido): ID del ejercicio

**Query Parameters:**
- `recentLimit` (int, opcional, default=5, min=1, max=50): Número de sesiones recientes a retornar

---

## 📥 REQUEST

### Ejemplo 1: Estadísticas con 5 sesiones recientes (default)
```http
GET /api/trainingSessions/exercise/6915a0637c585f65f4556f5b/stats
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Ejemplo 2: Estadísticas con 10 sesiones recientes
```http
GET /api/trainingSessions/exercise/6915a0637c585f65f4556f5b/stats?recentLimit=10
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📤 RESPONSE

### **200 OK** - Éxito

#### Para ejercicios con repeticiones (Push-ups / Squats)

```json
{
  "data": {
    "weeklySummary": {
      "totalSessions": 5,
      "totalReps": 125,
      "totalSeconds": 0,
      "averageReps": 25.0,
      "averageSeconds": 0.0,
      "bestSessionReps": 30,
      "bestSessionSeconds": 0,
      "improvementPercentage": 15.5,
      "exerciseType": "pushup"
    },
    "recentSessions": [
      {
        "id": "675c123456789abcdef12345",
        "date": "2025-11-17T10:30:00Z",
        "reps": 30,
        "seconds": 0,
        "duration": "5 min",
        "qualityLabel": "Excelente",
        "techniquePercentage": 92.5
      },
      {
        "id": "675c123456789abcdef12346",
        "date": "2025-11-16T09:15:00Z",
        "reps": 25,
        "seconds": 0,
        "duration": "4 min",
        "qualityLabel": "Buena",
        "techniquePercentage": 78.3
      },
      {
        "id": "675c123456789abcdef12347",
        "date": "2025-11-15T14:20:00Z",
        "reps": 22,
        "seconds": 0,
        "duration": "4 min",
        "qualityLabel": "Buena",
        "techniquePercentage": 75.8
      },
      {
        "id": "675c123456789abcdef12348",
        "date": "2025-11-14T11:00:00Z",
        "reps": 28,
        "seconds": 0,
        "duration": "5 min",
        "qualityLabel": "Excelente",
        "techniquePercentage": 88.2
      },
      {
        "id": "675c123456789abcdef12349",
        "date": "2025-11-13T16:45:00Z",
        "reps": 20,
        "seconds": 0,
        "duration": "3 min",
        "qualityLabel": "Regular",
        "techniquePercentage": 65.0
      }
    ]
  }
}
```

#### Para ejercicios de tiempo (Plank)

```json
{
  "data": {
    "weeklySummary": {
      "totalSessions": 4,
      "totalReps": 0,
      "totalSeconds": 240,
      "averageReps": 0.0,
      "averageSeconds": 60.0,
      "bestSessionReps": 0,
      "bestSessionSeconds": 75,
      "improvementPercentage": 20.0,
      "exerciseType": "plank"
    },
    "recentSessions": [
      {
        "id": "675c123456789abcdef12350",
        "date": "2025-11-17T08:00:00Z",
        "reps": 0,
        "seconds": 75,
        "duration": "1 min",
        "qualityLabel": "Excelente",
        "techniquePercentage": 90.5
      },
      {
        "id": "675c123456789abcdef12351",
        "date": "2025-11-16T07:30:00Z",
        "reps": 0,
        "seconds": 60,
        "duration": "1 min",
        "qualityLabel": "Buena",
        "techniquePercentage": 82.0
      },
      {
        "id": "675c123456789abcdef12352",
        "date": "2025-11-15T08:15:00Z",
        "reps": 0,
        "seconds": 55,
        "duration": "55 seg",
        "qualityLabel": "Buena",
        "techniquePercentage": 78.5
      },
      {
        "id": "675c123456789abcdef12353",
        "date": "2025-11-14T07:45:00Z",
        "reps": 0,
        "seconds": 50,
        "duration": "50 seg",
        "qualityLabel": "Regular",
        "techniquePercentage": 68.0
      }
    ]
  }
}
```

#### Sin sesiones esta semana

```json
{
  "data": {
    "weeklySummary": {
      "totalSessions": 0,
      "totalReps": 0,
      "totalSeconds": 0,
      "averageReps": 0.0,
      "averageSeconds": 0.0,
      "bestSessionReps": 0,
      "bestSessionSeconds": 0,
      "improvementPercentage": 0.0,
      "exerciseType": "unknown"
    },
    "recentSessions": []
  }
}
```

### **401 Unauthorized** - No autenticado

```json
{
  "message": "Usuario no autenticado"
}
```

### **500 Internal Server Error** - Error del servidor

```json
{
  "message": "Error al obtener estadísticas del ejercicio",
  "error": "Detalles del error..."
}
```

---

## 📊 ESTRUCTURA DE DATOS

### **ExerciseStatsDto**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `weeklySummary` | `WeeklySummaryDto` | Resumen de estadísticas de la semana actual |
| `recentSessions` | `List<RecentSessionDto>` | Lista de sesiones recientes |

### **WeeklySummaryDto**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `totalSessions` | `int` | Total de sesiones realizadas esta semana |
| `totalReps` | `int` | Total de repeticiones (0 para plank) |
| `totalSeconds` | `int` | Total de segundos (para plank, 0 para otros) |
| `averageReps` | `double` | Promedio de repeticiones por sesión |
| `averageSeconds` | `double` | Promedio de segundos por sesión (para plank) |
| `bestSessionReps` | `int` | Mejor sesión en número de repeticiones |
| `bestSessionSeconds` | `int` | Mejor sesión en segundos (para plank) |
| `improvementPercentage` | `double` | % de mejora vs semana anterior |
| `exerciseType` | `string` | Tipo de ejercicio: "pushup", "squat", "plank" |

### **RecentSessionDto**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | `string` | ID de la sesión |
| `date` | `DateTime` | Fecha y hora de inicio de la sesión |
| `reps` | `int` | Número de repeticiones (0 para plank) |
| `seconds` | `int` | Número de segundos (para plank, 0 para otros) |
| `duration` | `string` | Duración formateada: "{n} min" o "{n} seg" |
| `qualityLabel` | `string` | Etiqueta de calidad: "Excelente", "Buena", "Regular", "Mala" |
| `techniquePercentage` | `double` | Porcentaje de técnica correcta (redondeado a 1 decimal) |

---

## 🎯 LÓGICA DE NEGOCIO

### **Resumen Semanal**
- **Semana actual**: Últimos 7 días desde hoy
- **Semana anterior**: Días 8-14 atrás desde hoy
- **ImprovementPercentage**: Calcula el % de cambio entre el promedio de la semana actual vs la anterior
  - Para push-ups y squats: Basado en `averageReps`
  - Para plank: Basado en `averageSeconds`
  - Fórmula: `((nuevo - anterior) / anterior) * 100`
  - Positivo = mejora 📈
  - Negativo = disminución 📉

### **Etiquetas de Calidad (QualityLabel)**

| Rango | Etiqueta | Color sugerido |
|-------|----------|----------------|
| ≥ 85% | Excelente | Verde 🟢 |
| 70% - 84.9% | Buena | Azul 🔵 |
| 50% - 69.9% | Regular | Amarillo 🟡 |
| < 50% | Mala | Rojo 🔴 |

### **Formato de Duración (Duration)**

```csharp
if (durationSeconds >= 60)
    return "{minutos} min"
else
    return "{segundos} seg"
```

**Ejemplos:**
- 45 segundos → `"45 seg"`
- 60 segundos → `"1 min"`
- 90 segundos → `"1 min"`
- 300 segundos → `"5 min"`

---

## 🧪 EJEMPLOS DE USO

### Desde Flutter (Dart)

```dart
class TrainingService {
  final String baseUrl = 'http://your-api-url:5180';
  String? _token;

  Future<ExerciseStats> getExerciseStats(String exerciseId, {int recentLimit = 5}) async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/trainingSessions/exercise/$exerciseId/stats')
          .replace(queryParameters: {'recentLimit': recentLimit.toString()}),
      headers: {
        'Authorization': 'Bearer $_token',
      },
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'];
      return ExerciseStats.fromJson(data);
    } else if (response.statusCode == 401) {
      throw Exception('Token expirado, necesita renovar');
    } else {
      throw Exception('Error al obtener estadísticas: ${response.body}');
    }
  }
}

// Uso en widget
void loadExerciseStats() async {
  try {
    final stats = await _trainingService.getExerciseStats(widget.exerciseId, recentLimit: 10);
    
    setState(() {
      _weeklySummary = stats.weeklySummary;
      _recentSessions = stats.recentSessions;
    });
  } catch (e) {
    print('Error: $e');
    // Mostrar error al usuario
  }
}
```

### Desde JavaScript (Fetch API)

```javascript
async function getExerciseStats(exerciseId, recentLimit = 5) {
  const token = localStorage.getItem('jwt_token');
  
  const response = await fetch(
    `http://localhost:5180/api/trainingSessions/exercise/${exerciseId}/stats?recentLimit=${recentLimit}`,
    {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    }
  );

  if (response.ok) {
    const result = await response.json();
    return result.data;
  } else if (response.status === 401) {
    throw new Error('No autorizado');
  } else {
    throw new Error('Error al obtener estadísticas');
  }
}

// Uso
getExerciseStats('6915a0637c585f65f4556f5b', 10)
  .then(stats => {
    console.log('Resumen semanal:', stats.weeklySummary);
    console.log('Sesiones recientes:', stats.recentSessions);
  })
  .catch(error => console.error('Error:', error));
```

---

## 📱 CASOS DE USO EN LA APP

### 1. Pantalla de Resumen del Ejercicio

```
┌─────────────────────────────────┐
│  Push-ups                       │
├─────────────────────────────────┤
│  📊 Resumen Semanal             │
│  ┌───────────────────────────┐ │
│  │ 5 sesiones                │ │
│  │ 125 reps totales          │ │
│  │ 25 reps promedio          │ │
│  │ 🏆 Mejor: 30 reps         │ │
│  │ 📈 Mejora: +15.5%         │ │
│  └───────────────────────────┘ │
│                                 │
│  📝 Sesiones Recientes          │
│  ┌───────────────────────────┐ │
│  │ Hoy 10:30 AM              │ │
│  │ 30 reps • 5 min           │ │
│  │ ⭐ Excelente (92.5%)      │ │
│  ├───────────────────────────┤ │
│  │ Ayer 9:15 AM              │ │
│  │ 25 reps • 4 min           │ │
│  │ ✓ Buena (78.3%)           │ │
│  ├───────────────────────────┤ │
│  │ 15 Nov 2:20 PM            │ │
│  │ 22 reps • 4 min           │ │
│  │ ✓ Buena (75.8%)           │ │
│  └───────────────────────────┘ │
└─────────────────────────────────┘
```

### 2. Widget de Progreso

```dart
class ExerciseProgressWidget extends StatelessWidget {
  final WeeklySummary summary;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: EdgeInsets.all(16),
        child: Column(
          children: [
            Text('Esta Semana', style: Theme.of(context).textTheme.headline6),
            SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                _buildStat('Sesiones', summary.totalSessions.toString()),
                _buildStat('Total', '${summary.totalReps} reps'),
                _buildStat('Promedio', '${summary.averageReps} reps'),
              ],
            ),
            SizedBox(height: 16),
            _buildImprovementBadge(summary.improvementPercentage),
          ],
        ),
      ),
    );
  }

  Widget _buildImprovementBadge(double improvement) {
    final isPositive = improvement >= 0;
    return Container(
      padding: EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: isPositive ? Colors.green : Colors.red,
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            isPositive ? Icons.trending_up : Icons.trending_down,
            color: Colors.white,
            size: 18,
          ),
          SizedBox(width: 4),
          Text(
            '${improvement > 0 ? '+' : ''}${improvement.toStringAsFixed(1)}%',
            style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
          ),
        ],
      ),
    );
  }
}
```

---

## ✅ VALIDACIONES

### Parámetros de Request
- `recentLimit` debe estar entre 1 y 50
- Si está fuera del rango, se usa el default (5)
- `exerciseId` debe ser un ObjectId válido

### Autenticación
- JWT token válido y no expirado
- UserId extraído automáticamente del token
- Solo se retornan sesiones del usuario autenticado

### Datos
- Si no hay sesiones esta semana, retorna objeto vacío
- Si no hay semana anterior, `improvementPercentage` = 0
- Sesiones ordenadas por fecha descendente (más reciente primero)

---

## 🎯 MEJORES PRÁCTICAS

### ✅ DO
1. **Cache en Flutter**: Guarda los resultados por 5-10 minutos
2. **Pull to refresh**: Permite actualizar los datos con gesto
3. **Loading states**: Muestra skeleton/shimmer mientras carga
4. **Error handling**: Maneja errores de red gracefully
5. **Formateo de fechas**: Usa formato relativo (Hoy, Ayer, hace 3 días)
6. **Colores dinámicos**: Usa colores según qualityLabel

### ❌ DON'T
1. ❌ No llames este endpoint cada vez que la app abre
2. ❌ No uses `recentLimit` muy alto sin paginación
3. ❌ No ignores el token expirado (renova automáticamente)
4. ❌ No bloquees UI mientras carga
5. ❌ No olvides manejar el caso de 0 sesiones

---

## 🚀 ARCHIVOS MODIFICADOS/CREADOS

### Creados
✅ `DTOs/Training/ExerciseStatsDto.cs`
✅ `DTOs/Training/WeeklySummaryDto.cs`
✅ `DTOs/Training/RecentSessionDto.cs`
✅ `ExerciseStats.http` - Archivo de pruebas

### Modificados
✅ `Repositories/Training/ITrainingSessionRepository.cs` - Agregado `GetRecentSessionsAsync`
✅ `Repositories/Training/TrainingSessionRepository.cs` - Implementado `GetRecentSessionsAsync`
✅ `Services/Training/ITrainingSessionService.cs` - Agregado `GetExerciseStatsAsync`
✅ `Services/Training/TrainingSessionService.cs` - Implementado `GetExerciseStatsAsync` + helpers
✅ `Controllers/TrainingSessionsController.cs` - Agregado endpoint `/exercise/{exerciseId}/stats`

---

## 🎉 ¡LISTO PARA USAR!

El endpoint está completamente implementado y listo para integrarse con tu app Flutter. Proporciona toda la información necesaria para mostrar un resumen completo del progreso del usuario en cada ejercicio.

**Próximos pasos:**
1. Prueba el endpoint con el archivo `.http` o Swagger
2. Integra con tu app Flutter
3. Diseña widgets para mostrar las estadísticas
4. Implementa cache local para mejorar UX offline

¡Disfruta del nuevo endpoint! 💪📊

