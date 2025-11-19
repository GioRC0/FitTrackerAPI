# 📊 Progress Dashboard API - Documentación Completa

## ✅ IMPLEMENTACIÓN COMPLETA

Se han implementado exitosamente **3 endpoints principales** para el dashboard de progreso de ejercicios con análisis detallado de rendimiento y objetivos.

---

## 🚀 ENDPOINTS IMPLEMENTADOS

### **1. GET /api/trainingSessions/exercise/{exerciseId}/progress**

**Descripción:** Obtiene datos históricos de progreso para visualizar gráficas de rendimiento.

**Query Parameters:**
- `range` (string, requerido): `"week"` o `"month"`

**Response Example (Week):**
```json
{
  "success": true,
  "data": {
    "timeRange": "week",
    "exerciseId": "6915f4b1504d0bd2afc9fef6",
    "exerciseName": "Push-ups",
    "exerciseType": "pushup",
    "dataPoints": [
      {
        "label": "Lun",
        "date": "2024-11-11T00:00:00Z",
        "reps": 20,
        "seconds": 0,
        "averageForm": 85.5
      },
      {
        "label": "Mar",
        "date": "2024-11-12T00:00:00Z",
        "reps": 22,
        "seconds": 0,
        "averageForm": 90.0
      },
      {
        "label": "Mié",
        "date": "2024-11-13T00:00:00Z",
        "reps": 25,
        "seconds": 0,
        "averageForm": 92.5
      }
    ],
    "summary": {
      "totalReps": 125,
      "totalSeconds": 0,
      "totalSessions": 6,
      "daysWithActivity": 6,
      "averagePerDay": 17.9,
      "averageFormScore": 89.2,
      "bestDay": {
        "label": "Sáb",
        "value": 30
      },
      "improvement": 15.5,
      "consistency": "Excelente"
    }
  }
}
```

**Response Example (Month):**
```json
{
  "success": true,
  "data": {
    "timeRange": "month",
    "exerciseId": "6915f4b1504d0bd2afc9fef6",
    "exerciseName": "Plancha",
    "exerciseType": "plank",
    "dataPoints": [
      {
        "label": "Sem 1",
        "date": "2024-10-21T00:00:00Z",
        "reps": 0,
        "seconds": 180,
        "averageForm": 82.0
      },
      {
        "label": "Sem 2",
        "date": "2024-10-28T00:00:00Z",
        "reps": 0,
        "seconds": 210,
        "averageForm": 85.5
      },
      {
        "label": "Sem 3",
        "date": "2024-11-04T00:00:00Z",
        "reps": 0,
        "seconds": 240,
        "averageForm": 88.0
      },
      {
        "label": "Sem 4",
        "date": "2024-11-11T00:00:00Z",
        "reps": 0,
        "seconds": 270,
        "averageForm": 92.0
      }
    ],
    "summary": {
      "totalReps": 0,
      "totalSeconds": 900,
      "totalSessions": 12,
      "daysWithActivity": 12,
      "averagePerDay": 32.1,
      "averageFormScore": 86.9,
      "bestDay": {
        "label": "Sem 4",
        "value": 270
      },
      "improvement": 20.0,
      "consistency": "Buena"
    }
  }
}
```

---

### **2. GET /api/trainingSessions/exercise/{exerciseId}/form-analysis**

**Descripción:** Análisis detallado de técnica con desglose por aspectos según tipo de ejercicio.

**Query Parameters:**
- `range` (string, requerido): `"week"` o `"month"`

