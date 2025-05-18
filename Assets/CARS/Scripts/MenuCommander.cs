using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace CARS
{
    /// <summary>
    /// the following code is the scene controller for the main menu
    /// </summary>
    public class MenuCommander : MonoBehaviour
    {
        /// <summary>
        /// Waiting for user to input map name
        /// </summary>
        public InputField inputbox; // 现在用 Inspector 拖拽引用
        /// <summary>
        /// Button for creating map
        /// </summary>
        public GameObject createMap;
        /// <summary>
        /// Button for delete map, but only local
        /// </summary>
        public GameObject deleteMap;
        /// <summary>
        /// GameController used to mount scripts
        /// </summary>
        private EARcreatePath findObject;

        void Start()
        {
            /// <summary>
            /// Check if necessary components exist
            /// </summary>
            findObject = FindObjectOfType<EARcreatePath>();

            if (inputbox == null || createMap == null || deleteMap == null)
            {
                Debug.LogError("Error!!! UI element not properly bound, please drag and drop InputField, Buttons Build Map, Buttons Delete in Inspector");
                return;
            }

            SetBuildUI();
        }

        /// <summary>
        /// This method for establishing a map
        /// </summary>
        public void BuildMap()
        {
            if (!string.IsNullOrEmpty(inputbox.text))
            {
                findObject.inputName = inputbox.text;
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
            inputbox.text = PlayerPrefs.GetString("MapName");
            bool status = string.IsNullOrEmpty(inputbox.text);

            inputbox.interactable = status;
            createMap.SetActive(status);
            deleteMap.SetActive(!status);
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
