# 👤 User Profile & Stats API - Documentación Completa

## ✅ IMPLEMENTACIÓN COMPLETA

Se han implementado exitosamente **4 endpoints** para el sistema de perfil de usuario con estadísticas y achievements/logros.

---

## 🚀 ENDPOINTS IMPLEMENTADOS

### **1. GET /api/users/profile**
**Descripción:** Obtiene información completa del perfil del usuario autenticado

**Headers:**
```
Authorization: Bearer {access_token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": "6915a0637c585f65f4556f5b",
    "name": "Giovanni",
    "lastName": "Ramos",
    "email": "giovanni@example.com",
    "weight": 75.5,
    "height": 180,
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

---

### **2. GET /api/users/profile/stats**
**Descripción:** Obtiene estadísticas del usuario para mostrar en el perfil

**Headers:**
```
Authorization: Bearer {access_token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "activeDays": 12,
    "masteredExercises": 4,
    "completedGoals": 8,
    "totalWorkouts": 45,
    "totalReps": 1250,
    "totalSeconds": 3600,
    "currentStreak": 5,
    "bestStreak": 7
  }
}
```

**Cálculos:**
- `activeDays`: Días únicos con al menos 1 sesión completada
- `masteredExercises`: Ejercicios con al menos 10 sesiones completadas
- `completedGoals`: Total de achievements/logros alcanzados
- `totalWorkouts`: Total de sesiones completadas
- `totalReps`: Total de repeticiones (solo push-ups y squats)
- `totalSeconds`: Total de segundos (solo plank)
- `currentStreak`: Días consecutivos actuales con actividad
- `bestStreak`: Mejor racha de días consecutivos (histórico)

---

### **3. GET /api/users/profile/achievements**
**Descripción:** Obtiene logros/achievements del usuario

**Headers:**
```
Authorization: Bearer {access_token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": "507f1f77bcf86cd799439011",
      "name": "Primera semana",
      "description": "Completaste tu primera semana de entrenamiento",
      "icon": "🎯",
      "earned": true,
      "earnedAt": "2024-01-07T10:30:00Z",
      "category": "milestone"
    },
    {
      "id": "507f1f77bcf86cd799439012",
      "name": "100 Push-ups",
      "description": "Alcanzaste 100 push-ups en total",
      "icon": "💪",
      "earned": true,
      "earnedAt": "2024-01-10T15:20:00Z",
      "category": "volume"
    },
    {
      "id": "507f1f77bcf86cd799439013",
      "name": "Constancia 7 días",
      "description": "Entrenaste 7 días seguidos",
      "icon": "🔥",
      "earned": true,
      "earnedAt": "2024-01-14T09:00:00Z",
      "category": "streak"
    },
    {
      "id": "507f1f77bcf86cd799439014",
      "name": "Mejorador",
      "description": "Supera tu mejor marca en 5 ejercicios diferentes",
      "icon": "📈",
      "earned": false,
      "earnedAt": null,
      "category": "improvement"
    }
  ]
}
```

**Categorías de Achievements:**
- `milestone`: Hitos importantes (primera semana, primera sesión, etc.)
- `volume`: Volumen acumulado (100 push-ups, 500 reps, etc.)
- `streak`: Rachas consecutivas (7 días, 30 días, etc.)
- `improvement`: Mejoras (superar marcas en ejercicios)
- `special`: Logros especiales (madrugador, etc.)

---

### **4. PUT /api/users/profile**
**Descripción:** Actualiza información del perfil del usuario

**Headers:**
```
Authorization: Bearer {access_token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Giovanni",
  "lastName": "Ramos",
  "weight": 75.5,
  "height": 180
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Perfil actualizado correctamente",
  "data": {
    "id": "6915a0637c585f65f4556f5b",
    "name": "Giovanni",
    "lastName": "Ramos",
    "email": "giovanni@example.com",
    "weight": 75.5,
    "height": 180,
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

---

## 📊 ACHIEVEMENTS PREDEFINIDOS

Los siguientes achievements están seed-eados en la base de datos:

| Nombre | Icon | Categoría | Requisito | Descripción |
|--------|------|-----------|-----------|-------------|
| Primera sesión | 🌟 | milestone | 1 sesión | Completaste tu primera sesión |
| Primera semana | 🎯 | milestone | 7 días | Completaste tu primera semana |
| Constante | 📅 | milestone | 30 días | Entrena 30 días diferentes |
| 100 Push-ups | 💪 | volume | 100 reps | Alcanzaste 100 push-ups en total |
| 100 Plank segundos | ⏱️ | volume | 100 seg | Alcanzaste 100 segundos de plancha |
| 500 Repeticiones | 💯 | volume | 500 reps | Alcanzaste 500 repeticiones |
| Constancia 7 días | 🔥 | streak | 7 días | Entrenaste 7 días seguidos |
| Racha de 30 días | 🔥🔥 | streak | 30 días | Entrenaste 30 días seguidos |
| Mejorador | 📈 | improvement | 5 ejercicios | Supera tu mejor marca en 5 ejercicios |
| Madrugador | 🌅 | special | 5 veces | Entrena antes de las 7 AM en 5 ocasiones |

---

## 🎯 LÓGICA DE DESBLOQUEO AUTOMÁTICO

Los achievements se verifican y desbloquean automáticamente después de cada sesión de entrenamiento mediante el método `CheckAndUnlockAchievementsAsync()`.

### **Milestone Achievements**
```csharp
// Primera sesión
sessions.Count >= 1

// Primera semana
activeDays >= 7

// Constante (30 días)
activeDays >= 30
```

### **Volume Achievements**
```csharp
// 100 Push-ups
totalPushups >= 100

// 100 Plank segundos
totalSeconds >= 100

// 500 Repeticiones
totalReps >= 500
```

### **Streak Achievements**
```csharp
// Constancia 7 días
bestStreak >= 7

// Racha de 30 días
bestStreak >= 30
```

### **Improvement Achievements**
```csharp
// Mejorador: 5 ejercicios con mejora
// Compara primera sesión vs última sesión
// Cuenta ejercicios donde última > primera
exercisesWithImprovement >= 5
```

### **Special Achievements**
```csharp
// Madrugador
sessionsBefo re7AM >= 5
```

---

## 📱 INTEGRACIÓN CON FLUTTER

### **Modelos Dart**

```dart
class UserProfile {
  final String id;
  final String name;
  final String lastName;
  final String email;
  final double weight;
  final int height;
  final DateTime dateOfBirth;
  final DateTime createdAt;

  UserProfile({
    required this.id,
    required this.name,
    required this.lastName,
    required this.email,
    required this.weight,
    required this.height,
    required this.dateOfBirth,
    required this.createdAt,
  });

  factory UserProfile.fromJson(Map<String, dynamic> json) {
    return UserProfile(
      id: json['id'],
      name: json['name'],
      lastName: json['lastName'],
      email: json['email'],
      weight: json['weight'].toDouble(),
      height: json['height'],
      dateOfBirth: DateTime.parse(json['dateOfBirth']),
      createdAt: DateTime.parse(json['createdAt']),
    );
  }
}

class UserStats {
  final int activeDays;
  final int masteredExercises;
  final int completedGoals;
  final int totalWorkouts;
  final int totalReps;
  final int totalSeconds;
  final int currentStreak;
  final int bestStreak;

  UserStats({
    required this.activeDays,
    required this.masteredExercises,
    required this.completedGoals,
    required this.totalWorkouts,
    required this.totalReps,
    required this.totalSeconds,
    required this.currentStreak,
    required this.bestStreak,
  });

  factory UserStats.fromJson(Map<String, dynamic> json) {
    return UserStats(
      activeDays: json['activeDays'],
      masteredExercises: json['masteredExercises'],
      completedGoals: json['completedGoals'],
      totalWorkouts: json['totalWorkouts'],
      totalReps: json['totalReps'],
      totalSeconds: json['totalSeconds'],
      currentStreak: json['currentStreak'],
      bestStreak: json['bestStreak'],
    );
  }
}

class Achievement {
  final String id;
  final String name;
  final String description;
  final String icon;
  final bool earned;
  final DateTime? earnedAt;
  final String category;

  Achievement({
    required this.id,
    required this.name,
    required this.description,
    required this.icon,
    required this.earned,
    this.earnedAt,
    required this.category,
  });

  factory Achievement.fromJson(Map<String, dynamic> json) {
    return Achievement(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      icon: json['icon'],
      earned: json['earned'],
      earnedAt: json['earnedAt'] != null 
          ? DateTime.parse(json['earnedAt'])
          : null,
      category: json['category'],
    );
  }
}
```

### **Servicio API**

```dart
class UserProfileService {
  final String baseUrl = 'http://your-api:5180';
  String? _token;

  Future<UserProfile> getProfile() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/users/profile'),
      headers: {'Authorization': 'Bearer $_token'},
    );

    if (response.statusCode == 200) {
      return UserProfile.fromJson(jsonDecode(response.body)['data']);
    }
    throw Exception('Error al obtener perfil');
  }

  Future<UserStats> getStats() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/users/profile/stats'),
      headers: {'Authorization': 'Bearer $_token'},
    );

    if (response.statusCode == 200) {
      return UserStats.fromJson(jsonDecode(response.body)['data']);
    }
    throw Exception('Error al obtener estadísticas');
  }

  Future<List<Achievement>> getAchievements() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/users/profile/achievements'),
      headers: {'Authorization': 'Bearer $_token'},
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'] as List;
      return data.map((a) => Achievement.fromJson(a)).toList();
    }
    throw Exception('Error al obtener logros');
  }

  Future<UserProfile> updateProfile(String name, String lastName, double weight, int height) async {
    final response = await http.put(
      Uri.parse('$baseUrl/api/users/profile'),
      headers: {
        'Authorization': 'Bearer $_token',
        'Content-Type': 'application/json',
      },
      body: jsonEncode({
        'name': name,
        'lastName': lastName,
        'weight': weight,
        'height': height,
      }),
    );

    if (response.statusCode == 200) {
      return UserProfile.fromJson(jsonDecode(response.body)['data']);
    }
    throw Exception('Error al actualizar perfil');
  }
}
```

---

## 🎨 PANTALLA DE EJEMPLO

```
┌─────────────────────────────────┐
│  Perfil                         │
├─────────────────────────────────┤
│  [📸 Foto]                      │
│  Giovanni Ramos                 │
│  giovanni@example.com           │
│                                  │
│  📊 Estadísticas                │
│  ┌───────────────────────────┐  │
│  │ 12 Días Activos           │  │
│  │ 4 Ejercicios Dominados    │  │
│  │ 45 Entrenamientos         │  │
│  │ 🔥 Racha: 5 (mejor: 7)   │  │
│  └───────────────────────────┘  │
│                                  │
│  🏆 Logros (8 desbloqueados)    │
│  ┌───────────────────────────┐  │
│  │ 🎯 Primera semana         │  │
│  │ ✅ Desbloqueado           │  │
│  ├───────────────────────────┤  │
│  │ 💪 100 Push-ups           │  │
│  │ ✅ Desbloqueado           │  │
│  ├───────────────────────────┤  │
│  │ 📈 Mejorador              │  │
│  │ 🔒 Bloqueado              │  │
│  └───────────────────────────┘  │
└─────────────────────────────────┘
```

---

## ✅ ARCHIVOS CREADOS/MODIFICADOS

### **Modelos (2 nuevos)**
✅ `Models/Achievements/Achievement.cs`
✅ `Models/Achievements/UserAchievement.cs`

### **DTOs (4 nuevos)**
✅ `DTOs/UserProfile/UserProfileDto.cs`
✅ `DTOs/UserProfile/UserStatsDto.cs`
✅ `DTOs/UserProfile/AchievementDto.cs`
✅ `DTOs/UserProfile/UpdateProfileRequest.cs`

### **Repositories (2 nuevos)**
✅ `Repositories/Achievements/IAchievementRepository.cs`
✅ `Repositories/Achievements/AchievementRepository.cs`

### **Services (2 nuevos)**
✅ `Services/UserProfile/IUserProfileService.cs`
✅ `Services/UserProfile/UserProfileService.cs`

### **Controllers (1 nuevo)**
✅ `Controllers/UserProfileController.cs`

### **Modificados**
✅ `Data/MongoDbContext.cs` - Agregadas colecciones
✅ `Models/UserInfo/UserProfile.cs` - Agregados campos
✅ `Program.cs` - Registrados servicios

### **Documentación**
✅ `UserProfile.http` - Archivo de pruebas

---

## 🎉 ¡SISTEMA COMPLETO!

**4 endpoints funcionales** para perfil de usuario con:
- ✅ Información de perfil completa
- ✅ Estadísticas detalladas
- ✅ Sistema de achievements/logros
- ✅ Actualización de perfil
- ✅ Desbloqueo automático de logros
- ✅ 10 achievements predefinidos

**¡Listo para crear una pantalla de perfil profesional en tu app móvil!** 👤💪🏆

