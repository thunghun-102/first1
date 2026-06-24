using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class AccountManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject registerPanel;
    public GameObject loginPanel;
    public GameObject successPanel;

    [Header("Register Inputs")]
    public TMP_InputField regUsername;
    public TMP_InputField regPassword;
    public TMP_InputField regEmail;
    public TMP_InputField regCharName;
    public TMP_InputField regPhone;
    public TextMeshProUGUI regErrorText;

    [Header("Login Inputs")]
    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;
    public TextMeshProUGUI loginErrorText;

    [Header("Success Panel Elements")]
    public TextMeshProUGUI successInfoText;

    private string filePath;

    void Start()
    {
        // Đường dẫn lưu file account.txt trong thư mục dự án
        filePath = Path.Combine(Application.persistentDataPath, "account.txt");
        Debug.Log("Đường dẫn file account: " + filePath);

        // --- 1. CÀI ĐẶT CHỮ GỢI Ý MỜ (PLACEHOLDER) ---
        // Gọi hàm bổ trợ bên dưới để tự động điền chữ mờ cho từng ô
        SetPlaceholderText(regUsername, "Username");
        SetPlaceholderText(regPassword, "Password");
        SetPlaceholderText(regEmail, "Email");
        SetPlaceholderText(regCharName, "Character Name");
        SetPlaceholderText(regPhone, "Mobile Phone");

        SetPlaceholderText(loginUsername, "Username");
        SetPlaceholderText(loginPassword, "Password");

        // --- 2. CÀI ĐẶT ẨN MẬT KHẨU (DẤU *) ---
        if (regPassword != null)
        {
            regPassword.contentType = TMP_InputField.ContentType.Password;
            regPassword.ForceLabelUpdate(); // Ép Unity cập nhật giao diện ngay lập tức
        }
        if (loginPassword != null)
        {
            loginPassword.contentType = TMP_InputField.ContentType.Password;
            loginPassword.ForceLabelUpdate();
        }

        // Khởi tạo màn hình ban đầu
        ShowLoginPanel();
    }

    // Hàm bổ trợ: Tìm Component "Placeholder" có sẵn trong TMP_InputField và đổi chữ
    private void SetPlaceholderText(TMP_InputField inputField, string textToSet)
    {
        if (inputField != null && inputField.placeholder != null)
        {
            TextMeshProUGUI placeholderComponent = inputField.placeholder.GetComponent<TextMeshProUGUI>();
            if (placeholderComponent != null)
            {
                placeholderComponent.text = textToSet;
            }
        }
    }

    #region Chuyển đổi màn hình
    public void ShowRegisterPanel()
    {
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
        successPanel.SetActive(false);
        ClearRegisterInputs();
    }

    public void ShowLoginPanel()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        successPanel.SetActive(false);
        ClearLoginInputs();
    }

    public void CloseSuccessPanel()
    {
        successPanel.SetActive(false);
    }
    #endregion

    #region Xử lý Đăng Ký
    public void OnRegisterClick()
    {
        regErrorText.text = "";

        string username = regUsername.text.Trim();
        string password = regPassword.text; // Giữ nguyên khoảng trắng nếu pass có
        string email = regEmail.text.Trim();
        string charName = regCharName.text.Trim();
        string phone = regPhone.text.Trim();

        // 1. Kiểm tra không được để trống
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(charName) || string.IsNullOrEmpty(phone))
        {
            regErrorText.text = "Vui lòng nhập đầy đủ thông tin!";
            return;
        }

        // 2. Validate Username: chỉ viết thường và số, độ dài <= 20
        if (!Regex.IsMatch(username, @"^[a-z0-9]{1,20}$"))
        {
            regErrorText.text = "Username chỉ gồm chữ viết thường, số và không quá 20 ký tự!";
            return;
        }

        // 3. Validate Password: chữ, số, tối thiểu 1 ký tự đặc biệt (@#$%), dài 6-20 ký tự
        // Sử dụng Regex Lookahead để đảm bảo chứa ít nhất 1 ký tự đặc biệt thuộc tập [@#$%]
        if (password.Length < 6 || password.Length > 20 || !Regex.IsMatch(password, @"(?=.*[@#$%])"))
        {
            regErrorText.text = "Password dài 6-20 ký tự và chứa ít nhất 1 ký tự đặc biệt (@#$%)!";
            return;
        }

        // 4. Validate Email
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            regErrorText.text = "Định dạng Email không hợp lệ!";
            return;
        }

        // 5. Validate Character Name: không dài quá 15 ký tự
        if (charName.Length > 15)
        {
            regErrorText.text = "Character name không được dài quá 15 ký tự!";
            return;
        }

        // 6. Validate Số điện thoại di động VN
        // Các đầu số di động VN hiện tại: 03, 05, 07, 08, 09 kèm theo 8 chữ số phía sau.
        if (!Regex.IsMatch(phone, @"^(03|05|07|08|09)\d{8}$"))
        {
            regErrorText.text = "Số điện thoại di động Việt Nam không hợp lệ!";
            return;
        }

        // Kiểm tra xem trùng Username cũ trong file không (Tránh trùng lặp)
        if (IsUsernameExist(username))
        {
            regErrorText.text = "Tài khoản này đã tồn tại!";
            return;
        }

        // Nếu tất cả hợp lệ -> Tiến hành lưu vào file account.txt
        SaveAccountToFile(username, password, email, charName, phone);

        // Chuyển sang màn hình đăng nhập
        ShowLoginPanel();
    }

    private void SaveAccountToFile(string user, string pass, string email, string charName, string phone)
    {
        // Định dạng dữ liệu cách nhau bằng dấu Tab (\t)
        string accountData = $"{user}\t{pass}\t{email}\t{charName}\t{phone}";

        // StreamWriter với tham số 'true' sẽ tự động tạo file nếu chưa có và ghi nối tiếp (append) vào cuối file
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine(accountData);
        }
    }

    private bool IsUsernameExist(string username)
    {
        if (!File.Exists(filePath)) return false;

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] data = line.Split('\t');
            if (data.Length > 0 && data[0].Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Xử lý Đăng Nhập
    public void OnLoginClick()
    {
        loginErrorText.text = "";
        string user = loginUsername.text.Trim();
        string pass = loginPassword.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            loginErrorText.text = "Vui lòng nhập đầy đủ Username và Password!";
            return;
        }

        if (!File.Exists(filePath))
        {
            loginErrorText.text = "Tài khoản hoặc mật khẩu không chính xác! (File dữ liệu trống)";
            return;
        }

        // Đọc từng dòng để kiểm tra thông tin đăng nhập
        string[] lines = File.ReadAllLines(filePath);
        bool isSuccess = false;

        foreach (string line in lines)
        {
            // Tách dữ liệu bằng dấu tab
            string[] data = line.Split('\t');

            // Đảm bảo dòng có đủ cấu trúc 5 phần tử
            if (data.Length >= 5)
            {
                string fileUser = data[0];
                string filePass = data[1];

                if (fileUser == user && filePass == pass)
                {
                    isSuccess = true;
                    DisplaySuccessPanel(data);
                    break;
                }
            }
        }

        if (!isSuccess)
        {
            loginErrorText.text = "Tài khoản hoặc mật khẩu không chính xác!";
        }
    }

    private void DisplaySuccessPanel(string[] accountData)
    {
        successPanel.SetActive(true);

        // Hiển thị toàn bộ thông tin lên panel thông báo thành công
        successInfoText.text = $"<b>ĐĂNG NHẬP THÀNH CÔNG!</b>\n\n" +
                               $"<b>Username:</b> {accountData[0]}\n" +
                               $"<b>Email:</b> {accountData[2]}\n" +
                               $"<b>Character Name:</b> {accountData[3]}\n" + $"<b>Phone:</b> {accountData[4]}";
    }
    #endregion

    #region Helper dọn dẹp Input
    private void ClearRegisterInputs()
    {
        regUsername.text = "";
        regPassword.text = "";
        regEmail.text = "";
        regCharName.text = "";
        regPhone.text = "";
        regErrorText.text = "";
    }

    private void ClearLoginInputs()
    {
        loginUsername.text = "";
        loginPassword.text = "";
        loginErrorText.text = "";
    }
    #endregion
}
