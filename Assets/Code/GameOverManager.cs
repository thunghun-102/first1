using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Bắt buộc phải có để chuyển cảnh/chơi lại

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel; // Kéo thả Canvas/Panel Game Over vào đây

    [Header("Score Texts")]
    public TextMeshProUGUI currentScoreText; // Text hiển thị điểm lượt này
    public TextMeshProUGUI highScoreText;    // Text hiển thị điểm kỷ lục

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        // Ẩn bảng Game Over khi mới vào màn chơi
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Hàm này sẽ được gọi từ bên SlimeHealth sang khi Slime chết
    public void ShowGameOverScreen()
    {
        // Tự động đi tìm cái bảng mang tên "GameOverPanel" dưới Canvas nếu quên kéo thả
        if (gameOverPanel == null)
        {
            Transform foundPanel = transform.parent != null ?
                transform.parent.Find("Canvas/GameOverPanel") :
                GameObject.Find("Canvas")?.transform.Find("GameOverPanel");

            if (foundPanel != null) gameOverPanel = foundPanel.gameObject;
        }

        if (gameOverPanel == null)
        {
            Debug.LogError("LỖI KHÔNG TÌM THẤY: Bạn chưa đặt tên đúng là 'GameOverPanel' ở bảng Hierarchy!");
            return;
        }

        // 1. Hiển thị bảng điểm lên màn hình
        gameOverPanel.SetActive(true);

        // 2. Lấy số điểm hiện tại từ CoinManager
        int currentScore = 0;
        if (CoinManager.instance != null)
        {
            currentScore = CoinManager.instance.GetCoinCount();
        }

        // 3. Lấy điểm HighScore đã lưu từ trước
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        // 4. So sánh phá kỷ lục
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        // 5. Cập nhật chữ lên bảng điểm
        if (currentScoreText != null) currentScoreText.text = "SCORE: " + currentScore;
        if (highScoreText != null) highScoreText.text = "HIGH SCORE: " + highScore;
    }

    // 🔥 ĐÂY LÀ HÀM BẠN ĐANG BỊ THIẾU: Gắn vào Nút Replay để chơi lại từ đầu
    public void ReplayGame()
    {
        // Tải lại Scene hiện tại đang chơi
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
