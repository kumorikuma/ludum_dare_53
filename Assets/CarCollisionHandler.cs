using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionHandler : MonoBehaviour {
    void OnCollisionEnter(Collision collision) {
        // Check if the car has collided with an obstacle
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("The car has collided with an obstacle!");
        }
    }

}
