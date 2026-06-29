using UnityEngine;

public class TrapKill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem vật va chạm có Tag là Player (Nhân vật Mc) hay không
        if (other.CompareTag("Player"))
        {
            Debug.Log("Nhân vật Mc đã chạm trúng bẫy: " + gameObject.name);

            // Tự động tìm file code quản lý máu (SlimeHealth) đang gắn trên người Slime
            SlimeHealth slimeLife = other.GetComponent<SlimeHealth>();

            // Nếu tìm thấy, ra lệnh kích hoạt Game Over ngay lập tức
            if (slimeLife != null)
            {
                slimeLife.TriggerGameOver();
            }
            else
            {
                // Cảnh báo nếu script SlimeHealth nằm ở Object con hoặc chưa được gắn
                slimeLife = other.GetComponentInParent<SlimeHealth>();
                if (slimeLife != null)
                {
                    slimeLife.TriggerGameOver();
                }
                else
                {
                    Debug.LogError("LỖI: Không tìm thấy script 'SlimeHealth' trên nhân vật chính!");
                }
            }
        }
    }

    // Trường hợp bẫy của bạn không tích ô Is Trigger (là một khối cứng chặn đường)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SlimeHealth slimeLife = collision.gameObject.GetComponent<SlimeHealth>();
            if (slimeLife == null) slimeLife = collision.gameObject.GetComponentInParent<SlimeHealth>();

            if (slimeLife != null)
            {
                slimeLife.TriggerGameOver();
            }
        }
    }
}
