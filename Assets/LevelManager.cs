using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Loads different levels
// Loads parts of levels

// Updates car positions

// Level:
// In terms of the cars spawning, can be thought of as a grid.
// Grid columns are the lanes, and grid y are lengths down the lane.

// Each lane can either have forward traffic, or opposite traffic.
// Assuming car is going 30m/s, if going forward for 60s, it will reach 1800m.
// Generate level up to 2000m.
// Grid will be 6x400


// Terrain tile should be 250m wide (wide enough for camera to see a flat horizon)
// 500m long (long enough for camera to see far off into the distance)

// Level should have:
// List of terrain tiles that connect to each other
// Maybe road if we want curve can have a bezier curve that defines it, for now, let's assume that road is a straight vertical line.
// Level has # of oncoming traffic lanes (could be 0), and a # of following traffic lanes. Given this, it has:
// - A total width
// - A median if oncoming traffic lanes > 0 (double yellow)
// - Dividers between lanes
// The road could be a separate mesh on top of the ground plane.
// If the road follows a curve, we could also generate the mesh such that it follows the curve. 
// In order to shade it properly, it just needs proper UVs.

public class LevelManager : Singleton<LevelManager> {
    public List<LevelData> Levels = new List<LevelData>();
    public LevelData CurrentLevel;

    public float GetEndPosition() {
        return CurrentLevel.GetLevelLengthMeters() + GetLevelOffset();
    }

    public float GetLevelOffset() {
        return TileManager.Instance.GetLevelOffset();
    }

    public float GetTotalDistance() {
        var total = 0f;
        for (int i = 0; i < Levels.Count; i++) {
            total += Levels[i].GetLevelLengthMeters();
        }
        return total;
    }

    public void LoadLevel(int levelIndex) {
        if (levelIndex < 0 || levelIndex >= Levels.Count) {
            Debug.LogError($"[LevelManager] Error loading level: Data for {levelIndex} not found!");
            return;
        }

        CurrentLevel = Levels[levelIndex];
        TileManager.Instance.LoadLevel(CurrentLevel);
        CarManager.Instance.LoadLevel(CurrentLevel);

        // In the first level, the player should be moved to the first traffic lane.
        // Start the player in the first following traffic lane
        // PlayerManager.Instance.
    }

}
