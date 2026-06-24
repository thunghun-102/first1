using UnityEngine;

public class SlimeScrollJump2 : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    private SlimeHealth healthScript;
    private GravityRunner gravityScript;
    private Animator anim; // Thêm biến lưu trữ Animator

    [Header("Ground Check")]
    public Transform groundCheck;
    public Transform ceilingCheck;
    public LayerMask groundLayer;

    [Header("Jump Settings (Trục Y)")]
    public float maxJumpForceY = 15f;
    public float chargeSpeed = 12f;
    private float currentJumpForceY = 0f;

    [Header("Scroll Output (Truyền sang cho Map)")]
    public float normalScrollSpeed = 6f;
    [HideInInspector] public float currentScrollSpeed;
    [HideInInspector] public bool isCharging = false;

    // Biến phụ để tối ưu hóa việc kiểm tra trạng thái tiếp đất trước đó
    private bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<SlimeHealth>();
        gravityScript = GetComponent<GravityRunner>();
        anim = GetComponent<Animator>(); // Khởi tạo Animator
        currentScrollSpeed = normalScrollSpeed;
    }

    void Update()
    {
        if (healthScript != null && healthScript.IsDead())
        {
            if (isCharging || currentScrollSpeed > 0f) StopMapScrolling();
            return;
        }

        bool isUpsideDown = (gravityScript != null && gravityScript.IsTop());

        if (isUpsideDown)
        {
            isGrounded = Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, groundLayer);
        }
        else
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }

        // --- XỬ LÝ ANIMATION KHI TIẾP ĐẤT (LANDING) ---
        if (isGrounded && !wasGrounded)
        {
            if (anim != null)
            {
                anim.SetBool("isJumping", false); // Tắt animation nhảy, chuyển về chạy
            }
        }
        wasGrounded = isGrounded; // Cập nhật lại trạng thái ground cho khung hình sau

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
                Jump(isUpsideDown);
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
        float direction = upsideDown ? -1f : 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForceY * direction);

        float forwardBoost = currentJumpForceY * 0.5f;
        currentScrollSpeed = normalScrollSpeed + forwardBoost;
        isCharging = false;

        // --- XỬ LÝ ANIMATION KHI NHẢY (JUMP) ---
        if (anim != null)
        {
            anim.SetBool("isJumping", true); // Kích hoạt animation nhảy
        }
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