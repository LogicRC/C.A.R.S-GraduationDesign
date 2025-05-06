using UnityEngine;
using UnityEngine.SceneManagement;

namespace CARS
{
    /// <summary>
    /// This script contains some common features, currently only...
    /// Switching scenes
    /// </summary>
    public class Common : MonoBehaviour
    {
        /// <summary>
        /// This method is used to load scenes
        /// based on the scene name added during the call, its quite simple, isnt it
        /// </summary>
        /// <param name="sceneName">Enter the scene name when calling</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }        
    }
}

