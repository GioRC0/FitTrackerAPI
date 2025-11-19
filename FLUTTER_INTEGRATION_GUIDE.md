# 💡 Mejores Prácticas - Training Sessions System

## 📱 INTEGRACIÓN CON FLUTTER

### 1. Clase de Modelo en Flutter (Dart)

```dart
// training_session.dart
class TrainingSession {
  final String exerciseId;
  final String exerciseType;
  final String exerciseName;
  final DateTime startTime;
  final DateTime endTime;
  final int durationSeconds;
  final int? totalReps;
  final List<RepData>? repsData;
  final int? totalSeconds;
  final List<SecondData>? secondsData;
  final PerformanceMetrics metrics;

  TrainingSession({
    required this.exerciseId,
    required this.exerciseType,
    required this.exerciseName,
    required this.startTime,
    required this.endTime,
    required this.durationSeconds,
    this.totalReps,
    this.repsData,
    this.totalSeconds,
    this.secondsData,
    required this.metrics,
  });

  Map<String, dynamic> toJson() => {
    'exerciseId': exerciseId,
    'exerciseType': exerciseType,
    'exerciseName': exerciseName,
    'startTime': startTime.toUtc().toIso8601String(),
    'endTime': endTime.toUtc().toIso8601String(),
    'durationSeconds': durationSeconds,
    if (totalReps != null) 'totalReps': totalReps,
    if (repsData != null) 'repsData': repsData!.map((r) => r.toJson()).toList(),
    if (totalSeconds != null) 'totalSeconds': totalSeconds,
    if (secondsData != null) 'secondsData': secondsData!.map((s) => s.toJson()).toList(),
    'metrics': metrics.toJson(),
  };
}

class RepData {
  final int repNumber;
  final String classification;
  final double confidence;
  final Map<String, double> probabilities;
  final DateTime timestamp;

  RepData({
    required this.repNumber,
    required this.classification,
    required this.confidence,
    required this.probabilities,
    required this.timestamp,
  });

  Map<String, dynamic> toJson() => {
    'repNumber': repNumber,
    'classification': classification,
    'confidence': confidence,
    'probabilities': probabilities,
    'timestamp': timestamp.toUtc().toIso8601String(),
  };
}

class PerformanceMetrics {
  final double techniquePercentage;
  final double consistencyScore;
  final double averageConfidence;
  final double? controlScore;
  final double? stabilityScore;
  // ... otros campos

  PerformanceMetrics({
    required this.techniquePercentage,
    required this.consistencyScore,
    required this.averageConfidence,
    this.controlScore,
    this.stabilityScore,
    // ... otros campos
  });

  Map<String, dynamic> toJson() => {
    'techniquePercentage': techniquePercentage,
    'consistencyScore': consistencyScore,
    'averageConfidence': averageConfidence,
    if (controlScore != null) 'controlScore': controlScore,
    if (stabilityScore != null) 'stabilityScore': stabilityScore,
    // ... otros campos
  };
}
```

---

### 2. Servicio API en Flutter

```dart
// training_session_service.dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class TrainingSessionService {
  final String baseUrl = 'http://your-api-url:5180';
  String? _token;

  void setToken(String token) {
    _token = token;
  }

  // Crear sesión de entrenamiento
  Future<Map<String, dynamic>> createSession(TrainingSession session) async {
    if (_token == null) throw Exception('No authenticated');

    final response = await http.post(
      Uri.parse('$baseUrl/api/trainingSessions'),
      headers: {
        'Authorization': 'Bearer $_token',
        'Content-Type': 'application/json',
      },
      body: jsonEncode(session.toJson()),
    );

    if (response.statusCode == 201) {
      return jsonDecode(response.body);
    } else if (response.statusCode == 401) {
      throw Exception('Token expirado, necesita renovar');
    } else {
      throw Exception('Error al crear sesión: ${response.body}');
    }
  }

  // Listar sesiones con cache local
  Future<List<dynamic>> getSessions({
    int page = 1,
    int pageSize = 10,
    String? exerciseId,
    bool useCache = true,
  }) async {
    if (_token == null) throw Exception('No authenticated');

    // Si está offline, intentar cargar del cache
    if (!await _hasConnection() && useCache) {
      return await _loadFromCache();
    }

    var uri = Uri.parse('$baseUrl/api/trainingSessions')
        .replace(queryParameters: {
      'page': page.toString(),
      'pageSize': pageSize.toString(),
      if (exerciseId != null) 'exerciseId': exerciseId,
    });

    final response = await http.get(
      uri,
      headers: {
        'Authorization': 'Bearer $_token',
      },
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'];
      // Guardar en cache para uso offline
      await _saveToCache(data);
      return data;
    } else if (response.statusCode == 401) {
      throw Exception('Token expirado');
    } else {
      throw Exception('Error: ${response.body}');
    }
  }

  // Progreso semanal
  Future<Map<String, dynamic>> getWeeklyProgress(String exerciseId) async {
    if (_token == null) throw Exception('No authenticated');

    final response = await http.get(
      Uri.parse('$baseUrl/api/trainingSessions/weekly-progress/$exerciseId'),
      headers: {
        'Authorization': 'Bearer $_token',
      },
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body)['data'];
    } else {
      throw Exception('Error: ${response.body}');
    }
  }

  // Helper: Verificar conexión
  Future<bool> _hasConnection() async {
    try {
      final result = await http.get(Uri.parse(baseUrl)).timeout(
        Duration(seconds: 5),
      );
      return result.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  // Helper: Cache local
  Future<void> _saveToCache(List<dynamic> data) async {
    // Implementar con shared_preferences o hive
    // SharedPreferences prefs = await SharedPreferences.getInstance();
    // await prefs.setString('cached_sessions', jsonEncode(data));
  }

  Future<List<dynamic>> _loadFromCache() async {
    // Implementar con shared_preferences o hive
    // SharedPreferences prefs = await SharedPreferences.getInstance();
    // String? cached = prefs.getString('cached_sessions');
    // return cached != null ? jsonDecode(cached) : [];
    return [];
  }
}
```

