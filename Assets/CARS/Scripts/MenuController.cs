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
        private InputField inputField;
        /// <summary>
        /// Button for creating map
        /// </summary>
        private GameObject btnBuildMap;
        /// <summary>
        /// Button for delete map, but only local
        /// </summary>
        private GameObject btnDelete;
        /// <summary>
        /// GameController used to mount scripts
        /// </summary>
        private GameController gameController;


        void Start()
        {
            Transform canvas = GameObject.Find("/Canvas").transform;
            inputField = canvas.Find("InputField").GetComponent<InputField>();
            btnBuildMap = canvas.Find("ButtonBuildMap").gameObject;
            btnDelete = canvas.Find("ButtonDelete").gameObject;
            gameController = FindObjectOfType<GameController>();

            SetBuildUI();
        }
        /// <summary>
        /// This method for establishing a map
        /// </summary>
        public void BuildMap()
        {
            ///<summary>
            /// Check if the user has entered a map name
            /// but there is no pop-up prompt yet, it just won't run
            /// Mark here, maybe I will add it later
            ///</summary>
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
            ///<summary>
            /// Removes the given key from the PlayerPrefs
            /// The ID and Name in EasyAR PlayerPrefs will be deleted here
            /// But according to the settings of the EasyAR plugin, 
            /// the map data and ID will actually be uploaded to their server, 
            /// so only the local data has been deleted here (if you want to create a new map, etc.).
            ///</summary>
            PlayerPrefs.DeleteKey("MapID");
            PlayerPrefs.DeleteKey("MapName");
            ///<symmary>
            /// use the 'SetBuildUI' method again to reset UI data
            ///</symmary>
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

