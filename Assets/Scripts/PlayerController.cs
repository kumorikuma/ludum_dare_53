using System.Collections.Generic;
using UnityEngine;

/*Simple player movement controller, based on character controller component,
with footstep system based on check the current texture of the component*/
public class PlayerController : MonoBehaviour {
    [NonNullField]
    public Animator Animator;

    //Variables for footstep system list
    [System.Serializable]
    public class GroundLayer {
        public string layerName;
        public Texture2D[] groundTextures;
        public AudioClip[] footstepSounds;
    }

    [Header("Forward Acceleration")]
    [Tooltip("Acceleration")]
    [SerializeField]
    private float Acceleration = 0.1f;

    [Tooltip("Side acceleration")]
    [SerializeField]
    private float SideAcceleration = 1.0f;

    [Tooltip("Force of the jump with which the controller rushes upwards")]
    [SerializeField]
    private float JumpForce = 1.0f;

    [SerializeField]
    private float DefaultSpeed = 0.0f;

    [SerializeField]
    private float MaxSpeed = 40.0f;

    [SerializeField]
    private float MinSpeed = -10.0f;

    [SerializeField]
    private float MaxSideSpeed = 20.0f;

    [Tooltip("Gravity, pushing down controller when it jumping")]
    [SerializeField]
    private float gravity = -9.81f;

    public Vector3 packageFireVelocity = new Vector3(0, 0, 0);

    public GameObject PlayerModel;

    [NonNullField]
    [SerializeField]
    private GameObject deliveryPackage;

    private float shotCooldown = 1f;
    private float currentShotCooldown = 1f;

    //Private movement variables
    private Rigidbody rb;
    private CharacterController characterController;

    private Vector2 inputMoveVector;
    private bool inputJumpOnNextFrame = false;
    private Vector3 velocity;  // velocity relative to player: +x = right, +y = up, +z = forward

    public float GetPlayerSpeed() {
        return velocity.z;
    }

    // Continue on current path to ZPos.
    // Disable user control.
    // Callback when done.
    public void DriveToZPos() {

    }

    // Turn towards position and drive towards it
    public void DriveToPosition() {

    }

    public void Reset() {
        var position = transform.position;
        position.z = 0;
        transform.position = position;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    public void OnMove(Vector2 moveVector) {
        inputMoveVector = moveVector;
    }

    public void OnJump() {
        inputJumpOnNextFrame = true;
        Debug.Log("OnJump");
    }

    public void OnShoot() {
        if (currentShotCooldown > 0) {
            return;
        }
        var package = Instantiate(deliveryPackage, transform.position, Quaternion.identity);
        var packageRb = package.GetComponent<Rigidbody>();
        packageRb.velocity = velocity + packageFireVelocity;
        var rotationForce = packageRb.velocity.magnitude;
        packageRb.AddTorque(Random.onUnitSphere * rotationForce);
        SoundSystem.Instance.PlayClip("yeet");
        currentShotCooldown = shotCooldown;
    }

    // Using KBM controls, there's a specific button to walk.
    public void OnWalk(bool isWalking) {
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Obstacle")) {
            SoundSystem.Instance.PlayClip("crash");
            GameManager.Instance.ScoreCollision(collision.relativeVelocity.magnitude);
        }
    }

    void Update() {
        if (currentShotCooldown > 0) {
            currentShotCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        // Accelerate / decelerate
        velocity = rb.velocity;
        if (inputMoveVector.y > 0.1) {
            velocity.z += Acceleration * Time.fixedDeltaTime;
        } else if (inputMoveVector.y < -0.1) {
            velocity.z -= Acceleration * Time.fixedDeltaTime;
        } else if (Mathf.Abs(velocity.z - DefaultSpeed) > 0.01) {
            // Decay to default speed
            velocity.z -= Mathf.Sign(velocity.z - DefaultSpeed) * Acceleration * Time.fixedDeltaTime;
        }
        velocity.z = Mathf.Clamp(velocity.z, MinSpeed, MaxSpeed);
        SoundSystem.Instance.SetEngineLevel(velocity.z / MaxSpeed);

        // Move left / right
        if (inputMoveVector.x > 0.1) {
            velocity.x += SideAcceleration * Time.fixedDeltaTime;
        } else if (inputMoveVector.x < -0.1) {
            velocity.x -= SideAcceleration * Time.fixedDeltaTime;
        } else if (Mathf.Abs(velocity.x) > 0.01) {
            // Decay to 0
            velocity.x = velocity.x * 0.9f * Time.fixedDeltaTime;
        }
        velocity.x = Mathf.Clamp(velocity.x, -MaxSideSpeed, MaxSideSpeed);

        // Jump
        if (inputJumpOnNextFrame) {
            velocity.y = JumpForce;
            inputJumpOnNextFrame = false;
        }

        // Gravity
        velocity.y += gravity * Time.fixedDeltaTime;

        if (transform.position.y <= 0) {
            var position = transform.position;
            position.y = 0;
            transform.position = position;
            velocity.y = Mathf.Max(0, velocity.y);
        }

        rb.velocity = velocity;

        // Debug.Log($"Move {velocity.x}, {velocity.y}, {velocity.z}");

        // Vector3 newPosition = transform.position + velocity * Time.deltaTime;
        // newPosition.y = Mathf.Max(0, newPosition.y);
        // transform.position = newPosition;

        // characterController.Move(velocity * Time.deltaTime);
    }

}