**Response Example (Push-ups):**
```json
{
  "success": true,
  "data": {
    "averageScore": 89.5,
    "aspectScores": [
      {
        "aspect": "Postura",
        "score": 92.0,
        "metric": "controlScore"
      },
      {
        "aspect": "Velocidad",
        "score": 3.5,
        "metric": "repsPerMinute"
      },
      {
        "aspect": "Rango",
        "score": 88.0,
        "metric": "depthScore"
      },
      {
        "aspect": "Estabilidad",
        "score": 90.5,
        "metric": "stabilityScore"
      },
      {
        "aspect": "Consistencia",
        "score": 87.0,
        "metric": "consistencyScore"
      }
    ],
    "trend": [
      {
        "date": "2024-11-11T00:00:00Z",
        "score": 85.5
      },
      {
        "date": "2024-11-12T00:00:00Z",
        "score": 87.0
      },
      {
        "date": "2024-11-13T00:00:00Z",
        "score": 90.5
      },
      {
        "date": "2024-11-14T00:00:00Z",
        "score": 92.0
      }
    ]
  }
}
```

**Response Example (Plank):**
```json
{
  "success": true,
  "data": {
    "averageScore": 86.5,
    "aspectScores": [
      {
        "aspect": "Postura",
        "score": 88.0,
        "metric": "hipScore"
      },
      {
        "aspect": "Estabilidad",
        "score": 85.5,
        "metric": "stabilityScore"
      },
      {
        "aspect": "Core",
        "score": 90.0,
        "metric": "coreScore"
      },
      {
        "aspect": "Brazos",
        "score": 87.0,
        "metric": "armPositionScore"
      },
      {
        "aspect": "Resistencia",
        "score": 82.0,
        "metric": "resistanceScore"
      }
    ],
    "trend": [...]
  }
}
```

---

### **3. GET /api/trainingSessions/exercise/{exerciseId}/goals**

**Descripción:** Objetivos y metas del usuario con progreso actual.

**Response Example:**
```json
{
  "success": true,
  "data": {
    "goals": [
      {
        "id": "streak_10_days",
        "title": "Racha de 10 días",
        "description": "Entrena 10 días consecutivos",
        "current": 7,
        "target": 10,
        "progress": 70.0,
        "achieved": false
      },
      {
        "id": "total_reps_200",
        "title": "200 repeticiones totales",
        "description": "Alcanza 200 reps en total",
        "current": 145,
        "target": 200,
        "progress": 72.5,
        "achieved": false
      },
      {
        "id": "perfect_technique",
        "title": "Técnica perfecta",
        "description": "Alcanza 95 puntos de técnica promedio",
        "current": 89,
        "target": 95,
        "progress": 93.7,
        "achieved": false
      }
    ],
    "currentStreak": 7,
    "longestStreak": 12
  }
}
```

---

## 📊 LÓGICA DE NEGOCIO DETALLADA

### **1. Progress Data**

#### **Agrupación por Semana (range=week)**
- Muestra **últimos 7 días** (incluyendo hoy)
- Etiquetas: "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"
- Cada día es un punto de datos individual

#### **Agrupación por Mes (range=month)**
- Muestra **últimas 4 semanas** (28 días)
- Etiquetas: "Sem 1", "Sem 2", "Sem 3", "Sem 4"
- Cada semana es un punto de datos agregado

#### **Cálculo de Mejora (Improvement)**

**Para Week:**
```csharp
promedioPrimeros3Dias = Avg(día 1, 2, 3)
promedioÚltimos3Días = Avg(día 5, 6, 7)
mejora = ((últimos - primeros) / primeros) * 100
```

**Para Month:**
```csharp
semana1 = Total de primera semana
semana4 = Total de última semana
mejora = ((semana4 - semana1) / semana1) * 100
```

**Ejemplos:**
- Primera mitad: 20 reps promedio
- Segunda mitad: 25 reps promedio
- **Mejora: +25%** 📈

#### **Consistencia**

**Para Week (sobre 7 días):**
- ✅ **Excelente**: ≥6 días con actividad
- 👍 **Buena**: 4-5 días
- ⚠️ **Regular**: 2-3 días
- ❌ **Baja**: ≤1 día

**Para Month (sobre 28 días):**
- ✅ **Excelente**: ≥20 días con actividad
- 👍 **Buena**: 12-19 días
- ⚠️ **Regular**: 6-11 días
- ❌ **Baja**: ≤5 días

---

### **2. Form Analysis**

#### **Mapeo de Aspectos por Ejercicio**

