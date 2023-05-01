using UnityEngine;

public class SpinBounce : MonoBehaviour {
    public float rotationSpeed = 20f;
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 2f;

    private float initialY;

    void Start() {
        initialY = transform.position.y;
    }

    void Update() {
        // Rotate the object around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Bounce the object up and down using an easing function
        float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight * 0.5f;
        transform.position = new Vector3(transform.position.x, initialY + bounce, transform.position.z);
    }
}