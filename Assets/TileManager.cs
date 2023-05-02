using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileManager : Singleton<TileManager> {
    [NonNullField]
    public GameObject InvisibleWallsObject;

    [NonNullField]
    public GameObject DeliveryLocationPrefab;

    [NonNullField]
    public GameObject TowerPrefab;

    public int MaxViewDistance = 800;
    private int maxLoadedTiles;

    private LevelData? currentLevel = null;
    private Queue<(int, GameObject)> loadedTiles = new Queue<(int, GameObject)>();
    private HashSet<int> loadedTileIndices = new HashSet<int>();

    private float levelOffset = 0;
    private int currentTile;
    private int maxLoadedTileForLevel = -1; // Start it as -1 to stop first level from having an offset
    private int maxLoadedTile = 0;
    private int tilesInLevel;
    private float stopLocation;

    private int levelsLoaded = 0;

    private GameObject TowerObject = null;
    private int towerOffset = 40;
    private float distanceToTower = 0;

    protected override void Awake() {
        base.Awake();
        // Clear any stuff here from development
        foreach (Transform child in this.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void Update() {
        if (currentLevel == null) {
            return;
        }

        float playerPosition = PlayerManager.Instance.PlayerController.transform.position.z;
        // TODO: move this out of here
        InvisibleWallsObject.transform.position = new Vector3(
            InvisibleWallsObject.transform.position.x,
            InvisibleWallsObject.transform.position.y,
            playerPosition
        );

        MaybeLoadMoreTiles();

        // Hack for big building
        if (TowerObject != null) {
            // LERP the y pos
            float t = (1 - (LevelManager.Instance.GetEndPosition() - playerPosition) / distanceToTower);
            TowerObject.transform.position = new Vector3(
                TowerObject.transform.position.x,
                Mathf.Lerp(-550.0f, 0, t),
                TowerObject.transform.position.z);
        }
    }

    public float GetLevelOffset() {
        return levelOffset;
    }

    private void MaybeLoadMoreTiles() {
        float playerPosition = PlayerManager.Instance.PlayerController.transform.position.z;

        float playerPositionInLevel = playerPosition - LevelManager.Instance.GetLevelOffset();
        float maxPosition = playerPositionInLevel + MaxViewDistance;
        currentTile = (int)(playerPositionInLevel / currentLevel.TileSize);
        int maxTileIdx = (int)(maxPosition / currentLevel.TileSize);
        maxLoadedTileForLevel = maxTileIdx;
        for (int i = currentTile; i <= maxTileIdx; i++) {
            LoadTile(i);
        }
    }

    public void LoadLevel(LevelData level) {
        // Can we... shift everything in the world back... such that the next level starts at 0?
        // Can we put the user on a treadmill while in the transitioning phase?
        levelsLoaded += 1;
        maxLoadedTile = maxLoadedTile + maxLoadedTileForLevel;
        levelOffset = (maxLoadedTile + 1) * level.TileSize;
        Debug.Log($"Load Level {level.Name} with offset {levelOffset}. MaxLoadedTile {maxLoadedTile}");
        currentTile = 0;
        currentLevel = level;
        maxLoadedTiles = (int)(MaxViewDistance / level.TileSize) + 3;
        tilesInLevel = (int)(level.GetLevelLengthMeters() / level.TileSize); // How many tiles to use the regular level for.
        stopLocation = (tilesInLevel + 1.25f) * level.TileSize;

        // Initial tiles
        MaybeLoadMoreTiles();

        // Adjust size of rails
        float roadWidth = (level.OncomingTrafficLanes + level.FollowingTrafficLanes) * level.LaneSize;
        Transform rightRail = InvisibleWallsObject.transform.Find("Right Rail");
        rightRail.position = new Vector3(
            roadWidth / 2,
            rightRail.position.y,
            rightRail.position.z
        );
        Transform leftRail = InvisibleWallsObject.transform.Find("Left Rail");
        leftRail.position = new Vector3(
            -roadWidth / 2,
            leftRail.position.y,
            leftRail.position.z
        );

        // Hack for big building
        if (level.name == "Last Level") {
            TowerObject = Instantiate(TowerPrefab, new Vector3(0, 0, LevelManager.Instance.GetEndPosition()), Quaternion.identity);
            distanceToTower = LevelManager.Instance.GetEndPosition() - PlayerManager.Instance.PlayerController.transform.position.z;
        }
    }

    void LoadTile(int tileIdx) {
        int tileUid = tileIdx + levelsLoaded * 1000;

        // Debug.Log($"[TileManager] Load Tile {tileIdx}. UID: {tileUid}");

        if (tileIdx < 0 || loadedTileIndices.Contains(tileUid)) {
            return;
        }

        Vector3 newTilePosition = new Vector3(0, 0, tileIdx * currentLevel.TileSize + GetLevelOffset());

        GameObject tilePrefab;
        bool isRegularLevelTile = false;
        if (tileIdx == 0) {
            // Spawn first tile in the level
            tilePrefab = currentLevel.EntryTile;
        } else if (tileIdx == tilesInLevel) {
            // Spawn exit tile (tile that contains the finish line)
            tilePrefab = currentLevel.ExitTile;
        } else if (tileIdx > tilesInLevel) {
            // Spawn level transition tiles.
            tilePrefab = currentLevel.TransitionTile;
        } else {
            int levelTileIdx = tileIdx % currentLevel.LevelTiles.Count;
            tilePrefab = currentLevel.LevelTiles[levelTileIdx];
            isRegularLevelTile = true;
        }
        GameObject newTileObj = Instantiate(tilePrefab, newTilePosition, Quaternion.identity);
        newTileObj.transform.parent = this.transform;
        loadedTiles.Enqueue((tileIdx, newTileObj));
        loadedTileIndices.Add(tileUid);

        if (isRegularLevelTile) {
            // Hack to tile the first level
            Material mat = newTileObj.GetComponentInChildren<MeshRenderer>().material;
            if (tileIdx % 2 == 1) {
                mat.SetFloat("_FlipV", 1);
            }

            // Check if there is a delivery location that is within range of the tile
            float rangeMin = tileIdx * currentLevel.TileSize - 50;
            float rangeMax = (tileIdx + 1) * currentLevel.TileSize + 50;
            List<float> deliveryLocations = currentLevel.DeliveryLocations;
            for (int i = 0; i < deliveryLocations.Count; i++) {
                if (deliveryLocations[i] >= rangeMin && deliveryLocations[i] <= rangeMax) {
                    float deliveryZPosition = deliveryLocations[i] + GetLevelOffset();

                    // Tell the material to stop drawing terrain in that area
                    mat.SetFloat("_HasClearingLocation", 1);
                    mat.SetFloat("_ClearingLocation", deliveryZPosition);

                    // ALso spawn a delivery location there
                    GameObject deliveryLocationObj = Instantiate(DeliveryLocationPrefab, new Vector3(25, 0, deliveryZPosition + 15), Quaternion.identity);
                    deliveryLocationObj.transform.SetParent(newTileObj.transform);
                }
            }
        }

        if (loadedTiles.Count > maxLoadedTiles) {
            UnloadOldestTile();
        }
    }

    void UnloadOldestTile() {
        (int tileIdx, GameObject oldTile) = loadedTiles.Dequeue();
        Destroy(oldTile);
        loadedTileIndices.Remove(tileIdx);
    }
}
