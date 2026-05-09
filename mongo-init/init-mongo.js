db = db.getSiblingDB('FitTrackerDB');

// Solo inserta si la colección está vacía
if (db.Exercises.countDocuments() === 0) {
  db.Exercises.insertMany([
    {
      _id: ObjectId("6915d24f504d0bd2afc9fef4"),
      name: "Push-ups",
      shortDescription: "Fortalece pecho, hombros y tríceps",
      fullDescription: "Las flexiones de pecho son un ejercicio fundamental que fortalece el pecho, hombros, tríceps y core. Mantén el cuerpo recto y controla el movimiento.",
      difficulty: "Intermedio",
      muscleGroup: "Tren Superior",
      minTime: 5,
      maxTime: 10,
      steps: [
        "Colócate en posición de plancha con las manos ligeramente más anchas que los hombros",
        "Mantén el cuerpo recto desde la cabeza hasta los talones",
        "Baja el pecho hacia el suelo controladamente",
        "Empuja hacia arriba hasta la posición inicial"
      ],
      tips: [
        "Mantén los codos cerca del cuerpo y no arquees la espalda"
      ],
      imageUrl: "https://images.unsplash.com/photo-1731341400836-baaa5535b8d5?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      shortImageUrl: "https://images.unsplash.com/photo-1731341400836-baaa5535b8d5?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
    },
    {
      _id: ObjectId("6915f39c504d0bd2afc9fef5"),
      name: "Sentadillas",
      shortDescription: "Desarrolla piernas y glúteos",
      fullDescription: "Las sentadillas son el ejercicio rey para el tren inferior. Trabajan cuádriceps, glúteos, isquiotibiales y core, mejorando la fuerza funcional.",
      difficulty: "Principiante",
      muscleGroup: "Tren Inferior",
      minTime: 8,
      maxTime: 12,
      steps: [
        "Párate con los pies separados al ancho de hombros",
        "Desciende como si fueras a sentarte en una silla",
        "Mantén el peso en los talones y la espalda recta",
        "Vuelve arriba empujando con los talones"
      ],
      tips: [
        "Las rodillas deben seguir la dirección de los pies, no colapsar hacia adentro"
      ],
      imageUrl: "https://cdn.betterme.world/articles/wp-content/uploads/2021/02/100-Squats-a-Day-for-30-Days_-What-Happens-to-Your-Body.png",
      shortImageUrl: "https://cdn.betterme.world/articles/wp-content/uploads/2021/02/100-Squats-a-Day-for-30-Days_-What-Happens-to-Your-Body.png"
    },
    {
      _id: ObjectId("6915f4b1504d0bd2afc9fef6"),
      name: "Plancha",
      shortDescription: "Fortalece el core y estabilidad",
      fullDescription: "La plancha es un ejercicio isométrico excelente para fortalecer el core, hombros y mejorar la estabilidad corporal general.",
      difficulty: "Intermedio",
      muscleGroup: "Core",
      minTime: 3,
      maxTime: 8,
      steps: [
        "Colócate en posición de flexión con antebrazos en el suelo",
        "Mantén el cuerpo recto desde la cabeza hasta los talones",
        "Contrae el abdomen y glúteos",
        "Mantén la posición respirando normalmente"
      ],
      tips: [
        "No dejes que las caderas se hundan o se eleven demasiado"
      ],
      imageUrl: "https://media.gq.com.mx/photos/639d1e3358d663f4a3d6bcb4/16:9/w_2560%2Cc_limit/plancha-506760290.jpg",
      shortImageUrl: "https://media.gq.com.mx/photos/639d1e3358d663f4a3d6bcb4/16:9/w_2560%2Cc_limit/plancha-506760290.jpg"
    }
  ]);

  print("✅ 3 ejercicios insertados en la colección Exercises");
} else {
  print("⚠️  La colección Exercises ya tiene datos, se omite la inserción");
}

