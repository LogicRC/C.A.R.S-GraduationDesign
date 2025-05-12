using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CARS
{
    public class RegisterButton : MonoBehaviour
    {
        public InputField usernameInput;
        public InputField passwordInput;
        public InputField phoneInput;
        public InputField emailInput;
        public InputField fullnameInput;

        private string registerURL = "http://114.55.236.178:3000/users";

        public void OnRegisterClick()
        {
            if (string.IsNullOrEmpty(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
            {
                Debug.LogError("❌ 用户名或密码不能为空");
                return;
            }

            StartCoroutine(Register());
        }

        IEnumerator Register()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            string phone = phoneInput.text;
            string email = emailInput.text;
            string fullname = fullnameInput.text;

            string json = $"{{\"username\":\"{username}\",\"password\":\"{password}\",\"phone_number\":\"{phone}\",\"email\":\"{email}\",\"full_name\":\"{fullname}\"}}";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            UnityWebRequest request = new UnityWebRequest(registerURL, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#else
            if (!request.isNetworkError && !request.isHttpError)
#endif
            {
                Debug.Log("✅ 注册成功: " + request.downloadHandler.text);
                SceneManager.LoadScene("Login");  // 注册成功后跳转到登录场景
            }
            else
            {
                Debug.LogError("❌ 注册失败: " + request.error);
                Debug.LogError("❌ 返回内容: " + request.downloadHandler.text);
            }
        }
    }
}
