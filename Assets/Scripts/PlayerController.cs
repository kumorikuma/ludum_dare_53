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

    public GameObject PlayerModel;

    //Private movement variables
    private Rigidbody rb;
    private CharacterController characterController;

    private Vector2 inputMoveVector;
    private bool inputJumpOnNextFrame = false;
    private Vector3 velocity;  // velocity relative to player: +x = right, +y = up, +z = forward


    private void Awake() {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(Vector2 moveVector) {
        inputMoveVector = moveVector;
    }

    public void OnJump() {
        inputJumpOnNextFrame = true;
        Debug.Log("OnJump");
    }

    // Using KBM controls, there's a specific button to walk.
    public void OnWalk(bool isWalking) {
    }

    void FixedUpdate() {
        // Accelerate / decelerate
        if (inputMoveVector.y > 0.1) {
            velocity.z += Acceleration * Time.deltaTime;
        } else if (inputMoveVector.y < -0.1) {
            velocity.z -= Acceleration * Time.deltaTime;
        } else {
            // Decay to default speed
            velocity.z -= Mathf.Sign(velocity.z - DefaultSpeed) * Acceleration * Time.deltaTime;
        }
        velocity.z = Mathf.Clamp(velocity.z, MinSpeed, MaxSpeed);

        // Move left / right
        if (inputMoveVector.x > 0.1) {
            velocity.x += SideAcceleration * Time.deltaTime;
        } else if (inputMoveVector.x < -0.1) {
            velocity.x -= SideAcceleration * Time.deltaTime;
        } else {
            // Decay to 0
            velocity.x -= Mathf.Sign(velocity.x) * SideAcceleration * Time.deltaTime;
        }
        velocity.x = Mathf.Clamp(velocity.x, -MaxSideSpeed, MaxSideSpeed);

        // Jump
        if (inputJumpOnNextFrame) {
            velocity.y = JumpForce;
            inputJumpOnNextFrame = false;
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        Debug.Log($"Move {velocity.x}, {velocity.y}, {velocity.z}");

        Vector3 newPosition = transform.position + velocity * Time.deltaTime;
        newPosition.y = Mathf.Max(0, newPosition.y);
        transform.position = newPosition;

        // characterController.Move(velocity * Time.deltaTime);
    }

}