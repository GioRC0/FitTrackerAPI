# 🏠 Home Dashboard API - Documentación Completa

## ✅ IMPLEMENTACIÓN COMPLETA

Se ha implementado exitosamente el endpoint del **Home Dashboard** que muestra un resumen general de la actividad del usuario con estadísticas agregadas y sesiones recientes.

---

## 🚀 ENDPOINT IMPLEMENTADO

### **GET /api/dashboard/home**

**Descripción:** Obtiene el resumen completo del dashboard de inicio con estadísticas semanales, mejor racha, racha actual y actividad reciente.

**Autenticación:** Requiere JWT Bearer token

**Headers:**
```
Authorization: Bearer {token}
```

---

## 📤 RESPONSE

### **200 OK - Éxito**

```json
{
  "success": true,
  "data": {
    "user": {
      "name": "Giovanni"
    },
    "stats": {
      "weeklyWorkouts": 4,
      "weeklyTotalReps": 340,
      "weeklyTotalSeconds": 450,
      "bestStreak": 7,
      "currentStreak": 3
    },
    "lastExercise": {
      "id": "507f1f77bcf86cd799439011",
      "exerciseId": "6915f4b1504d0bd2afc9fef6",
      "exerciseName": "Push-ups",
      "exerciseType": "pushup",
      "date": "2024-11-18T10:30:00Z",
      "reps": 25,
      "seconds": 0,
      "improvement": "+3 reps",
      "duration": "5 min",
      "imageUrl": "https://example.com/pushup-short.jpg"
    },
    "recentActivity": [
      {
        "id": "507f1f77bcf86cd799439011",
        "exerciseId": "6915f4b1504d0bd2afc9fef6",
        "exerciseName": "Push-ups",
        "exerciseType": "pushup",
        "date": "2024-11-18T10:30:00Z",
        "reps": 25,
        "seconds": 0,
        "improvement": "+3 reps",
        "duration": "5 min"
      },
      {
        "id": "507f1f77bcf86cd799439012",
        "exerciseId": "6915f4b1504d0bd2afc9fef7",
        "exerciseName": "Sentadillas",
        "exerciseType": "squat",
        "date": "2024-11-17T15:20:00Z",
        "reps": 30,
        "seconds": 0,
        "improvement": "+5 reps",
        "duration": "8 min"
      },
      {
        "id": "507f1f77bcf86cd799439013",
        "exerciseId": "6915f4b1504d0bd2afc9fef8",
        "exerciseName": "Plancha",
        "exerciseType": "plank",
        "date": "2024-11-16T09:00:00Z",
        "reps": 0,
        "seconds": 45,
        "improvement": "+10 seg",
        "duration": "3 min"
      }
    ]
  }
}
```

### **401 Unauthorized - No autenticado**

```json
{
  "message": "Usuario no autenticado"
}
```

### **500 Internal Server Error**

```json
{
  "message": "Error al obtener dashboard",
  "error": "Detalles del error..."
}
```

---

## 📊 ESTRUCTURA DE DATOS

### **HomeDashboardDto**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `user` | `UserSummaryDto` | Información básica del usuario |
| `stats` | `HomeStatsDto` | Estadísticas semanales y rachas |
| `lastExercise` | `RecentExerciseDto?` | Última sesión realizada (con imagen) |
| `recentActivity` | `List<RecentExerciseDto>` | Últimas 3 sesiones (sin imagen) |

### **UserSummaryDto**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `name` | `string` | Nombre del usuario (firstName de User.Profile) |

### **HomeStatsDto**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `weeklyWorkouts` | `int` | Total de sesiones en últimos 7 días |
| `weeklyTotalReps` | `int` | Total de repeticiones (solo push-ups y squats) |
| `weeklyTotalSeconds` | `int` | Total de segundos (solo plank) |
| `bestStreak` | `int` | Mejor racha histórica de días consecutivos |
| `currentStreak` | `int` | Racha actual de días consecutivos |

### **RecentExerciseDto**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | `string` | ID de la sesión |
| `exerciseId` | `string` | ID del ejercicio |
| `exerciseName` | `string` | Nombre del ejercicio |
| `exerciseType` | `string` | Tipo: "pushup", "squat", "plank" |
| `date` | `DateTime` | Fecha y hora de inicio |
| `reps` | `int` | Total de repeticiones (0 para plank) |
| `seconds` | `int` | Total de segundos (0 para push-ups/squats) |
| `improvement` | `string` | Mejora vs sesión anterior |
| `duration` | `string` | Duración formateada |
| `imageUrl` | `string?` | URL de imagen (solo en lastExercise) |

---

## 🎯 LÓGICA DE NEGOCIO

### **1. Estadísticas Semanales (Stats)**

