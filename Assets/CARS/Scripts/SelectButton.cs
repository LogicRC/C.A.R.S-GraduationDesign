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
        public EARpointData keyPoint;
        /// <summary>
        /// Declaration Path
        /// </summary>
        public RoadInformation path;
        /// <summary>
        /// Declaration of destination
        /// </summary>
        public Transform endPoint;

        void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Find("SceneMaster").SendMessage("SelectButtonClicked", transform);
            });
        }
    }
}

