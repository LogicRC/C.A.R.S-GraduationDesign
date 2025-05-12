using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace CARS
{
    /// <summary>
    /// the following code is the scene controller for the main menu
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        /// <summary>
        /// Waiting for user to input map name
        /// </summary>
        public InputField inputField; // 现在用 Inspector 拖拽引用
        /// <summary>
        /// Button for creating map
        /// </summary>
        public GameObject btnBuildMap;
        /// <summary>
        /// Button for delete map, but only local
        /// </summary>
        public GameObject btnDelete;
        /// <summary>
        /// GameController used to mount scripts
        /// </summary>
        private GameController gameController;

        void Start()
        {
            gameController = FindObjectOfType<GameController>();

            if (inputField == null || btnBuildMap == null || btnDelete == null)
            {
                Debug.LogError("❌ UI 元素未正确绑定，请在 Inspector 中拖拽 InputField、ButtonBuildMap、ButtonDelete");
                return;
            }

            SetBuildUI();
        }

        /// <summary>
        /// This method for establishing a map
        /// </summary>
        public void BuildMap()
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                gameController.inputName = inputField.text;
                SceneManager.LoadScene("BuildMap");
            }
        }

        /// <summary>
        /// Method to delete map button
        /// </summary>
        public void DeleteMap()
        {
            PlayerPrefs.DeleteKey("MapID");
            PlayerPrefs.DeleteKey("MapName");
            SetBuildUI();
        }

        /// <summary>
        /// this method is used to set the relevant UI when creating a map
        /// </summary>
        private void SetBuildUI()
        {
            inputField.text = PlayerPrefs.GetString("MapName");
            bool status = string.IsNullOrEmpty(inputField.text);

            inputField.interactable = status;
            btnBuildMap.SetActive(status);
            btnDelete.SetActive(!status);
        }

        /// <summary>
        /// Just used to close the application
        /// </summary>
        public void Exit()
        {
            Application.Quit();
        }
    }
}