#### **Weekly Workouts**
```csharp
weekStart = hoy - 6 días // Últimos 7 días incluido hoy
weeklyWorkouts = Count(sesiones donde startTime >= weekStart)
```

**Ejemplo:**
- Hoy: 18 de noviembre
- Rango: 12-18 de noviembre (7 días)
- Sesiones: 12/Nov (1), 14/Nov (1), 16/Nov (1), 18/Nov (1)
- **weeklyWorkouts = 4**

#### **Weekly Total Reps**
```csharp
weeklyTotalReps = Sum(totalReps de sesiones donde exerciseType != "plank")
```

**Ejemplo:**
- Push-ups (12/Nov): 20 reps
- Squats (14/Nov): 30 reps
- Push-ups (16/Nov): 25 reps
- Push-ups (18/Nov): 25 reps
- **weeklyTotalReps = 100**

#### **Weekly Total Seconds**
```csharp
weeklyTotalSeconds = Sum(totalSeconds de sesiones donde exerciseType == "plank")
```

**Ejemplo:**
- Plank (13/Nov): 45 seg
- Plank (15/Nov): 60 seg
- Plank (17/Nov): 50 seg
- **weeklyTotalSeconds = 155**

---

### **2. Rachas (Streaks)**

#### **Best Streak (Mejor Racha Histórica)**

**Algoritmo:**
```csharp
1. Obtener todas las fechas únicas de sesiones (sin hora)
2. Ordenar ascendente
3. Iterar comparando fechas consecutivas:
   - Si diferencia = 1 día → incrementar racha
   - Si diferencia > 1 día → reiniciar racha a 1
4. Retornar la racha máxima encontrada
```

**Ejemplo:**
```
Fechas: [1/Nov, 2/Nov, 3/Nov, 5/Nov, 6/Nov, 7/Nov, 8/Nov, 10/Nov]

Racha 1: 1-2-3 (3 días)
Racha 2: 5-6-7-8 (4 días) ← Máxima
Racha 3: 10 (1 día)

bestStreak = 4
```

#### **Current Streak (Racha Actual)**

**Algoritmo:**
```csharp
1. Obtener fechas únicas ordenadas descendente
2. Si no hay sesión hoy NI ayer → return 0
3. Empezar desde hoy (o ayer si hoy no hay)
4. Contar hacia atrás mientras haya días consecutivos
5. Return conteo
```

**Ejemplo 1 - Racha activa:**
```
Hoy: 18/Nov
Fechas: [18/Nov, 17/Nov, 16/Nov, 14/Nov, 13/Nov]

18 → 17 → 16 (consecutivos)
16 → 14 (salto, se detiene)

currentStreak = 3
```

**Ejemplo 2 - Racha rota:**
```
Hoy: 18/Nov
Fechas: [16/Nov, 15/Nov, 14/Nov]

Última sesión: 16/Nov (hace 2 días)
currentStreak = 0 (racha rota)
```

**Ejemplo 3 - Racha válida sin sesión hoy:**
```
Hoy: 18/Nov
Fechas: [17/Nov, 16/Nov, 15/Nov]

Última sesión: 17/Nov (ayer)
17 → 16 → 15 (consecutivos)

currentStreak = 3 (racha sigue activa)
```

---

### **3. Last Exercise (Última Sesión)**

**Lógica:**
```csharp
1. Ordenar todas las sesiones por startTime descendente
2. Tomar la primera (más reciente)
3. Obtener imageUrl de Exercise.shortImageUrl usando exerciseId
4. Buscar sesión anterior del mismo ejercicio
5. Calcular improvement comparando con anterior
6. Formatear duration
```

**Características especiales:**
- ✅ **Incluye imageUrl** (único lugar donde se incluye)
- ✅ Muestra mejora vs sesión anterior del mismo ejercicio
- ✅ Duration formateado

---

### **4. Recent Activity (Actividad Reciente)**

**Lógica:**
```csharp
1. Ordenar todas las sesiones por startTime descendente
2. Tomar las primeras 3
3. Para cada una:
   - Buscar sesión anterior del mismo ejercicio
   - Calcular improvement
   - Formatear duration
4. NO incluir imageUrl
```

**Diferencia con LastExercise:**
- ❌ **NO incluye imageUrl**
- ✅ Lista de 3 sesiones
- ✅ Misma lógica de improvement y duration

---

### **5. Improvement (Mejora)**

**Para Push-ups y Squats:**
```csharp
diff = sesionActual.totalReps - sesionAnterior.totalReps

Si diff > 0: "+X reps"
Si diff < 0: "-X reps" 
Si diff = 0: "+0 reps"
Si no hay anterior: "Primera sesión"
```

**Ejemplos:**
- Anterior: 20 reps, Actual: 25 reps → **"+5 reps"**
- Anterior: 30 reps, Actual: 25 reps → **"-5 reps"**
- Primera sesión → **"Primera sesión"**

