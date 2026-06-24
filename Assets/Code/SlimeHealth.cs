using UnityEngine;

public class SlimeHealth : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SlimeScrollJump jumpScript; // Gọi sang script nhảy để báo dừng map
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpScript = GetComponent<SlimeScrollJump>(); // Tự động kết nối với script nhảy chung Object
    }

    // Hàm này sẽ được gọi từ bên Kỵ sĩ sang khi va chạm xảy ra
    public void TriggerGameOver()
    {
        if (isDead) return;
        isDead = true;

        // 1. Gọi sang bên script nhảy để bắt ép tốc độ cuộn bản đồ về 0
        if (jumpScript != null)
        {
            jumpScript.StopMapScrolling();
        }

        // 2. Kích hoạt hoạt ảnh MCdie trong Animator của Slime
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 3. Khóa cứng tọa độ đứng im trên đất, không lo bị rơi xuyên lòng đất
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        Debug.Log("Slime đã chết! Xử lý hoàn toàn bên script SlimeHealth.");


        //them
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.ShowGameOverScreen();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void OnBecameInvisible()
    {
        // Kiểm tra nếu trước đó nhân vật chưa chết thì mới xử lý chết
        if (!IsDead())
        {
            Debug.Log("Mc đã rơi/rời khỏi màn hình Camera! Kích hoạt GameOver.");

            // Gọi chính hàm xử lý chết có sẵn trong script SlimeHealth của bạn
            TriggerGameOver();
        }
    }

}