---

### 3. Almacenamiento Offline en Flutter

```dart
// offline_session_manager.dart
import 'package:hive/hive.dart';

class OfflineSessionManager {
  static const String _boxName = 'pending_sessions';
  
  // Guardar sesión pendiente (sin conexión)
  Future<void> savePendingSession(TrainingSession session) async {
    final box = await Hive.openBox(_boxName);
    await box.add(session.toJson());
  }

  // Obtener sesiones pendientes
  Future<List<TrainingSession>> getPendingSessions() async {
    final box = await Hive.openBox(_boxName);
    return box.values
        .map((json) => TrainingSession.fromJson(json))
        .toList();
  }

  // Sincronizar con el servidor
  Future<void> syncPendingSessions(TrainingSessionService api) async {
    final pendingSessions = await getPendingSessions();
    
    for (var session in pendingSessions) {
      try {
        await api.createSession(session);
        // Si éxito, eliminar de pendientes
        await _removePendingSession(session);
        print('Sesión sincronizada: ${session.exerciseName}');
      } catch (e) {
        print('Error sincronizando: $e');
        // Mantener en pendientes para reintentar después
      }
    }
  }

  Future<void> _removePendingSession(TrainingSession session) async {
    final box = await Hive.openBox(_boxName);
    // Implementar lógica para eliminar
  }

  // Limpiar sesiones antiguas (>1 mes)
  Future<void> cleanOldSessions() async {
    final box = await Hive.openBox(_boxName);
    final oneMonthAgo = DateTime.now().subtract(Duration(days: 30));
    
    final keysToDelete = [];
    for (var key in box.keys) {
      final session = TrainingSession.fromJson(box.get(key));
      if (session.endTime.isBefore(oneMonthAgo)) {
        keysToDelete.add(key);
      }
    }
    
    await box.deleteAll(keysToDelete);
    print('Eliminadas ${keysToDelete.length} sesiones antiguas');
  }
}
```

---

### 4. Widget de Entrenamiento en Flutter

