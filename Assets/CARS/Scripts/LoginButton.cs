using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

namespace CARS
{
    public class LoginButton : MonoBehaviour
    {
        public InputField emailInput;
        public InputField passwordInput;
        public Text resultText;

        private string loginURL = "http://114.55.236.178:3000/login";

        public void OnLoginClick()
        {
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

            if (!request.isNetworkError && !request.isHttpError)
            {
                resultText.text = "✅ 登录成功";
                Debug.Log("Login Success: " + request.downloadHandler.text);
            }
            else
            {
                resultText.text = "❌ 登录失败: " + request.error;
                Debug.LogError("Login Error: " + request.error);
            }
        }
    }
}
