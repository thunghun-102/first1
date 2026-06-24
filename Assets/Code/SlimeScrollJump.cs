using UnityEngine;

public class SlimeScrollJump : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    private SlimeHealth healthScript;
    private GravityRunner gravityScript; // Kết nối với script đảo trọng lực

    [Header("Ground Check")]
    public Transform groundCheck;
    public Transform ceilingCheck; // Thêm trần để check khi ở trên trần
    public LayerMask groundLayer;

    [Header("Jump Settings (Trục Y)")]
    public float maxJumpForceY = 15f;
    public float chargeSpeed = 12f;
    private float currentJumpForceY = 0f;

    [Header("Scroll Output (Truyền sang cho Map)")]
    public float normalScrollSpeed = 6f;
    [HideInInspector] public float currentScrollSpeed;
    [HideInInspector] public bool isCharging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<SlimeHealth>();
        gravityScript = GetComponent<GravityRunner>(); // Lấy script trọng lực
        currentScrollSpeed = normalScrollSpeed;
    }

    void Update()
    {
        if (healthScript != null && healthScript.IsDead())
        {
            if (isCharging || currentScrollSpeed > 0f) StopMapScrolling();
            return;
        }

        // Tự động kiểm tra xem đang bám sàn hay bám trần dựa vào script GravityRunner
        bool isUpsideDown = (gravityScript != null && gravityScript.IsTop());

        if (isUpsideDown)
        {
            isGrounded = Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, groundLayer);
        }
        else
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }

        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isCharging = true;
                currentJumpForceY = 0f;
            }

            if (Input.GetKey(KeyCode.Space) && isCharging)
            {
                currentJumpForceY += chargeSpeed * Time.deltaTime;
                currentJumpForceY = Mathf.Min(currentJumpForceY, maxJumpForceY);
                currentScrollSpeed = normalScrollSpeed * 0.2f;
            }

            if (Input.GetKeyUp(KeyCode.Space) && isCharging)
            {
                Jump(isUpsideDown); // Truyền trạng thái vào hàm Jump
            }
        }
        else
        {
            if (isCharging && !Input.GetKey(KeyCode.Space))
            {
                isCharging = false;
                currentScrollSpeed = normalScrollSpeed;
            }
        }
    }

    void Jump(bool upsideDown)
    {
        // Nếu đang ở trên trần (upsideDown = true) thì nhảy hướng xuống (-1)
        float direction = upsideDown ? -1f : 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForceY * direction);

        float forwardBoost = currentJumpForceY * 1.2f;
        currentScrollSpeed = normalScrollSpeed + forwardBoost;
        isCharging = false;
    }

    public void StopMapScrolling()
    {
        isCharging = false;
        currentScrollSpeed = 0f;
        normalScrollSpeed = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (healthScript != null && healthScript.IsDead()) return;
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            currentScrollSpeed = normalScrollSpeed;
        }
    }
}
