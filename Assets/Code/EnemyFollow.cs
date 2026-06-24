using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform playerTransform; // Kéo thả nhân vật Mc vào đây

    [Header("Khoảng cách rượt đuổi")]
    public float targetDistanceX = 12f; // Khoảng cách Kỵ sĩ muốn lùi tới (12m là mất hút khỏi Camera)

    [Header("Tốc độ lùi")]
    public float retreatSpeed = 1f;     // Tốc độ Kỵ sĩ từ từ đi lùi về phía sau

    private float currentDistanceX;

    void Start()
    {
        // Tự động tìm nhân vật nếu quên kéo thả trong Inspector
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        if (playerTransform != null)
        {
            // NGAY KHI BẮT ĐẦU: Tính khoảng cách thực tế hiện tại giữa Kỵ sĩ và Slime
            currentDistanceX = playerTransform.position.x - transform.position.x;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // TỪ TỪ TĂNG KHOẢNG CÁCH: Ép Kỵ sĩ phải lùi dần ra xa Slime theo thời gian
        currentDistanceX = Mathf.MoveTowards(currentDistanceX, targetDistanceX, retreatSpeed * Time.deltaTime);

        // KHÓA TỌA ĐỘ TRỤC X: Đẩy Kỵ sĩ lùi về bên trái dựa theo khoảng cách đang tăng dần
        float targetX = playerTransform.position.x - currentDistanceX;

        // Giữ nguyên cao độ Y hiện tại để trọng lực Rigidbody 2D tự xử lý trên mặt đất
        float targetY = transform.position.y;

        // Cập nhật vị trí mới cho Kỵ sĩ
        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}
