using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

namespace CARS
{
    public class RegisterButton : MonoBehaviour
    {
        public InputField usernameInput;
        public InputField passwordInput;
        public InputField passwordAgainInput; // 新增确认密码
        public InputField fullNameInput;
        public InputField phoneInput;
        public InputField emailInput;

        private string registerURL = "http://114.55.236.178:3000/register";

        public void OnRegisterClick()
        {
            // 校验两次密码是否一致
            if (passwordInput.text != passwordAgainInput.text)
            {
                Debug.LogError("❌ 两次密码不一致");
                return;
            }

            StartCoroutine(Register());
        }

        IEnumerator Register()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            string fullName = fullNameInput.text;
            string phone = phoneInput.text;
            string email = emailInput.text;

            string json = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"fullName\":\"" + fullName + "\",\"phone\":\"" + phone + "\",\"email\":\"" + email + "\"}";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            UnityWebRequest request = new UnityWebRequest(registerURL, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log("✅ 注册成功: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ 注册失败: " + request.error);
            }
        }
    }
}
