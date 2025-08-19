using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpForce = 10f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallGravityMultiplier = 2f;

    // Karakter y�n�n� takip etmek i�in
    private bool facingRight = true;

    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        Gravity();

        // Animasyon parametrelerini g�ncelle
        animator.SetFloat("speed", Mathf.Abs(horizontalMovement));

        // Karakter y�n�n� kontrol et ve �evir
        HandleDirection();
    }

    private void HandleDirection()
    {
        // Sa�a hareket ediyorsa ve karakter sola bak�yorsa
        if (horizontalMovement > 0 && !facingRight)
        {
            Flip();
        }
        // Sola hareket ediyorsa ve karakter sa�a bak�yorsa
        else if (horizontalMovement < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        // Karakteri Y ekseninde �evir
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded())
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    private bool isGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}