using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// #nullable enable

// Spawn Cars
// Despawn cars
// Keeps track of cars

// Town car is 5m long and 2m wide
// Average width of a lane is 3.7m
// 60 mph is 26.8224 meters per second

public class CarData {
    public Vector2 position;
    public Vector2 velocity;
    public float cruiseSpeed; // MPH
    public int lane; // TODO: Maybe add decimals for cars between lanes
    public GameObject gameObject;
    public bool dead;
    public int direction; // Direction of travel: -1 for oncoming traffic, 1 for following traffic

    // Used by Car Manager
    public bool isFree; // Whether or not this Car is allocated
    public int carIdx; // Index into CarData array
}

// Updates car positions
public class CarManager : Singleton<CarManager> {
    // Keep track of all cars in a list
    // TODO: This does not store CarData adjacent in memory because CarData is a value type.
    // TODO: This list should be treated like a pool of cars.
    // When we despawn cars, we should free up that slot.
    List<CarData> cars = new List<CarData>();
    Dictionary<GameObject, CarData> objectToCarData = new Dictionary<GameObject, CarData>();
    // When a car is despawned, its CarData slot can be re-used again.
    // We add its index into the freeList.
    // If there is no available spot in the freeList, then extend the cars array.
    Queue<int> freeList = new Queue<int>();

    [NonNullField]
    public GameObject CarPrefab;

    [NonNullField]
    public GameObject CarContainer;

    public int Seed = 1234;
    public float DEFAULT_CRUISE_SPEED = 26.8224f; // 60 mph
    public int OncomingTrafficSpawnDistance = 800;
    public int OncomingTrafficDespawnDistance = 50;
    public int FollowingTrafficSpawnDistance = 50;
    public int FollowingTrafficDespawnDistance = 800;
    public float SpawnCheckIntervalS = 0.2f; // Potentially spawn a car every X s
    private float spawnCheckTimer = 0; // Accumulates time

    private float carSpawnProbabilityOncoming; // Every Xs, Y% chance to spawn a car
    private float carSpawnProbabilityFollowing; // Every Xs, Y% chance to spawn a car

    private Vector3 centroid;
    private System.Random random;

    protected override void Awake() {
        base.Awake();
        random = new System.Random(Seed);
    }

    public void LoadLevel(LevelData level) {
        if (!level.ShouldSpawnCars) {
            return;
        }

        float checksPerSpec = 1 / SpawnCheckIntervalS;
        float oncomingCarsPerSec = level.CarSpawnsPerSecOncoming;
        float followingCarsPerSec = level.CarSpawnsPerSecFollowing;
        if (GameManager.Instance.DEVELOPMENT_MODE) {
            oncomingCarsPerSec = 0.05f;
            followingCarsPerSec = 0.1f;
        }
        carSpawnProbabilityOncoming = oncomingCarsPerSec / checksPerSpec;
        carSpawnProbabilityFollowing = followingCarsPerSec / checksPerSpec;

        SpawnInitialCars(level);
    }

    void FixedUpdate() {
        LevelData currentLevel = LevelManager.Instance.CurrentLevel;
        if (!currentLevel.ShouldSpawnCars) {
            return;
        }

        List<int> laneDirections = currentLevel.GetLaneDirections();
        Transform playerXform = PlayerManager.Instance.CameraController.transform;

        // Advance simulation
        centroid = Vector3.zero;
        for (int i = 0; i < cars.Count; i++) {
            if (cars[i].dead || cars[i].isFree) {
                continue;
            }
            cars[i].position += cars[i].velocity * Time.fixedDeltaTime;
            cars[i].gameObject.transform.position = new Vector3(cars[i].position.x, 0, cars[i].position.y);
            centroid += cars[i].gameObject.transform.position;
        }
        if (cars.Count > 0) {
            centroid = centroid / cars.Count;
        }

        // Spawn Cars
        spawnCheckTimer += Time.fixedDeltaTime;
        // Stop spawning cars if player goes past the end of the level
        if (spawnCheckTimer >= SpawnCheckIntervalS && playerXform.position.z < LevelManager.Instance.GetEndPosition()) {
            for (int lane = 0; lane < laneDirections.Count; lane++) {
                int direction = laneDirections[lane];
                float laneXPositionOffset = (laneDirections.Count - 1) * currentLevel.LaneSize / 2;
                float laneXPosition = lane * currentLevel.LaneSize - laneXPositionOffset;
                float laneYPosition;
                // Because the player is moving, the car spawns experience the doppler effect.
                // The gap between each car increases because the spawn point is moving, we can compensate for it by multiplying the destination.
                float spawnMultiplier = (DEFAULT_CRUISE_SPEED + PlayerManager.Instance.PlayerController.GetPlayerSpeed()) / DEFAULT_CRUISE_SPEED;
                if (direction < 0) {
                    // Oncoming Traffic
                    float modifiedCarSpawnProbability = spawnMultiplier * carSpawnProbabilityOncoming;
                    bool shouldSpawnCar = (float)random.NextDouble() <= modifiedCarSpawnProbability;
                    if (!shouldSpawnCar) {
                        continue;
                    }
                    // Spawn the car from beyond the view distance
                    laneYPosition = playerXform.position.z + OncomingTrafficSpawnDistance;
                    // Debug.Log($"Spawn Multiplier: {multiplier}");
                } else {
                    // Following Traffic
                    float modifiedCarSpawnProbability = spawnMultiplier * carSpawnProbabilityFollowing;
                    bool shouldSpawnCar = (float)random.NextDouble() <= modifiedCarSpawnProbability;
                    if (!shouldSpawnCar) {
                        continue;
                    }
                    // Spawn the car from behind the camera
                    laneYPosition = playerXform.position.z - FollowingTrafficSpawnDistance;
                }

                float speed = DEFAULT_CRUISE_SPEED + Random.Range(-4.4704f, 4.4704f); // +/- 10mph
                SpawnCar(new Vector2(laneXPosition, laneYPosition), lane, speed * direction);
            }
            spawnCheckTimer = 0;
        }

        // Despawn Cars
        for (int i = 0; i < cars.Count; i++) {
            if (cars[i].isFree) {
                // Ignore if it's already freed
                continue;
            }

            int direction = laneDirections[cars[i].lane];
            GameObject carObject = cars[i].gameObject;
            if (direction < 0 || cars[i].dead) {
                // Oncoming Traffic (or if car is no longer being simulated by CarManager)
                // If car goes a certain distance beyond the camera's position, then remove it.
                // Use the GameObject position here because it might've encoutered a collision and stop being simulated.
                if (carObject.transform.position.z < playerXform.position.z - OncomingTrafficDespawnDistance) {
                    DespawnCar(carObject);
                }
            } else {
                // Following Traffic
                // If the traffic goes past a certain distance past the end of the level, then remove it
                if (carObject.transform.position.z > LevelManager.Instance.GetEndPosition() + FollowingTrafficDespawnDistance) {
                    DespawnCar(carObject);
                }
            }
        }
    }

