using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject {
    public string Name; // Name of the level
    public int LevelLengthMeters; // Level ends after player has traveled this far (in meters)
    public int OncomingTrafficLanes;
    public int FollowingTrafficLanes;
    public float LaneSize;
    public int TileSize;
    public List<GameObject> Tiles;
    public bool ShouldSpawnCars;
    public float CarSpawnsPerSecOncoming = 0.2f; // For each lane
    public float CarSpawnsPerSecFollowing = 1.0f; // For each lane

    public List<int> GetLaneDirections() {
        List<int> laneDirections = new List<int>();
        for (int i = 0; i < OncomingTrafficLanes; i++) {
            laneDirections.Add(-1);
        }
        for (int i = 0; i < FollowingTrafficLanes; i++) {
            laneDirections.Add(1);
        }
        return laneDirections;
    }
}