| Ejercicio | Aspecto 1 | Aspecto 2 | Aspecto 3 | Aspecto 4 | Aspecto 5 |
|-----------|-----------|-----------|-----------|-----------|-----------|
| **Push-ups** | Postura (controlScore) | Velocidad (repsPerMinute) | Rango (depthScore) | Estabilidad (stabilityScore) | Consistencia (consistencyScore) |
| **Squats** | Postura (alignmentScore) | Velocidad (repsPerMinute) | Rango (depthScore) | Estabilidad (balanceScore) | Consistencia (consistencyScore) |
| **Plank** | Postura (hipScore) | Estabilidad (stabilityScore) | Core (coreScore) | Brazos (armPositionScore) | Resistencia (resistanceScore) |

#### **Cálculo de Scores**
```csharp
aspectScore = Promedio(métrica) de todas las sesiones del período
```

#### **Trend (Tendencia)**
- Agrupa sesiones por día
- Calcula promedio de `techniquePercentage` por día
- Ordena cronológicamente
- Útil para gráficos de línea

---

### **3. Goals (Objetivos)**

#### **Goal 1: Racha de Días Consecutivos**

**Racha Actual:**
```csharp
1. Obtener todas las fechas con sesiones
2. Ordenar descendente desde hoy
3. Contar días consecutivos hacia atrás
4. Si no hay sesión hoy ni ayer, racha = 0
```

**Ejemplo:**
```
Sesiones: 17/Nov, 16/Nov, 15/Nov, 14/Nov, 12/Nov
Hoy: 18/Nov
Racha: 0 (se rompió ayer)

Sesiones: 18/Nov, 17/Nov, 16/Nov, 15/Nov
Hoy: 18/Nov
Racha: 4 días
```

**Racha Más Larga:**
```csharp
1. Obtener todas las fechas con sesiones históricas
2. Identificar todas las rachas consecutivas
3. Retornar la más larga
```

#### **Goal 2: Total de Reps/Segundos**
```csharp
Para push-ups/squats: Suma de todos los totalReps
Para plank: Suma de todos los totalSeconds
```

#### **Goal 3: Técnica Perfecta**
```csharp
Promedio de techniquePercentage de las últimas 10 sesiones
Meta: ≥95%
```

---

## 🎨 CASOS DE USO EN LA APP

### **1. Gráfica de Progreso Semanal**

```dart
// Llamar al endpoint
final progressData = await api.getProgressData(exerciseId, 'week');

// Crear gráfica de barras
BarChart(
  data: progressData.dataPoints.map((point) => 
    BarData(
      label: point.label, // "Lun", "Mar", etc.
      value: point.reps,  // o point.seconds para plank
    )
  ).toList(),
);

// Mostrar resumen
Text('Total: ${progressData.summary.totalReps} reps');
Text('Promedio: ${progressData.summary.averagePerDay} reps/día');
Text('Mejor día: ${progressData.summary.bestDay.label} (${progressData.summary.bestDay.value})');

// Badge de mejora
ImprovementBadge(
  percentage: progressData.summary.improvement,
  isPositive: progressData.summary.improvement > 0,
);

// Badge de consistencia
ConsistencyBadge(
  label: progressData.summary.consistency,
  color: getConsistencyColor(progressData.summary.consistency),
);
```

### **2. Análisis de Técnica (Radar Chart)**

```dart
final formAnalysis = await api.getFormAnalysis(exerciseId, 'week');

RadarChart(
  data: formAnalysis.aspectScores.map((aspect) =>
    RadarData(
      label: aspect.aspect,    // "Postura", "Velocidad", etc.
      value: aspect.score / 100, // Normalizar a 0-1
    )
  ).toList(),
);

// Gráfica de tendencia
LineChart(
  data: formAnalysis.trend.map((point) =>
    LineData(date: point.date, value: point.score)
  ).toList(),
);
```

### **3. Objetivos y Metas**

