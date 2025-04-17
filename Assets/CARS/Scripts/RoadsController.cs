using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kuromu
{
    // Another trouble. This script is used for path scene controllers
    public class RoadsController : MonoBehaviour
    {
        // Same statement game controller
        private GameController gameController;
        /// <summary>
        /// Declaration component, this is the starting point drop-down list
        /// </summary>
        private Dropdown dpdStart;
        /// <summary>
        /// Declaration component, this is the dropdown list of arrival points
        /// </summary>
        private Dropdown dpdArrival;
        /// <summary>
        /// Declaration component, this is a button
        /// </summary>
        public SelectButton prefab;
        /// <summary>
        /// Declaration component, this is a button container
        /// </summary>
        private Transform svContent;
        /// <summary>
        /// Declaration components, some information used for display
        /// </summary>
        private Text info;
        /// <summary>
        /// Declaration component, used to display a list of key points
        /// </summary>
        private List<KeyPoint> keyPoints;
        /// <summary>
        /// Declaration component, used to display selected objects
        /// </summary>
        private Transform selected;
        /// <summary>
        /// Declare a delete button
        /// </summary>
        private Button btnDelete;

        void Start()
        {
            // In this section, first fill in the dropdown list
            gameController = FindObjectOfType<GameController>();
            dpdStart = GameObject.Find("/Canvas/Panel/dpdStart").GetComponent<Dropdown>();
            dpdArrival = GameObject.Find("/Canvas/Panel/dpdArrival").GetComponent<Dropdown>();
            // Call the button to add a path
            svContent = GameObject.Find("/Canvas/Panel/Scroll View/Viewport/Content").transform;
            info = GameObject.Find("/Canvas/Panel/Text").GetComponent<Text>();
            GameObject.Find("/Canvas/Panel/ButtonAdd").GetComponent<Button>().onClick.AddListener(AddRoad);
            keyPoints = new List<KeyPoint>();
            // Call the button to delete the path
            btnDelete = GameObject.Find("/Canvas/Panel/ButtonDelete").GetComponent<Button>();
            btnDelete.onClick.AddListener(DeleteRoad);
            btnDelete.interactable = false;
            // Call the button to save the path
            GameObject.Find("/Canvas/Panel/ButtonSave").GetComponent<Button>().onClick.AddListener(SaveRoads);

            BindDropdown();
            LoadRoad();
        }
        /// <summary>
        /// This method is used to add a path
        /// The two ends of the path are points, whether it is the destination point or the passing point
        /// </summary>
        private void LoadRoad()
        {
            var list = gameController.LoadRoads();
            foreach (var item in list)
            {
                var btn = Instantiate(prefab, svContent);
                btn.road = JsonUtility.FromJson<Road>(item);
                btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.arrivalName;
            }
        }
        /// <summary>
        /// Used to save the path
        /// </summary>
        private void SaveRoads()
        {
            string[] jsons = new string[svContent.childCount];
            for (int i = 0; i < svContent.childCount; i++)
            {
                jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().road);
            }
            gameController.SaveRoads(jsons);
            info.text = "Successfully saved";
        }
        /// <summary>
        /// Used to delete paths
        /// </summary>
        private void DeleteRoad()
        {
            Destroy(selected.gameObject);
            info.text = "Delete successfully";
            btnDelete.interactable = false;
        }
        /// <summary>
        /// This section is used for button clicking
        /// </summary>
        /// <param name="btnTF"></param>
        public void SelectButtonClicked(Transform btnTF)
        {
            selected = btnTF;
            info.text = btnTF.GetComponentInChildren<Text>().text;
            btnDelete.interactable = true;
        }
        /// <summary>
        /// This is used to add paths
        /// </summary>
        private void AddRoad()
        {
            var btn = Instantiate(prefab, svContent);

            btn.road.startName = dpdStart.captionText.text;
            btn.road.arrivalName = dpdArrival.captionText.text;
            btn.road.startPosition = GetPositionByName(btn.road.startName);
            btn.road.arrivalPosition = GetPositionByName(btn.road.arrivalName);

            btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.arrivalName;

            info.text = "Added successfully";
        }
        /// <summary>
        /// The following section is used to obtain coordinates based on keypoint names
        /// Technically speaking, it means generating a straight line between two coordinates
        /// </summary>
        /// <param name="pName">Key point name</param>
        /// <returns>key point coordinate</returns>
        private Vector3 GetPositionByName(string pName)
        {
            foreach (var kp in keyPoints)
            {
                if (kp.name == pName)
                {
                    return kp.position;
                }
            }
            return Vector3.zero;
        }
        /// <summary>
        /// Bind the corresponding dropdown list
        /// </summary>
        private void BindDropdown()
        {
            var list = gameController.LoadKeyPoins();

            foreach (var item in list)
            {
                KeyPoint point = JsonUtility.FromJson<KeyPoint>(item);
                keyPoints.Add(point);
                dpdStart.options.Add(new Dropdown.OptionData(point.name));
                dpdArrival.options.Add(new Dropdown.OptionData(point.name));
                dpdStart.captionText.text = dpdStart.options[0].text;
                dpdArrival.captionText.text = dpdArrival.options[0].text;
            }
        }
    }
}

