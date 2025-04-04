using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Common : MonoBehaviour
    {

    /// <summary>
    /// º«‘ÿ≥°æ∞
    /// </summary>

       public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