    // TODO: Return a ref to struct instead
    public CarData GetCharData(GameObject obj) {
        return objectToCarData[obj];
    }

    // TODO: Return a ref to struct instead
    public CarData? GetCarInFrontOf(Vector2 position, int lane) {
        for (int i = 0; i < cars.Count; i++) {
            // Ignore cars in other lanes
            if (cars[i].lane != lane) {
                continue;
            }

            // Check if the position is in front
            if (cars[i].position.y > position.y) {
                return cars[i];
            }
        }

        return null;
    }

    // This Car is no longer needed in this world.
    // Free up its Data and also delete the GameObject.
    public void DespawnCar(GameObject carObject) {
        // Add to freeList
        objectToCarData[carObject].isFree = true; // Stop simulating the physics
        objectToCarData[carObject].gameObject = null;
        freeList.Enqueue(objectToCarData[carObject].carIdx);

        objectToCarData.Remove(carObject);

        Destroy(carObject);
    }

    // Stop updating the car in CarManager and handoff to Unity Physics engine
    public void CarCollision(GameObject carObject) {
        if (!objectToCarData.ContainsKey(carObject)) {
            return;
        }

        var carData = objectToCarData[carObject];
        if (carData.dead) {
            return;
        }

        // Leave calculations to rigidbody
        var rb = carObject.GetComponent<Rigidbody>();
        rb.velocity = carData.velocity;
        rb.constraints = rb.constraints ^ RigidbodyConstraints.FreezeRotationY;

        // Leave calculations to rigidbody
        carData.velocity = new Vector2(0, 0);
        carData.dead = true;
    }

    public void SpawnCar(Vector2 position, int lane, float speed) {
        // Debug.Log($"[CarManager] SpawnCar {position}");

        // Instantiate the game object
        GameObject newObject = (GameObject)Instantiate(CarPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        newObject.transform.SetParent(CarContainer.transform);

        // See if there's any freed up CarData slots available
        CarData newCarData;
        if (freeList.Count > 0) {
            int carIdx = freeList.Dequeue();
            newCarData = cars[carIdx];
        } else {
            newCarData = new CarData();
            newCarData.carIdx = cars.Count;
            cars.Add(newCarData);
        }

        newCarData.position = position;
        newCarData.lane = lane;
        newCarData.cruiseSpeed = speed;
        newCarData.velocity = new Vector2(0, newCarData.cruiseSpeed);
        newCarData.gameObject = newObject;
        newCarData.dead = false;
        newCarData.isFree = false;

        objectToCarData[newCarData.gameObject] = newCarData;
    }

    void SpawnInitialCars(LevelData level) {
        float defaultGridLength = DEFAULT_CRUISE_SPEED * SpawnCheckIntervalS;
        int numCars = (int)((level.GetLevelLengthMeters() + OncomingTrafficSpawnDistance) / defaultGridLength);
        List<int> laneDirections = level.GetLaneDirections();

        float laneXPositionOffset = (laneDirections.Count - 1) * level.LaneSize / 2;
        for (int i = 0; i < laneDirections.Count; i++) {
            float laneXPosition = i * level.LaneSize - laneXPositionOffset;
            // TODO: More complex spawning logic, for now, just random uniform distribution
            for (int carIdx = 0; carIdx < numCars; carIdx++) {
                float randomValue = (float)random.NextDouble();
                float carSpawnProbability = laneDirections[i] < 0 ? carSpawnProbabilityOncoming : carSpawnProbabilityFollowing;
                if (randomValue <= carSpawnProbability) {
                    float laneYPosition = carIdx * defaultGridLength + LevelManager.Instance.GetLevelOffset();
                    float speed = DEFAULT_CRUISE_SPEED + Random.Range(-4.4704f, 4.4704f); // +/- 10mph
                    CarManager.Instance.SpawnCar(new Vector2(laneXPosition, laneYPosition), i, speed * laneDirections[i]);
                }
            }
        }
        spawnCheckTimer = 0;
    }
}
