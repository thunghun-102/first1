using UnityEngine;

public class SlimeScrollJump : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    private SlimeHealth healthScript;
    private Animator anim;

    [Header("Ground Check Settings")]
    public LayerMask groundLayer;
    public float checkRadius = 0.15f; // Bán kính vòng tròn quét đất nhỏ gọn quanh chân Slime
    public Vector2 checkOffset = new Vector2(0f, -0.4f); // Khoảng cách dịch xuống dưới chân (Tự chỉnh số này)

    [Header("Jump Settings (Trục Y)")]
    public float maxJumpForceY = 15f;
    public float chargeSpeed = 12f;
    private float currentJumpForceY = 0f;

    [Header("Parallax Scroll Settings (Hình Parabol)")]
    public float normalScrollSpeed = 6f;
    public float forwardBoostWeight = 1.0f;
    [HideInInspector] public float currentScrollSpeed;
    [HideInInspector] public bool isCharging = false;

    [Header("Gravity Skill (Phím O)")]
    private bool isUpsideDown = false;

    private bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<SlimeHealth>();
        anim = GetComponent<Animator>();

        if (anim == null) anim = GetComponentInChildren<Animator>();

        currentScrollSpeed = normalScrollSpeed;
    }

    void Update()
    {
        if (healthScript != null && healthScript.IsDead())
        {
            if (isCharging || currentScrollSpeed > 0f) StopMapScrolling();
            return;
        }

        // 1. TỰ ĐỘNG CHECK ĐẤT ĐỘC LẬP CHUẨN XỊN (Không dùng Object con GroundCheck nữa)
        // Tính toán vị trí phát tia dựa theo hướng trọng lực hiện tại
        Vector2 currentOffset = isUpsideDown ? new Vector2(checkOffset.x, -checkOffset.y) : checkOffset;
        Vector2 checkPosition = (Vector2)transform.position + currentOffset;

        // Quét một vòng tròn vật lý thuần túy để tìm lớp mặt đất
        isGrounded = Physics2D.OverlapCircle(checkPosition, checkRadius, groundLayer);

        // --- KỸ NĂNG ĐẢO NGƯỢC TRỌNG LỰC (Bấm phím O) ---
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleGravity();
        }

        // --- XỬ LÝ ANIMATION KHI TIẾP ĐẤT ---
        if (isGrounded && !wasGrounded)
        {
            if (anim != null) anim.SetBool("isJumping", false);
        }
        wasGrounded = isGrounded;

        // 2. LOGIC ĐIỀU KHIỂN TỐC ĐỘ MAP THEO HÌNH PARABOL CHUẨN
        if (isGrounded)
        {
            if (!isCharging) currentScrollSpeed = normalScrollSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isCharging = true;
                currentJumpForceY = 5f;
            }

            if (Input.GetKey(KeyCode.Space) && isCharging)
            {
                currentJumpForceY += chargeSpeed * Time.deltaTime;
                currentJumpForceY = Mathf.Min(currentJumpForceY, maxJumpForceY);
                currentScrollSpeed = normalScrollSpeed * 0.2f;
            }

            if (Input.GetKeyUp(KeyCode.Space) && isCharging)
            {
                Jump();
            }
        }
        else
        {
            // KHI ĐANG BAY: Sử dụng linearVelocity chuẩn Parabol
            float speedY = Mathf.Abs(rb.linearVelocity.y);
            bool isFlyingUp = isUpsideDown ? (rb.linearVelocity.y < 0) : (rb.linearVelocity.y > 0);

            if (isFlyingUp)
            {
                currentScrollSpeed = normalScrollSpeed + (speedY * forwardBoostWeight);
            }
            else
            {
                currentScrollSpeed = Mathf.MoveTowards(currentScrollSpeed, normalScrollSpeed, 8f * Time.deltaTime);
            }
        }
    }

    void ToggleGravity()
    {
        isUpsideDown = !isUpsideDown;

        // Đảo chiều lực hút
        rb.gravityScale = isUpsideDown ? -1f : 1f;

        // Nhấc nhẹ Slime tách ra khỏi mặt đất để tránh kẹt va chạm
        float pushDirection = isUpsideDown ? 0.4f : -0.4f;
        transform.position = new Vector3(transform.position.x, transform.position.y + pushDirection, transform.position.z);

        // Lật ngược hình ảnh Model theo trục Y
        Vector3 currentScale = transform.localScale;
        currentScale.y = isUpsideDown ? -Mathf.Abs(currentScale.y) : Mathf.Abs(currentScale.y);
        transform.localScale = currentScale;

        if (isCharging)
        {
            isCharging = false;
            currentScrollSpeed = normalScrollSpeed;
        }
    }

    void Jump()
    {
        float jumpDirection = isUpsideDown ? -1f : 1f;
        rb.linearVelocity = new Vector2(0f, currentJumpForceY * jumpDirection);
        isCharging = false;

        if (anim != null) anim.SetBool("isJumping", true);
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

    // Hàm vẽ vòng tròn ảo trong chế độ Scene giúp bạn căn chỉnh chân Slime dễ dàng
    private void OnDrawGizmosSelected()
    {
        Vector2 currentOffset = isUpsideDown ? new Vector2(checkOffset.x, -checkOffset.y) : checkOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + currentOffset, checkRadius);
    }
}
