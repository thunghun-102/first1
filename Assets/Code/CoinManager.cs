using UnityEngine;
using TMPro; // Thêm dòng này để điều khiển TextMeshPro

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance; // Tạo bản mẫu để các script khác dễ gọi đến

    public TextMeshProUGUI coinText; // Kéo thả Object CoinText vào đây trong Inspector
    private int coinCount = 0;       // Biến lưu số lượng xu

    void Awake()
    {
        // Thiết lập Singleton để dễ truy cập từ script Đồng Xu
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        UpdateCoinUI();
    }

    // Hàm cộng thêm xu khi ăn được
    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI(); // Cập nhật lại số hiển thị trên màn hình
    }

    // Hàm cập nhật giao diện chữ
    void UpdateCoinUI()
    {
        coinText.text = "" + coinCount;
    }
    public int GetCoinCount()
    {
        return coinCount;
    }
}
