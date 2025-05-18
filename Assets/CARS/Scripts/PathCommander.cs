using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CARS
{
    // Another trouble. This script is used for path scene controllers
    public class PathCommander : MonoBehaviour
    {
        // Same statement game controller
        private EARcreatePath findObject;
        /// <summary>
        /// Declaration component, this is the starting point drop-down list
        /// </summary>
        private Dropdown loadJcombobox;
        /// <summary>
        /// Declaration component, this is the dropdown list of arrival points
        /// </summary>
        private Dropdown endJcombobox;
        /// <summary>
        /// Declaration component, this is a button
        /// </summary>
        public SelectButton prefab;
        /// <summary>
        /// Declaration component, this is a button container
        /// </summary>
        private Transform setPanel;
        /// <summary>
        /// Declaration components, some information used for display
        /// </summary>
        private Text info;
        /// <summary>
        /// Declaration component, used to display a list of key points
        /// </summary>
        private List<EARpointData> keyPoints;
        /// <summary>
        /// Declaration component, used to display selected objects
        /// </summary>
        private Transform selected;
        /// <summary>
        /// Declare a delete button
        /// </summary>
        private Button deleteKeyPoint;

        void Start()
        {
            // In this section, first fill in the dropdown list
            findObject = FindObjectOfType<EARcreatePath>();
            loadJcombobox = GameObject.Find("/Canvas/Panel/dpdStart").GetComponent<Dropdown>();
            endJcombobox = GameObject.Find("/Canvas/Panel/dpdArrival").GetComponent<Dropdown>();
            // Call the button to add a path
            setPanel = GameObject.Find("/Canvas/Panel/Scroll View/Viewport/Content").transform;
            info = GameObject.Find("/Canvas/Panel/Text").GetComponent<Text>();
            GameObject.Find("/Canvas/Panel/ButtonAdd").GetComponent<Button>().onClick.AddListener(NewPath);
            keyPoints = new List<EARpointData>();
            // Call the button to delete the path
            deleteKeyPoint = GameObject.Find("/Canvas/Panel/ButtonDelete").GetComponent<Button>();
            deleteKeyPoint.onClick.AddListener(DeleteRoad);
            deleteKeyPoint.interactable = false;
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
            var list = findObject.LoadRoads();
            foreach (var item in list)
            {
                var btn = Instantiate(prefab, setPanel);
                btn.path = JsonUtility.FromJson<RoadInformation>(item);
                btn.GetComponentInChildren<Text>().text = btn.path.startPointName + "<===>" + btn.path.endPointName;
            }
        }
        /// <summary>
        /// Used to save the path
        /// </summary>
        private void SaveRoads()
        {
            string[] jsons = new string[setPanel.childCount];
            for (int i = 0; i < setPanel.childCount; i++)
            {
                jsons[i] = JsonUtility.ToJson(setPanel.GetChild(i).GetComponent<SelectButton>().path);
            }
            findObject.SaveRoads(jsons);
            info.text = "Successfully saved";
        }
        /// <summary>
        /// Used to delete paths
        /// </summary>
        private void DeleteRoad()
        {
            Destroy(selected.gameObject);
            info.text = "Delete successfully";
            deleteKeyPoint.interactable = false;
        }
        /// <summary>
        /// This section is used for button clicking
        /// </summary>
        /// <param name="anySelected"></param>
        public void confirmSelection(Transform anySelected)
        {
            selected = anySelected;
            info.text = anySelected.GetComponentInChildren<Text>().text;
            deleteKeyPoint.interactable = true;
        }
        /// <summary>
        /// This is used to add paths
        /// </summary>
        private void NewPath()
        {
            var pathInfo = Instantiate(prefab, setPanel);

            pathInfo.path.startPointName = loadJcombobox.captionText.text;
            pathInfo.path.endPointName = endJcombobox.captionText.text;
            pathInfo.path.startPointPosition = GetPositionFORMname(pathInfo.path.startPointName);
            pathInfo.path.endPointPosition = GetPositionFORMname(pathInfo.path.endPointName);

            pathInfo.GetComponentInChildren<Text>().text = pathInfo.path.startPointName + "<===>" + pathInfo.path.endPointName;

            info.text = "Added successfully";
        }
        /// <summary>
        /// The following section is used to obtain coordinates based on keypoint names
        /// Technically speaking, it means generating a straight line between two coordinates
        /// </summary>
        /// <param name="anyPointName">Key point name</param>
        /// <returns>key point coordinate</returns>
        private Vector3 GetPositionFORMname(string anyPointName)
        {
            foreach (var anyPoint in keyPoints)
            {
                if (anyPoint.KeyPointName == anyPointName)
                {
                    return anyPoint.KeyPointPosition;
                }
            }
            return Vector3.zero;
        }
        /// <summary>
        /// Bind the corresponding dropdown list
        /// </summary>
        private void BindDropdown()
        {
            var list = findObject.LoadKeyPoins();

            foreach (var item in list)
            {
                EARpointData point = JsonUtility.FromJson<EARpointData>(item);
                keyPoints.Add(point);
                loadJcombobox.options.Add(new Dropdown.OptionData(point.KeyPointName));
                endJcombobox.options.Add(new Dropdown.OptionData(point.KeyPointName));
                loadJcombobox.captionText.text = loadJcombobox.options[0].text;
                endJcombobox.captionText.text = endJcombobox.options[0].text;
            }
        }
    }
}

