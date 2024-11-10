using UnityEngine;

public class BolController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Reference to the ball's SpriteRenderer
    [SerializeField] private float detectionRange = 0.5f; // Range within which the player can affect the ball
    [SerializeField]private Player1Controller[] players1;
    [SerializeField]private Player2Controller[] players2;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    private void Update()
    {
        // Check for players in range (You might want to call this from the player scripts instead)
        Player1Controller player1 = FindObjectOfType<Player1Controller>();
        Player2Controller player2 = FindObjectOfType<Player2Controller>();

        // Check for players in range using raycasting
        bool playerInRange = false;

        foreach (Player1Controller player in players1)
        {
            if (IsPlayerInRange(player.transform))
            {
                playerInRange = true;
                break;
            }
        }

        if (!playerInRange)
        {
            foreach (Player2Controller player in players2)
            {
                if (IsPlayerInRange(player.transform))
                {
                    playerInRange = true;
                    break;
                }
            }
        }

        if (playerInRange)
        {
            ChangeColorToYellow();
        }
        else
        {
            ResetColor(); // Reset to original color if no players are in range
        }
    }

           // Debug.Log("not quite workx" );
    //   

    private bool IsPlayerInRange(Transform player)
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            return distanceToPlayer < detectionRange; // Check if the player is within the detection range
        }
        return false;
    }
    //  private bool IsPlayerInRange(Transform playerTransform)
    // {
    //     // Create a direction vector from the ball to the player
    //     Vector2 direction = (playerTransform.position - transform.position).normalized;

    //     // Perform a raycast
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange);

    //     // Check if the raycast hits the player
    //     if (hit.collider != null && hit.collider.transform == playerTransform)
    //     {
    //         return true; // Player is within range and not obstructed
    //     }

    //     return false; // No player detected in range
    // }

    private void ChangeColorToYellow()
    {
        spriteRenderer.color = Color.yellow; // Change the color of the ball to yellow
    }

    private void ResetColor()
    {
        spriteRenderer.color = Color.white; // Reset the color to white (or the original color)
    }
}