**Para Plank:**
```csharp
diff = sesionActual.totalSeconds - sesionAnterior.totalSeconds

Si diff > 0: "+X seg"
Si diff < 0: "-X seg"
Si diff = 0: "+0 seg"
Si no hay anterior: "Primera sesión"
```

**Ejemplos:**
- Anterior: 45 seg, Actual: 60 seg → **"+15 seg"**
- Anterior: 60 seg, Actual: 50 seg → **"-10 seg"**

---

### **6. Duration (Duración)**

**Formato:**
```csharp
Si durationSeconds < 60:
    return "{segundos} seg"
Else:
    minutos = Round(durationSeconds / 60)
    return "{minutos} min"
```

**Ejemplos:**
- 45 seg → **"45 seg"**
- 60 seg → **"1 min"**
- 90 seg → **"2 min"** (redondeado)
- 330 seg → **"6 min"** (5.5 redondeado)

---

## 📱 INTEGRACIÓN CON FLUTTER

### **Modelo Dart**

```dart
class HomeDashboard {
  final UserSummary user;
  final HomeStats stats;
  final RecentExercise? lastExercise;
  final List<RecentExercise> recentActivity;

  HomeDashboard({
    required this.user,
    required this.stats,
    this.lastExercise,
    required this.recentActivity,
  });

  factory HomeDashboard.fromJson(Map<String, dynamic> json) {
    return HomeDashboard(
      user: UserSummary.fromJson(json['user']),
      stats: HomeStats.fromJson(json['stats']),
      lastExercise: json['lastExercise'] != null 
          ? RecentExercise.fromJson(json['lastExercise'])
          : null,
      recentActivity: (json['recentActivity'] as List)
          .map((e) => RecentExercise.fromJson(e))
          .toList(),
    );
  }
}

class UserSummary {
  final String name;

  UserSummary({required this.name});

  factory UserSummary.fromJson(Map<String, dynamic> json) {
    return UserSummary(name: json['name']);
  }
}

class HomeStats {
  final int weeklyWorkouts;
  final int weeklyTotalReps;
  final int weeklyTotalSeconds;
  final int bestStreak;
  final int currentStreak;

  HomeStats({
    required this.weeklyWorkouts,
    required this.weeklyTotalReps,
    required this.weeklyTotalSeconds,
    required this.bestStreak,
    required this.currentStreak,
  });

  factory HomeStats.fromJson(Map<String, dynamic> json) {
    return HomeStats(
      weeklyWorkouts: json['weeklyWorkouts'],
      weeklyTotalReps: json['weeklyTotalReps'],
      weeklyTotalSeconds: json['weeklyTotalSeconds'],
      bestStreak: json['bestStreak'],
      currentStreak: json['currentStreak'],
    );
  }
}

class RecentExercise {
  final String id;
  final String exerciseId;
  final String exerciseName;
  final String exerciseType;
  final DateTime date;
  final int reps;
  final int seconds;
  final String improvement;
  final String duration;
  final String? imageUrl;

  RecentExercise({
    required this.id,
    required this.exerciseId,
    required this.exerciseName,
    required this.exerciseType,
    required this.date,
    required this.reps,
    required this.seconds,
    required this.improvement,
    required this.duration,
    this.imageUrl,
  });

  factory RecentExercise.fromJson(Map<String, dynamic> json) {
    return RecentExercise(
      id: json['id'],
      exerciseId: json['exerciseId'],
      exerciseName: json['exerciseName'],
      exerciseType: json['exerciseType'],
      date: DateTime.parse(json['date']),
      reps: json['reps'],
      seconds: json['seconds'],
      improvement: json['improvement'],
      duration: json['duration'],
      imageUrl: json['imageUrl'],
    );
  }
}
```

### **Servicio API**

```dart
class DashboardService {
  final String baseUrl = 'http://your-api-url:5180';
  String? _token;

  Future<HomeDashboard> getHomeDashboard() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/dashboard/home'),
      headers: {'Authorization': 'Bearer $_token'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'];
      return HomeDashboard.fromJson(data);
    } else if (response.statusCode == 401) {
      throw Exception('Token expirado');
    } else {
      throw Exception('Error al obtener dashboard');
    }
  }
}
```

### **Widget de Home Screen**

