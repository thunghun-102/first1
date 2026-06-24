using UnityEngine;

public class CoinItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem vật thể chạm vào đồng xu có phải là nhân vật Slime không
        // (Bắt buộc Object 'Mc' của bạn phải được đặt Tag là "Player")
        if (other.CompareTag("Player"))
        {
            // Gọi hàm cộng 1 điểm từ CoinManager
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddCoin(1);
            }

            // Xóa đồng xu khỏi màn hình game sau khi ăn thành công
            Destroy(gameObject);
        }
    }
}