```dart
// workout_screen.dart
class WorkoutScreen extends StatefulWidget {
  final Exercise exercise;
  
  @override
  _WorkoutScreenState createState() => _WorkoutScreenState();
}

class _WorkoutScreenState extends State<WorkoutScreen> {
  final TrainingSessionService _api = TrainingSessionService();
  final OfflineSessionManager _offlineManager = OfflineSessionManager();
  
  DateTime? _startTime;
  List<RepData> _repsData = [];
  bool _isRecording = false;

  void startWorkout() {
    setState(() {
      _startTime = DateTime.now();
      _isRecording = true;
      _repsData.clear();
    });
  }

  void onMLClassification(String classification, double confidence, 
                          Map<String, double> probabilities) {
    // Callback desde el módulo ML
    if (_isRecording) {
      _repsData.add(RepData(
        repNumber: _repsData.length + 1,
        classification: classification,
        confidence: confidence,
        probabilities: probabilities,
        timestamp: DateTime.now(),
      ));
      setState(() {});
    }
  }

  Future<void> finishWorkout() async {
    if (!_isRecording || _startTime == null) return;

    setState(() => _isRecording = false);

    // Calcular métricas
    final metrics = _calculateMetrics();
    final endTime = DateTime.now();
    
    // Crear sesión
    final session = TrainingSession(
      exerciseId: widget.exercise.id,
      exerciseType: widget.exercise.type,
      exerciseName: widget.exercise.name,
      startTime: _startTime!,
      endTime: endTime,
      durationSeconds: endTime.difference(_startTime!).inSeconds,
      totalReps: _repsData.length,
      repsData: _repsData,
      metrics: metrics,
    );

    // Intentar subir al servidor
    try {
      await _api.createSession(session);
      _showSuccess('¡Entrenamiento guardado!');
    } catch (e) {
      // Si falla (sin conexión), guardar offline
      await _offlineManager.savePendingSession(session);
      _showInfo('Guardado offline. Se sincronizará cuando haya conexión.');
    }

    // Limpiar sesiones antiguas en segundo plano
    _offlineManager.cleanOldSessions();
  }

  PerformanceMetrics _calculateMetrics() {
    // Calcular % de técnica correcta
    final correctReps = _repsData.where((r) => 
        r.classification.contains('correcto')).length;
    final techniquePercentage = (correctReps / _repsData.length) * 100;

    // Calcular consistencia (variación de confidence)
    final avgConfidence = _repsData.map((r) => r.confidence)
        .reduce((a, b) => a + b) / _repsData.length;
    
    // ... más cálculos

    return PerformanceMetrics(
      techniquePercentage: techniquePercentage,
      consistencyScore: 90.0, // tu cálculo
      averageConfidence: avgConfidence,
      controlScore: 88.0,
      stabilityScore: 90.5,
      repsPerMinute: (_repsData.length / 
          (DateTime.now().difference(_startTime!).inMinutes)),
    );
  }

  void _showSuccess(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message), backgroundColor: Colors.green),
    );
  }

  void _showInfo(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message), backgroundColor: Colors.orange),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          // Camera preview
          // ML visualization
          // Rep counter
          if (_isRecording)
            ElevatedButton(
              onPressed: finishWorkout,
              child: Text('Terminar'),
            )
          else
            ElevatedButton(
              onPressed: startWorkout,
              child: Text('Iniciar'),
            ),
        ],
      ),
    );
  }
}
```

---

## 🔄 MANEJO DE TOKEN EXPIRADO

### En Flutter (Refresh Token)

```dart
class AuthService {
  String? _accessToken;
  String? _refreshToken;

  Future<void> refreshAccessToken() async {
    if (_refreshToken == null) throw Exception('No refresh token');

    final response = await http.post(
      Uri.parse('$baseUrl/api/auth/refresh'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'refreshToken': _refreshToken}),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body)['data'];
      _accessToken = data['accessToken'];
      _refreshToken = data['refreshToken'];
      // Guardar en storage seguro
    } else {
      // Refresh token expirado, debe hacer login nuevamente
      throw Exception('Session expirada, por favor inicia sesión');
    }
  }

  // Interceptor para renovar token automáticamente
  Future<http.Response> secureGet(Uri uri) async {
    var response = await http.get(
      uri,
      headers: {'Authorization': 'Bearer $_accessToken'},
    );

    // Si es 401 (Unauthorized), intentar renovar token
    if (response.statusCode == 401) {
      await refreshAccessToken();
      // Reintentar request
      response = await http.get(
        uri,
        headers: {'Authorization': 'Bearer $_accessToken'},
      );
    }

    return response;
  }
}
```

---

## 📊 ESTADÍSTICAS OFFLINE

### Calcular en Flutter cuando no hay conexión

```dart
class OfflineStatsCalculator {
  // Calcular estadísticas desde sesiones en cache
  Future<WeeklyStats> calculateWeeklyStats(String exerciseId) async {
    final box = await Hive.openBox('cached_sessions');
    final sessions = box.values
        .where((s) => s['exerciseId'] == exerciseId)
        .where((s) => _isThisWeek(DateTime.parse(s['createdAt'])))
        .toList();

    if (sessions.isEmpty) {
      return WeeklyStats.empty();
    }

    return WeeklyStats(
      totalSessions: sessions.length,
      totalReps: sessions.fold(0, (sum, s) => sum + s['totalReps']),
      avgTechniquePercentage: sessions
          .map((s) => s['metrics']['techniquePercentage'])
          .reduce((a, b) => a + b) / sessions.length,
      // ... más cálculos
    );
  }

  bool _isThisWeek(DateTime date) {
    final now = DateTime.now();
    final weekAgo = now.subtract(Duration(days: 7));
    return date.isAfter(weekAgo) && date.isBefore(now);
  }
}
```

