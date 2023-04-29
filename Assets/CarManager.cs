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
}

// Updates car positions
public class CarManager : Singleton<CarManager> {
    // Keep track of all cars in a list
    // TODO: This does not store CarData adjacent in memory because CarData is a value type.
    // TODO: This list should be treated like a pool of cars.
    // When we despawn cars, we should free up that slot.
    List<CarData> cars = new List<CarData>();
    Dictionary<GameObject, CarData> objectToCarData = new Dictionary<GameObject, CarData>();

    [NonNullField]
    public GameObject CarPrefab;

    [NonNullField]
    public GameObject CarContainer;

    public float DEFAULT_CRUISE_SPEED = 26.8224f; // 60 mph

    private Vector3 centroid;

    public GameObject FollowCam;

    void FixedUpdate() {
        centroid = Vector3.zero;
        for (int i = 0; i < cars.Count; i++) {
            cars[i].position += cars[i].velocity * Time.fixedDeltaTime;
            cars[i].gameObject.transform.position = new Vector3(cars[i].position.x, 0, cars[i].position.y);
            centroid += cars[i].gameObject.transform.position;
        }
        centroid = centroid / cars.Count;
        if (FollowCam) {
            FollowCam.transform.position = centroid;
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

    public void DespawnCar(GameObject carObject) {
        // TODO:
    }

    public void SpawnCar(Vector2 position, int lane) {
        // Instantiate the game object
        // Debug.Log($"[CarManager] SpawnCar {position}");
        GameObject newObject = (GameObject)Instantiate(CarPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        newObject.transform.SetParent(CarContainer.transform);

        CarData newCarData = new CarData();
        newCarData.position = position;
        newCarData.lane = lane;
        newCarData.cruiseSpeed = DEFAULT_CRUISE_SPEED + Random.Range(-10, 10);
        newCarData.velocity = new Vector2(0, newCarData.cruiseSpeed);
        newCarData.gameObject = newObject;
        cars.Add(newCarData);

        objectToCarData[newCarData.gameObject] = newCarData;
    }

    public void CarCollision(GameObject carObject) {
        // TODO stop car and ragdoll
    }
}
