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
}