using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileManager : Singleton<TileManager> {
    [NonNullField]
    public GameObject InvisibleWallsPrefab;

    [NonNullField]
    public GameObject DeliveryLocationPrefab;

    private GameObject invisibleWallsObject;

    public int MaxViewDistance = 800;
    private int maxLoadedTiles;

    private LevelData? currentLevel = null;
    private int currentTile = 0;
    private Queue<(int, GameObject)> loadedTiles = new Queue<(int, GameObject)>();
    private HashSet<int> loadedTileIndices = new HashSet<int>();

    void Update() {
        if (currentLevel == null) {
            return;
        }

        float playerPosition = PlayerManager.Instance.PlayerController.transform.position.z;
        invisibleWallsObject.transform.position = new Vector3(
            invisibleWallsObject.transform.position.x,
            invisibleWallsObject.transform.position.y,
            playerPosition
        );

        float maxPosition = playerPosition + MaxViewDistance;
        currentTile = (int)(playerPosition / currentLevel.TileSize);
        int maxTileIdx = (int)(maxPosition / currentLevel.TileSize);
        for (int i = currentTile; i <= maxTileIdx; i++) {
            LoadTile(i);
        }
    }

    public void LoadLevel(LevelData level) {
        currentTile = 0;
        currentLevel = level;
        maxLoadedTiles = (int)(MaxViewDistance / level.TileSize) + 2;

        // Unload current level
        while (loadedTiles.Count > 0) {
            UnloadOldestTile();
        }
        foreach (Transform child in this.transform) {
            // There might also be other stuff under here
            GameObject.Destroy(child.gameObject);
        }

        // Initial tiles
        float playerPosition = PlayerManager.Instance.PlayerController.transform.position.z;
        float maxPosition = playerPosition + MaxViewDistance;
        currentTile = (int)(playerPosition / currentLevel.TileSize);
        int maxTileIdx = (int)(maxPosition / currentLevel.TileSize);
        for (int i = currentTile; i <= maxTileIdx; i++) {
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
        if (loadedTileIndices.Contains(tileIdx)) {
            return;
        }

        Debug.Log($"[TileManager] Load Tile: {tileIdx}");
        int levelTileIdx = tileIdx % currentLevel.Tiles.Count;
        GameObject newTileObj = Instantiate(currentLevel.Tiles[levelTileIdx], new Vector3(0, 0, tileIdx * currentLevel.TileSize), Quaternion.identity);
        newTileObj.transform.parent = this.transform;
        loadedTiles.Enqueue((tileIdx, newTileObj));
        loadedTileIndices.Add(tileIdx);

        // Check if there is a delivery location that is within range of the tile
        float rangeMin = tileIdx * currentLevel.TileSize - 50;
        float rangeMax = (tileIdx + 1) * currentLevel.TileSize + 50;
        List<float> deliveryLocations = currentLevel.DeliveryLocations;
        for (int i = 0; i < deliveryLocations.Count; i++) {
            if (deliveryLocations[i] >= rangeMin && deliveryLocations[i] <= rangeMax) {
                // Tell the material to stop drawing terrain in that area
                Material mat = newTileObj.GetComponentInChildren<MeshRenderer>().material;
                mat.SetFloat("_ClearingLocation", deliveryLocations[i]);

                // ALso spawn a delivery location there
                GameObject deliveryLocationObj = Instantiate(DeliveryLocationPrefab, new Vector3(25, 0, deliveryLocations[i] + 15), Quaternion.identity);
                deliveryLocationObj.transform.SetParent(newTileObj.transform);
            }
        }

        if (loadedTiles.Count > maxLoadedTiles) {
            // TODO: Implement loading / unloading
            UnloadOldestTile();
        }
    }

    void UnloadOldestTile() {
        (int tileIdx, GameObject oldTile) = loadedTiles.Dequeue();
        Destroy(oldTile);
        loadedTileIndices.Remove(tileIdx);
    }
}