---

## ⚡ OPTIMIZACIONES

### 1. Lazy Loading en Lista de Sesiones

```dart
class SessionsListView extends StatefulWidget {
  @override
  _SessionsListViewState createState() => _SessionsListViewState();
}

class _SessionsListViewState extends State<SessionsListView> {
  final ScrollController _scrollController = ScrollController();
  List<dynamic> _sessions = [];
  int _currentPage = 1;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadSessions();
    _scrollController.addListener(_onScroll);
  }

  void _onScroll() {
    if (_scrollController.position.pixels ==
        _scrollController.position.maxScrollExtent) {
      _loadMore();
    }
  }

  Future<void> _loadSessions() async {
    setState(() => _isLoading = true);
    final sessions = await _api.getSessions(page: _currentPage, pageSize: 20);
    setState(() {
      _sessions.addAll(sessions);
      _isLoading = false;
    });
  }

  Future<void> _loadMore() async {
    if (!_isLoading) {
      _currentPage++;
      await _loadSessions();
    }
  }

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      controller: _scrollController,
      itemCount: _sessions.length + 1,
      itemBuilder: (context, index) {
        if (index == _sessions.length) {
          return _isLoading
              ? CircularProgressIndicator()
              : SizedBox.shrink();
        }
        return SessionCard(session: _sessions[index]);
      },
    );
  }
}
```

---

### 2. Compresión de Datos (Reducir repsData/secondsData)

Si tienes muchos datos por segundo en plank (60 segundos = 60 objetos), considera:

```dart
// Enviar solo cada N segundos
List<SecondData> _compressSecondsData(List<SecondData> fullData) {
  // Enviar solo cada 5 segundos + primero y último
  return fullData.where((s) => 
      s.secondNumber == 1 || 
      s.secondNumber == fullData.length ||
      s.secondNumber % 5 == 0
  ).toList();
}
```

O en el backend, crear un endpoint para datos "completos" vs "resumen":

```csharp
[HttpGet("{id}/full")]
public async Task<IActionResult> GetSessionWithFullData(string id) {
    // Incluye todos los repsData/secondsData
}

[HttpGet("{id}")]
public async Task<IActionResult> GetSession(string id) {
    // Solo métricas y contadores
}
```

---

## 🎯 MEJORES PRÁCTICAS

### ✅ DO

1. **Siempre usa UTC** para timestamps
2. **Valida datos ML** antes de enviar (confidence > threshold)
3. **Implementa retry logic** para requests fallidos
4. **Usa cache local** para mejorar UX offline
5. **Limpia datos antiguos** periódicamente
6. **Maneja errores gracefully** con mensajes claros
7. **Implementa loading states** en UI
8. **Usa pagination** para listas grandes
9. **Comprime datos** si son muy grandes
10. **Renueva tokens** automáticamente

### ❌ DON'T

1. ❌ No envíes todos los frames del video
2. ❌ No guardes tokens en plain text
3. ❌ No ignores errores de red
4. ❌ No cargues todas las sesiones de una vez
5. ❌ No olvides validar inputs
6. ❌ No expongas información sensible en logs
7. ❌ No hagas requests sin timeout
8. ❌ No almacenes datos infinitamente en cache
9. ❌ No uses fechas en timezone local
10. ❌ No bloquees UI durante operaciones largas

---

## 🧪 TESTING

### Unit Test para cálculo de métricas

```dart
test('Calculate technique percentage correctly', () {
  final repsData = [
    RepData(classification: 'pushup_correcto', ...),
    RepData(classification: 'pushup_correcto', ...),
    RepData(classification: 'pushup_codos_abiertos', ...),
  ];

  final metrics = MetricsCalculator.calculate(repsData);
  
  expect(metrics.techniquePercentage, closeTo(66.67, 0.01));
});
```

### Integration Test

```dart
testWidgets('Create session flow', (WidgetTester tester) async {
  await tester.pumpWidget(MyApp());
  
  // Start workout
  await tester.tap(find.text('Iniciar'));
  await tester.pump();
  
  // Simulate ML classifications
  // ...
  
  // Finish workout
  await tester.tap(find.text('Terminar'));
  await tester.pumpAndSettle();
  
  // Verify success message
  expect(find.text('¡Entrenamiento guardado!'), findsOneWidget);
});
```

---

¡Todo listo para una integración perfecta con Flutter! 🚀

