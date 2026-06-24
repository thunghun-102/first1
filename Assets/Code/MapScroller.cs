using UnityEngine;

public class MapScroller : MonoBehaviour
{
    // Kéo thả nhân vật Slime (Mc) vào 2 ô trống này trong Inspector
    public SlimeScrollJump slimeJump;
    public SlimeHealth slimeHealth;

    void Update()
    {
        // 1. Kiểm tra xem Slime đã chết chưa (gọi sang file SlimeHealth)
        if (slimeHealth != null && slimeHealth.IsDead())
        {
            // Nếu Slime chết, lập tức ép tốc độ cuộn về 0 và dừng hẳn map lại
            return;
        }

        // 2. Nếu Slime còn sống, cho map trượt đi theo tốc độ của Slime quy định
        if (slimeJump != null)
        {
            float speed = slimeJump.currentScrollSpeed;
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else
        {
            // Phòng hờ nếu bạn quên chưa kéo thả Slime, map vẫn tự chạy đều tốc độ 6
            transform.Translate(Vector3.left * 6f * Time.deltaTime);
        }
    }
}
