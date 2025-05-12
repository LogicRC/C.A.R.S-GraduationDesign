using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;  // 场景跳转所需
using System.Collections;

namespace CARS
{
    public class LoginButton : MonoBehaviour
    {
        public InputField emailInput;
        public InputField passwordInput;

        private string loginURL = "http://114.55.236.178:3000/login";

        public void OnLoginClick()
        {
            if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
            {
                Debug.LogError("❌ 邮箱和密码不能为空");
                return;
            }

            StartCoroutine(Login());
        }

        IEnumerator Login()
        {
            string email = emailInput.text;
            string password = passwordInput.text;

            string json = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            UnityWebRequest request = new UnityWebRequest(loginURL, "POST");
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
                Debug.Log("✅ 登录成功: " + request.downloadHandler.text);
                SceneManager.LoadScene("Menu");  // 登录成功后跳转到 Menu 场景
            }
            else
            {
                Debug.LogError("❌ 登录失败: " + request.error);
                Debug.LogError("❌ 返回内容: " + request.downloadHandler.text);
            }
        }
    }
}
