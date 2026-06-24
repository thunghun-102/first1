using UnityEngine;

public class GravityRunner : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isTop;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool isCeiled;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        isCeiled = Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, groundLayer);

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(0))
        {
            if (isGrounded || isCeiled)
            {
                FlipGravity();
            }
        }
    }

    void FlipGravity()
    {
        rb.gravityScale = -rb.gravityScale;
        isTop = !isTop;

        Vector3 scaler = transform.localScale;
        scaler.y = isTop ? -Mathf.Abs(scaler.y) : Mathf.Abs(scaler.y); // Sửa lại một chút để tránh lỗi scale
        transform.localScale = scaler;
    }

    // HÀM BỔ SUNG: Để script nhảy kiểm tra xem nhân vật đang ở trên trần hay dưới đất
    public bool IsTop()
    {
        return isTop;
    }
}
