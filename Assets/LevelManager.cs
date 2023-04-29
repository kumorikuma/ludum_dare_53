using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Loads different levels
// Loads parts of levels

// Updates car positions

public class LevelManager : Singleton<LevelManager> {
    void Start() {
        // Test level:
        // 3 lanes
        // Spawn 6 cars
        float[] lanePositions = { -3.7f, 0, 3.7f };
        for (int i = 0; i < lanePositions.Length; i++) {
            // Spawn cars spaced out some distance apart
            for (int carIdx = 0; carIdx < 5; carIdx++) {
                CarManager.Instance.SpawnCar(new Vector2(lanePositions[i], carIdx * 10.0f), 0);
            }
        }
    }
}