```dart
final goals = await api.getGoals(exerciseId);

// Mostrar rachas
Text('Racha actual: ${goals.currentStreak} días 🔥');
Text('Mejor racha: ${goals.longestStreak} días 🏆');

// Mostrar progreso de cada meta
for (var goal in goals.goals) {
  GoalCard(
    title: goal.title,
    description: goal.description,
    progress: goal.progress,
    current: goal.current,
    target: goal.target,
    achieved: goal.achieved,
  );
}
```

---

## 📱 INTEGRACIÓN CON FLUTTER

### **Modelos Dart**

```dart
class ProgressData {
  final String timeRange;
  final String exerciseId;
  final String exerciseName;
  final String exerciseType;
  final List<ProgressDataPoint> dataPoints;
  final ProgressSummary summary;

  ProgressData({
    required this.timeRange,
    required this.exerciseId,
    required this.exerciseName,
    required this.exerciseType,
    required this.dataPoints,
    required this.summary,
  });

  factory ProgressData.fromJson(Map<String, dynamic> json) {
    return ProgressData(
      timeRange: json['timeRange'],
      exerciseId: json['exerciseId'],
      exerciseName: json['exerciseName'],
      exerciseType: json['exerciseType'],
      dataPoints: (json['dataPoints'] as List)
          .map((p) => ProgressDataPoint.fromJson(p))
          .toList(),
      summary: ProgressSummary.fromJson(json['summary']),
    );
  }
}

class ProgressDataPoint {
  final String label;
  final DateTime date;
  final int reps;
  final int seconds;
  final double averageForm;

  ProgressDataPoint({
    required this.label,
    required this.date,
    required this.reps,
    required this.seconds,
    required this.averageForm,
  });

  factory ProgressDataPoint.fromJson(Map<String, dynamic> json) {
    return ProgressDataPoint(
      label: json['label'],
      date: DateTime.parse(json['date']),
      reps: json['reps'],
      seconds: json['seconds'],
      averageForm: json['averageForm'].toDouble(),
    );
  }
}

class ProgressSummary {
  final int totalReps;
  final int totalSeconds;
  final int totalSessions;
  final int daysWithActivity;
  final double averagePerDay;
  final double averageFormScore;
  final BestDay bestDay;
  final double improvement;
  final String consistency;

  ProgressSummary({
    required this.totalReps,
    required this.totalSeconds,
    required this.totalSessions,
    required this.daysWithActivity,
    required this.averagePerDay,
    required this.averageFormScore,
    required this.bestDay,
    required this.improvement,
    required this.consistency,
  });

  factory ProgressSummary.fromJson(Map<String, dynamic> json) {
    return ProgressSummary(
      totalReps: json['totalReps'],
      totalSeconds: json['totalSeconds'],
      totalSessions: json['totalSessions'],
      daysWithActivity: json['daysWithActivity'],
      averagePerDay: json['averagePerDay'].toDouble(),
      averageFormScore: json['averageFormScore'].toDouble(),
      bestDay: BestDay.fromJson(json['bestDay']),
      improvement: json['improvement'].toDouble(),
      consistency: json['consistency'],
    );
  }
}

class FormAnalysis {
  final double averageScore;
  final List<AspectScore> aspectScores;
  final List<TrendPoint> trend;

  FormAnalysis({
    required this.averageScore,
    required this.aspectScores,
    required this.trend,
  });

  factory FormAnalysis.fromJson(Map<String, dynamic> json) {
    return FormAnalysis(
      averageScore: json['averageScore'].toDouble(),
      aspectScores: (json['aspectScores'] as List)
          .map((a) => AspectScore.fromJson(a))
          .toList(),
      trend: (json['trend'] as List)
          .map((t) => TrendPoint.fromJson(t))
          .toList(),
    );
  }
}

class Goals {
  final List<Goal> goals;
  final int currentStreak;
  final int longestStreak;

  Goals({
    required this.goals,
    required this.currentStreak,
    required this.longestStreak,
  });

  factory Goals.fromJson(Map<String, dynamic> json) {
    return Goals(
      goals: (json['goals'] as List)
          .map((g) => Goal.fromJson(g))
          .toList(),
      currentStreak: json['currentStreak'],
      longestStreak: json['longestStreak'],
    );
  }
}
```

