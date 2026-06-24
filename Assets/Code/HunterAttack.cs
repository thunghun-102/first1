using UnityEngine;

public class HunterAttack : MonoBehaviour
{
    private Animator anim;
    private bool hasAttacked = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra nếu chạm trúng nhân vật chính (Mc có Tag là Player) và chưa tấn công lần nào
        if (other.CompareTag("Player") && !hasAttacked)
        {
            hasAttacked = true;

            // 🔍 DÒNG CHỮ KIỂM TRA: In ra màn hình Console để bạn theo dõi va chạm
            Debug.Log("Kỵ sĩ va chạm thành công với: " + other.name);

            // 2. Kỵ sĩ bật hoạt ảnh tấn công (Trigger "Attack")
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }

            // 3. Gọi sang đúng file code chết độc lập (SlimeHealth) gắn trên nhân vật Mc
            SlimeHealth slimeLife = other.GetComponent<SlimeHealth>();
            if (slimeLife != null)
            {
                slimeLife.TriggerGameOver();
            }
            else
            {
                // Cảnh báo nếu bạn quên chưa gắn script SlimeHealth vào nhân vật Mc
                Debug.LogError("LỖI: Chưa gắn Script 'SlimeHealth' vào nhân vật Mc (Slime)!");
            }
        }
    }
}
