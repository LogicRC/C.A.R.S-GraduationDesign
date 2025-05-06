using UnityEngine;
using UnityEngine.UI;

namespace CARS
{
    /// <summary>
    /// This script is used to define click buttons in a scrolling view
    /// </summary>
    public class SelectButton : MonoBehaviour
    {
        /// <summary>
        /// Declaration key points
        /// </summary>
        public KeyPoint keyPoint;
        /// <summary>
        /// Declaration Path
        /// </summary>
        public Road road;
        /// <summary>
        /// Declaration of destination
        /// </summary>
        public Transform arrival;

        void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Find("SceneMaster").SendMessage("SelectButtonClicked", transform);
            });
        }
    }
}

