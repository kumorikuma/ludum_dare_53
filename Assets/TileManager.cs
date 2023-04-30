using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileManager : Singleton<TileManager> {
    [NonNullField]
    public GameObject InvisibleWallsPrefab;
    private GameObject invisibleWallsObject;

    public int MaxViewDistance = 800;
    private int maxLoadedTiles;

    private LevelData? currentLevel = null;
    private int currentTile = 0;
    private Queue<GameObject> loadedTiles = new Queue<GameObject>();

    void Update() {
        invisibleWallsObject.transform.position = new Vector3(
            invisibleWallsObject.transform.position.x,
            invisibleWallsObject.transform.position.y,
            PlayerManager.Instance.PlayerController.transform.position.z
        );
    }

    public void LoadLevel(LevelData level) {
        currentLevel = level;
        maxLoadedTiles = (int)(MaxViewDistance / level.TileSize) + 1;

        // Unload current level
        while (loadedTiles.Count > 0) {
            UnloadOldestTile();
        }
        foreach (Transform child in this.transform) {
            // There might also be other stuff under here
            GameObject.Destroy(child.gameObject);
        }

        // Load all tiles at once for now
        for (int i = 0; i < level.Tiles.Count; i++) {
            LoadTile(i);
        }

        // Spawn guard rails to stop player from going into the terrain
        invisibleWallsObject = Instantiate(InvisibleWallsPrefab, Vector3.zero, Quaternion.identity);
        invisibleWallsObject.transform.SetParent(this.transform);
        float roadWidth = (level.OncomingTrafficLanes + level.FollowingTrafficLanes) * level.LaneSize;
        Transform rightRail = invisibleWallsObject.transform.Find("Right Rail");
        rightRail.position = new Vector3(
            roadWidth / 2,
            rightRail.position.y,
            rightRail.position.z
        );
        Transform leftRail = invisibleWallsObject.transform.Find("Left Rail");
        leftRail.position = new Vector3(
            -roadWidth / 2,
            leftRail.position.y,
            leftRail.position.z
        );
    }

    void LoadTile(int tileIdx) {
        if (tileIdx >= currentLevel.Tiles.Count) {
            Debug.Log($"[TileManager] Error loading tile: {tileIdx} >= tile counts for level '{currentLevel.name}'");
            return;
        }

        GameObject newTileObj = Instantiate(currentLevel.Tiles[tileIdx], new Vector3(0, 0, currentTile * currentLevel.TileSize), Quaternion.identity);
        newTileObj.transform.parent = this.transform;
        loadedTiles.Enqueue(newTileObj);

        if (loadedTiles.Count > maxLoadedTiles) {
            // TODO: Implement loading / unloading
            // UnloadOldestTile();
        }

        currentTile++;
    }

    void UnloadOldestTile() {
        GameObject oldTile = loadedTiles.Dequeue();
        Destroy(oldTile);
    }
}
