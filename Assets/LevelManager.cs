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

    public int Seed = 1234;
    public float DEFAULT_GRID_WIDTH = 3.7f;
    public float DEFAULT_GRID_LENGTH = 5;
    public float DEFAULT_CRUISE_SPEED = 26.8224f; // 60 mph

    private string currentSceneName = null;
    private System.Random random;

    protected override void Awake() {
        base.Awake();
        random = new System.Random(Seed);
    }

    public void LoadBoTestLevel() {
        currentSceneName = "BoTestLevel";
        SceneManager.LoadScene(currentSceneName, LoadSceneMode.Additive);
        SpawnCars();
    }

    public void LoadLevel(int levelIndex) {
        if (levelIndex < 0 || levelIndex >= Levels.Count) {
            Debug.LogError($"[LevelManager] Error loading level: Data for {levelIndex} not found!");
            return;
        }

        CurrentLevel = Levels[levelIndex];
        TileManager.Instance.LoadLevel(CurrentLevel);
        SpawnInitialCars(CurrentLevel);
    }

    public void UnloadCurrentLevel() {
        if (currentSceneName == null) {
            return;
        }
        SceneManager.UnloadSceneAsync(currentSceneName);
        currentSceneName = null;

        // TODO Despawn all cars
    }

    void SpawnInitialCars(LevelData level) {
        int numCars = (int)(level.LevelLengthMeters / DEFAULT_GRID_LENGTH);
        List<float> laneDirections = new List<float>();
        for (int i = 0; i < level.OncomingTrafficLanes; i++) {
            laneDirections.Add(-1);
        }
        for (int i = 0; i < level.FollowingTrafficLanes; i++) {
            laneDirections.Add(1);
        }

        float laneXPositionOffset = (laneDirections.Count - 1) * level.LaneSize / 2;
        for (int i = 0; i < laneDirections.Count; i++) {
            float laneXPosition = i * level.LaneSize - laneXPositionOffset;
            // TODO: More complex spawning logic, for now, just random uniform distribution
            for (int carIdx = 0; carIdx < numCars; carIdx++) {
                float randomValue = (float)random.NextDouble();
                if (randomValue <= 0.1f) {
                    float laneYPosition = carIdx * DEFAULT_GRID_LENGTH;
                    float speed = DEFAULT_CRUISE_SPEED + Random.Range(-4.4704f, 4.4704f); // +/- 10mph
                    CarManager.Instance.SpawnCar(new Vector2(laneXPosition, laneYPosition), i, speed * laneDirections[i]);
                }
            }
        }

        // Start the player in the first following traffic lane
    }

    void SpawnCars() {
        // Test level:
        // 3 lanes
        // Spawn 6 cars
        // Max visible distance: maybe 500m                                                                                                                                                                                                                                                                                                                                          
        int levelLengthMeters = 1000;
        int numCars = (int)(levelLengthMeters / DEFAULT_GRID_LENGTH);
        float[] laneDirections = { -1, -1, 1, 1 };

        float laneXPositionOffset = (laneDirections.Length - 1) * DEFAULT_GRID_WIDTH / 2;
        for (int i = 0; i < laneDirections.Length; i++) {
            float laneXPosition = i * DEFAULT_GRID_WIDTH - laneXPositionOffset;
            // TODO: More complex spawning logic, for now, just random uniform distribution
            for (int carIdx = 0; carIdx < numCars; carIdx++) {
                float randomValue = (float)random.NextDouble();
                if (randomValue <= 0.1f) {
                    float laneYPosition = carIdx * DEFAULT_GRID_LENGTH;
                    float speed = DEFAULT_CRUISE_SPEED + Random.Range(-4.4704f, 4.4704f); // +/- 10mph
                    CarManager.Instance.SpawnCar(new Vector2(laneXPosition, laneYPosition), i, speed * laneDirections[i]);
                }
            }
        }
    }

}
