using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryPackage : MonoBehaviour {
    // Time to live
    public float lifeTime = 10f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) {
            Destroy(this);
        }
    }

    void OnTriggerEnter(Collider collider) {
        Debug.Log("Hit");
        Destroy(this);
    }

}
