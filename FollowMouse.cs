using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public float maxRightRotation = -90f;
    public float maxLeftRotation = 90f;
    public Transform target;
    private bool isattacking = false;
    public float attackcooldown = 1f;
    public Animator animator;

    private float previousAngle;
    private bool clockwise;

    private void Start()
    {
        animator.Play("swordcharged");
        Cursor.visible = false;

        // Initialize the previousAngle with the current rotation angle
        previousAngle = transform.eulerAngles.z;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger zone is tagged as "enemy" and if the player is not already attacking
        if (collision.tag == "enemy" && !isattacking)
        {
            // Calculate the direction vector from the target (the player) to the enemy
            Vector2 directionToEnemy = collision.transform.position - target.position;

            // Calculate the dot product to determine the relative position of the enemy
            // Dot product helps us understand if the enemy is on the right or left side of the target
            float dotProduct = Vector2.Dot(directionToEnemy.normalized, target.right);

            // Variable to store the direction in which the attack force will be applied
            Vector2 dir;

            // If the enemy is on the right side of the target (dotProduct > 0)
            if (dotProduct > 0)
            {
                // If the attack is clockwise, move diagonally up-right; otherwise, move straight up
                dir = clockwise ? new Vector2(1, 1) : new Vector2(0, 1);
            }
            else // If the enemy is on the left side of the target (dotProduct < 0)
            {
                if (clockwise)
                {
                    // If the attack is clockwise, move straight up
                    dir = new Vector2(0, 1);
                }
                else
                {
                    // If the attack is counterclockwise, move diagonally up-left
                    dir = new Vector2(-1, 1);
                }
            }

            // Mark that the player is now attacking
            isattacking = true;

            // Play the attack animation
            animator.Play("nothing");

            // Apply a force to the enemy in the calculated direction to simulate the attack impact
            Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
            rb.AddForce(dir * 5, ForceMode2D.Impulse);

            // Schedule the "isattacking" flag to be reset after the attack cooldown period
            Invoke("isattackingfalse", attackcooldown);
        }
    }





    public void isattackingfalse()
    {
        isattacking = false;
        animator.Play("swordcharged");
    }

    void Update()
    {
        // Get the mouse position in screen coordinates
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Convert the screen position to world coordinates
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // Since ScreenToWorldPoint returns a position with the same z value as the camera, set z to 0 to stay in 2D
        mouseWorldPosition.z = 0f;

        // Calculate the direction from the target to the mouse position
        Vector3 directionFromTarget = mouseWorldPosition - target.position;

        // Clamp the distance to be between 0.5 and 2 units
        float distance = directionFromTarget.magnitude;
        if (distance > 2f)
        {
            directionFromTarget = directionFromTarget.normalized * 2f;
        }
        else if (distance < 1f)
        {
            directionFromTarget = directionFromTarget.normalized * 1f;
        }

        // Set the position of the GameObject to the clamped position
        transform.position = target.position + directionFromTarget;

        // Calculate the direction from the GameObject to the target
        Vector3 direction = target.position - transform.position;

        // Calculate the angle in radians and then convert to degrees
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        // Add 180 degrees to the angle to make the back look at the target
        angle += 180;

        // Determine rotation direction
        float currentAngle = transform.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(previousAngle, currentAngle);

        if (angleDifference > 0)
        {
            clockwise = false;
        }
        else if (angleDifference < 0)
        {
            clockwise=true;
        }

        // Update the previous angle
        previousAngle = currentAngle;

        // Apply the rotation
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
    }
}