### **Servicio API**

```dart
class TrainingDashboardService {
  final String baseUrl = 'http://your-api-url:5180';
  String? _token;

  Future<ProgressData> getProgressData(String exerciseId, String range) async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/trainingSessions/exercise/$exerciseId/progress')
          .replace(queryParameters: {'range': range}),
      headers: {'Authorization': 'Bearer $_token'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'];
      return ProgressData.fromJson(data);
    } else {
      throw Exception('Error al obtener datos de progreso');
    }
  }

  Future<FormAnalysis> getFormAnalysis(String exerciseId, String range) async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/trainingSessions/exercise/$exerciseId/form-analysis')
          .replace(queryParameters: {'range': range}),
      headers: {'Authorization': 'Bearer $_token'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'];
      return FormAnalysis.fromJson(data);
    } else {
      throw Exception('Error al obtener análisis de técnica');
    }
  }

  Future<Goals> getGoals(String exerciseId) async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/trainingSessions/exercise/$exerciseId/goals'),
      headers: {'Authorization': 'Bearer $_token'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'];
      return Goals.fromJson(data);
    } else {
      throw Exception('Error al obtener objetivos');
    }
  }
}
```

---

## ✅ ARCHIVOS CREADOS/MODIFICADOS

### **DTOs Creados (10 archivos)**
✅ `ProgressDataDto.cs`
✅ `ProgressDataPointDto.cs`
✅ `ProgressSummaryDto.cs`
✅ `BestDayDto.cs`
✅ `FormAnalysisDto.cs`
✅ `AspectScoreDto.cs`
✅ `TrendPointDto.cs`
✅ `GoalsDto.cs`
✅ `GoalDto.cs`

### **Repository Modificado**
✅ `ITrainingSessionRepository.cs` - 3 nuevos métodos
✅ `TrainingSessionRepository.cs` - Implementación de métodos

### **Service Modificado**
✅ `ITrainingSessionService.cs` - 3 nuevos métodos
✅ `TrainingSessionService.cs` - Implementación completa con helpers

### **Controller Modificado**
✅ `TrainingSessionsController.cs` - 3 nuevos endpoints

### **Documentación**
✅ `ProgressDashboard.http` - Archivo de pruebas

---

## 🎯 CARACTERÍSTICAS DESTACADAS

### ✅ **Flexibilidad**
- Soporta vista semanal y mensual
- Detecta automáticamente tipo de ejercicio (reps vs segundos)
- Adapta aspectos de técnica según ejercicio

### ✅ **Análisis Inteligente**
- Calcula mejora comparando períodos
- Determina consistencia automáticamente
- Identifica mejor día/semana

### ✅ **Objetivos Dinámicos**
- Racha actual y más larga
- Progreso hacia metas predefinidas
- Indicador de logros alcanzados

### ✅ **Optimización**
- Queries eficientes con MongoDB
- Agrupación en memoria para performance
- Retorna arrays vacíos (nunca null)

---

## 🧪 CÓMO PROBAR

### **Opción 1: Rider (.http file)**
1. Abre `ProgressDashboard.http`
2. Actualiza `@token` y `@exerciseId`
3. Ejecuta cada request

### **Opción 2: Swagger**
1. Ejecuta la API
2. Abre http://localhost:5180/swagger
3. Autoriza con tu JWT
4. Prueba los 3 endpoints

### **Opción 3: Postman**
```
GET http://localhost:5180/api/trainingSessions/exercise/{exerciseId}/progress?range=week
Authorization: Bearer {token}
```

---

## 🎉 ¡SISTEMA COMPLETO!

**3 endpoints poderosos** para crear un dashboard de progreso profesional con:
- 📊 Gráficas de progreso semanales y mensuales
- 🎯 Análisis de técnica detallado
- 🏆 Sistema de objetivos y rachas
- 📈 Métricas de mejora y consistencia

**¡Listo para integrar con tu app Flutter!** 💪📊🚀