```dart
class HomeScreen extends StatefulWidget {
  @override
  _HomeScreenState createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final DashboardService _api = DashboardService();
  HomeDashboard? _dashboard;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadDashboard();
  }

  Future<void> _loadDashboard() async {
    setState(() => _isLoading = true);
    try {
      final dashboard = await _api.getHomeDashboard();
      setState(() {
        _dashboard = dashboard;
        _isLoading = false;
      });
    } catch (e) {
      print('Error: $e');
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return Center(child: CircularProgressIndicator());
    }

    if (_dashboard == null) {
      return Center(child: Text('Error al cargar datos'));
    }

    return RefreshIndicator(
      onRefresh: _loadDashboard,
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header con nombre
            Padding(
              padding: EdgeInsets.all(16),
              child: Text(
                'Hola, ${_dashboard!.user.name}',
                style: Theme.of(context).textTheme.headline4,
              ),
            ),

            // Estadísticas semanales
            WeeklyStatsCard(stats: _dashboard!.stats),

            // Última sesión
            if (_dashboard!.lastExercise != null)
              LastExerciseCard(exercise: _dashboard!.lastExercise!),

            // Actividad reciente
            RecentActivityList(activities: _dashboard!.recentActivity),
          ],
        ),
      ),
    );
  }
}
```

---

## 🎨 CASOS DE USO EN LA APP

### **1. Pantalla de Inicio (Home)**

```
┌─────────────────────────────────────┐
│  Hola, Giovanni                     │
├─────────────────────────────────────┤
│  📊 Esta Semana                     │
│  ┌─────────────────────────────────┐ │
│  │ 🏋️ 4 entrenamientos             │ │
│  │ 💪 340 reps                      │ │
│  │ ⏱️ 450 seg                       │ │
│  │ 🔥 Racha: 3 días (mejor: 7)     │ │
│  └─────────────────────────────────┘ │
│                                       │
│  🎯 Última Sesión                    │
│  ┌─────────────────────────────────┐ │
│  │ [Imagen]                         │ │
│  │ Push-ups                         │ │
│  │ 25 reps • 5 min                  │ │
│  │ ✅ +3 reps                       │ │
│  │ Hoy 10:30 AM                     │ │
│  └─────────────────────────────────┘ │
│                                       │
│  📝 Actividad Reciente               │
│  ┌─────────────────────────────────┐ │
│  │ Push-ups                         │ │
│  │ 25 reps • 5 min • +3 reps       │ │
│  ├─────────────────────────────────┤ │
│  │ Sentadillas                      │ │
│  │ 30 reps • 8 min • +5 reps       │ │
│  ├─────────────────────────────────┤ │
│  │ Plancha                          │ │
│  │ 45 seg • 3 min • +10 seg        │ │
│  └─────────────────────────────────┘ │
└─────────────────────────────────────┘
```

---

## ✅ VALIDACIONES Y EDGE CASES

### **Sin Sesiones**
```json
{
  "user": { "name": "Giovanni" },
  "stats": {
    "weeklyWorkouts": 0,
    "weeklyTotalReps": 0,
    "weeklyTotalSeconds": 0,
    "bestStreak": 0,
    "currentStreak": 0
  },
  "lastExercise": null,
  "recentActivity": []
}
```

### **Primera Sesión**
```json
{
  "lastExercise": {
    "improvement": "Primera sesión",
    ...
  }
}
```

### **Racha Rota**
- Si última sesión fue hace 2+ días → `currentStreak = 0`
- Si última sesión fue ayer → racha sigue activa

### **Mix de Ejercicios**
- `weeklyTotalReps` solo cuenta push-ups y squats
- `weeklyTotalSeconds` solo cuenta plank

---

## 📁 ARCHIVOS CREADOS/MODIFICADOS

### **DTOs Creados (4 archivos)**
✅ `DTOs/Dashboard/HomeDashboardDto.cs`
✅ `DTOs/Dashboard/UserSummaryDto.cs`
✅ `DTOs/Dashboard/HomeStatsDto.cs`
✅ `DTOs/Dashboard/RecentExerciseDto.cs`

### **Service Creado (2 archivos)**
✅ `Services/Dashboard/IDashboardService.cs`
✅ `Services/Dashboard/DashboardService.cs`

### **Controller Creado (1 archivo)**
✅ `Controllers/DashboardController.cs`

### **Repository Modificado**
✅ `TrainingSessionRepository.cs` - 3 métodos actualizados para soportar exerciseId vacío

### **Program.cs Modificado**
✅ Registrado `IDashboardService` y `DashboardService`

### **Documentación**
✅ `HomeDashboard.http` - Archivo de pruebas

---

## 🎉 ¡IMPLEMENTACIÓN COMPLETA!

**El endpoint del Home Dashboard está completamente funcional con:**
- 📊 **Estadísticas semanales**
- 🔥 **Sistema de rachas (actual y mejor)**
- 🎯 **Última sesión con imagen**
- 📝 **Actividad reciente (3 últimas)**
- ✅ **Cálculo de mejora automático**
- ⏱️ **Formato de duración legible**

**¡Listo para crear una pantalla de inicio profesional en tu app móvil!** 💪🏠🚀

