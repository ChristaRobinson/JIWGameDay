using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Size")]
    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    [Header("Speed")]
    public float minSpeed = 5f;
    public float maxSpeed = 10f;

    private Rigidbody2D rb;
    private float speed;

    void Start()
    {
        // Random obstacle size
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1f);

        // Get Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Obstacle needs a Rigidbody2D!");
            return;
        }

        // Physics settings
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.freezeRotation = true;

        // Random speed
        speed = Random.Range(minSpeed, maxSpeed);

        // Random movement direction
        Vector2 direction = Random.insideUnitCircle.normalized;

        // Make sure we never get a zero direction
        if (direction == Vector2.zero)
        {
            direction = Vector2.right;
        }

        // Start moving
        rb.linearVelocity = direction * speed;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        Camera cam = Camera.main;

        if (cam == null)
            return;

        // Get camera boundaries
        Vector3 bottomLeft = cam.ViewportToWorldPoint(
            new Vector3(0f, 0f, 0f)
        );

        Vector3 topRight = cam.ViewportToWorldPoint(
            new Vector3(1f, 1f, 0f)
        );

        // Get obstacle size
        Collider2D obstacleCollider = GetComponent<Collider2D>();

        if (obstacleCollider == null)
            return;

        float halfWidth = obstacleCollider.bounds.extents.x;
        float halfHeight = obstacleCollider.bounds.extents.y;

        Vector2 position = rb.position;
        Vector2 velocity = rb.linearVelocity;

        // LEFT edge
        if (position.x - halfWidth <= bottomLeft.x && velocity.x < 0f)
        {
            position.x = bottomLeft.x + halfWidth;
            velocity.x = Mathf.Abs(velocity.x);
        }

        // RIGHT edge
        if (position.x + halfWidth >= topRight.x && velocity.x > 0f)
        {
            position.x = topRight.x - halfWidth;
            velocity.x = -Mathf.Abs(velocity.x);
        }

        // BOTTOM edge
        if (position.y - halfHeight <= bottomLeft.y && velocity.y < 0f)
        {
            position.y = bottomLeft.y + halfHeight;
            velocity.y = Mathf.Abs(velocity.y);
        }

        // TOP edge
        if (position.y + halfHeight >= topRight.y && velocity.y > 0f)
        {
            position.y = topRight.y - halfHeight;
            velocity.y = -Mathf.Abs(velocity.y);
        }

        // Apply corrected position
        rb.position = position;

        // Keep a consistent speed
        if (velocity.magnitude > 0.01f)
        {
            rb.linearVelocity = velocity.normalized * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.right * speed;
        }
    }
}