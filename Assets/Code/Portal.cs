using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Cài đặt Cổng")]
    public Transform destination; // Kéo thả cổng đích (cổng kia) vào đây
    public float cooldownTime = 0.5f; // Thời gian chờ trước khi cổng này có thể hoạt động lại

    private float lastTeleportTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ dịch chuyển nếu đối tượng chạm vào có tag là "Player"
        if (other.CompareTag("Player"))
        {
            // Kiểm tra xem cổng này có đang trong thời gian hồi (cooldown) không
            if (Time.time > lastTeleportTime)
            {
                // Lấy script Portal của cổng đích để vô hiệu hóa nó tạm thời
                Portal destPortal = destination.GetComponent<Portal>();
                if (destPortal != null)
                {
                    destPortal.lastTeleportTime = Time.time + cooldownTime;
                }

                // Dịch chuyển nhân vật sang vị trí của cổng đích
                other.transform.position = destination.position;

                // (Tùy chọn) Thêm hiệu ứng âm thanh hoặc Particle System ở đây
                Debug.Log("Đã dịch chuyển!");
            }
        }
    }
}