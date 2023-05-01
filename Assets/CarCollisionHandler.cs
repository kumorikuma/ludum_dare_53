using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionHandler : MonoBehaviour {
    // If using trigger
    void OnTriggerEnter(Collider other) {
        var playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController) {
            Debug.Log("Trigger collision");
            CarManager.Instance.CarCollision(gameObject);
            var rb = this.GetComponent<Rigidbody>();

            var animator = this.GetComponent<Animator>();
            animator.ResetTrigger("Ragdoll");
        }
    }

    // If using rigidbody collision
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            CarManager.Instance.CarCollision(gameObject);
        }
    }

}
