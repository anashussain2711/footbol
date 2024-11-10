using UnityEngine;
using UnityEngine.InputSystem;

public class Player1Controller : MonoBehaviour
{
    private GameControls controls;
    private Vector2 moveInput;
    public float speed = 1.5f;
    public float collisionBounceForce = 0.5f; // Bounce force applied on collision with the ball

    [SerializeField] private GameObject bol;            // Reference to the ball GameObject
    public float minShootForce = 1.5f;    // Minimum force applied when shooting
    [SerializeField] private BoxCollider2D playerCollider;  // Reference to the player's BoxCollider2D

    public float maxShootForce = 7f;   // Maximum force applied when shooting
    [SerializeField] private Rigidbody2D bolRb;         // Rigidbody of the ball
    [SerializeField] private Rigidbody2D playerRb;      // Rigidbody of the player (applied to legs)
    private Animator animator;          // Animator reference

    private bool isChargingShoot = false; // Tracks if the shoot button is held down
    private float shootChargeStartTime;   // Time when shoot button was pressed

    private void Awake()
    {
        controls = new GameControls();
        bolRb = bol.GetComponent<Rigidbody2D>();  // Get the ball's Rigidbody2D
        playerRb = GetComponent<Rigidbody2D>();   // Get the player's Rigidbody2D
        animator = GetComponent<Animator>();       // Get the Animator component
        playerCollider = GetComponent<BoxCollider2D>();  // Get the BoxCollider2D of the player
    }

    private void OnEnable()
    {
        controls.Player1.Enable();

        // Subscribe to input events
        controls.Player1.Move.performed += OnMove;
        controls.Player1.Move.canceled += OnMoveCanceled;

        // Set up shoot charging on key down and release on key up
        controls.Player1.Shoot.started += OnShootStarted;
        controls.Player1.Shoot.canceled += OnShootReleased;
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        controls.Player1.Move.performed -= OnMove;
        controls.Player1.Move.canceled -= OnMoveCanceled;

        controls.Player1.Shoot.started -= OnShootStarted;
        controls.Player1.Shoot.canceled -= OnShootReleased;

        controls.Player1.Disable();
    }

    private void Update()
    {
        // Apply movement to the player
        Vector2 movement = moveInput * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Update the isRunning boolean in the animator based on movement
        bool isRunning = moveInput != Vector2.zero;
        animator.SetBool("isRunning", isRunning);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Rotate the player based on movement direction
        if (moveInput.x < 0) // Moving left
        {
            transform.rotation = Quaternion.Euler(12, 180, 0); // Rotate to face left
        }
        else if (moveInput.x > 0) // Moving right
        {
            transform.rotation = Quaternion.Euler(-12, 0, 0); // Rotate to face right
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        if (IsBallInRange())
        {
            isChargingShoot = true;
            shootChargeStartTime = Time.time;  // Record the time when shoot started
            Debug.Log("Player 1 Shoot Charging...");
        }
    }

    private void OnShootReleased(InputAction.CallbackContext context)
    {
        if (isChargingShoot && IsBallInRange())
        {
            isChargingShoot = false;
            float chargeDuration = Time.time - shootChargeStartTime; // Calculate how long shoot was held

            // Calculate dynamic shoot force based on charge duration
            float dynamicShootForce = Mathf.Lerp(minShootForce, maxShootForce, chargeDuration);

            // Calculate the direction based on the ball's position relative to the player's BoxCollider2D center
            Vector2 directionToBall = ((Vector2)bol.transform.position - (Vector2)playerCollider.bounds.center).normalized;


            // Apply the calculated direction and dynamic force to the ball
            bolRb.AddForce(directionToBall * dynamicShootForce, ForceMode2D.Impulse);
            Debug.Log("Player 1 Shoot with force: " + dynamicShootForce + " in direction: " + directionToBall);

            // Set isKicking to true on shoot release in the animator
            animator.SetBool("isKicking", true);
            Invoke(nameof(ResetIsKicking), 0.1f);
        }
        else
        {
            // Reset charging state if out of range
            isChargingShoot = false;
            Debug.Log("Player moved out of range before releasing the shoot.");
        }
    }

    private bool IsBallInRange()
    {
        float distanceToBall = Vector2.Distance(playerRb.position, bol.transform.position);
        return distanceToBall < 0.5f;  // Adjust range as needed
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == bol)  // Check if collided object is the ball
        {
            Vector2 bounceDirection = (bol.transform.position - transform.position).normalized;  // Direction away from the player
            bolRb.AddForce(bounceDirection * collisionBounceForce, ForceMode2D.Impulse);  // Apply bounce force
            Debug.Log("Player collided with the ball, applying bounce force.");
        }
    }

    private void ResetIsKicking()
    {
        animator.SetBool("isKicking", false);
    }
}
