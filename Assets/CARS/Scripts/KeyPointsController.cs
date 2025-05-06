using UnityEngine;
using UnityEngine.UI;
using easyar;

namespace CARS
{
    //This script is used to implement scene control of key points
    public class KeyPointsController : MonoBehaviour
    {
        /// <summary>
        /// Declaration Key Canvas
        /// </summary>
        private GameObject panel;
        /// <summary>
        /// Declaration Information Text Box
        /// </summary>
        private Text info;
        /// <summary>
        /// Declare the selected object
        /// A Transform component used to reference a game object within a script.
        /// By reasonable initialization (manual drag and drop or code retrieval), 
        /// functions such as selecting objects and operating positions can be achieved
        /// 
        /// When starting keypoint scanning, 
        /// the application will display a red square in AR form to prompt the user when it recognizes the previously set beacon (such as a QR code).
        /// And when the user clicks on this beacon, the method of recording location will be activated.
        /// </summary>
        private Transform selected;
        /// <summary>
        /// Declaration of Rolling View Container
        /// </summary>
        private Transform svContent;
        /// <summary>
        /// Call the input box to enter the name of the key point
        /// </summary>
        private InputField inputField;
        /// <summary>
        /// Used for type selection, waypoints or destinations, waypoints cannot be set as navigation targets
        /// </summary>
        private Dropdown dropdown;
        /// <summary>
        /// Used to call key button preforms
        /// </summary>
        public SelectButton prefab;
        /// <summary>
        /// Just add button!
        /// </summary>
        private Button btnAdd;
        /// <summary>
        /// Just delete button
        /// </summary>
        private Button btnDelete;
        /// <summary>
        /// Declare calling game control components
        /// </summary>
        private GameController gameController;
        private ARSession session;
        private SparseSpatialMapWorkerFrameFilter mapWorker;
        private SparseSpatialMapController map;
        /// <summary>
        /// Used to check the localization status of sparse maps
        /// </summary>
        private bool localized = false;

        void Start()
        {
            // The following code is used to initialize and implement interface control
            panel = GameObject.Find("/Canvas/Panel");
            panel.transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
            {
                HiddenPanel();
            });
            info = panel.transform.Find("Text").GetComponent<Text>();

            svContent = panel.transform.Find("Scroll View/Viewport/Content");
            inputField = panel.transform.Find("InputField").GetComponent<InputField>();
            dropdown = panel.transform.Find("Dropdown").GetComponent<Dropdown>();
            btnAdd = panel.transform.Find("ButtonAdd").GetComponent<Button>();
            btnAdd.onClick.AddListener(AddKeyPoint);
            btnAdd.interactable = false;

            btnDelete = panel.transform.Find("ButtonDelete").GetComponent<Button>();
            btnDelete.onClick.AddListener(DeleteKeyPoint);
            btnDelete.interactable = false;

            panel.transform.Find("ButtonSave").GetComponent<Button>().onClick.AddListener(SaveKeyPoints);
            gameController = FindObjectOfType<GameController>();

            session = FindObjectOfType<ARSession>();
            mapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
            map = FindObjectOfType<SparseSpatialMapController>();

            LoadKeyPoints();
            HiddenPanel();
            LoadMap();
        }
        /// <summary>
        /// The following code is used to load sparse maps, 
        /// but the data is actually downloaded from EasyAR's server
        /// </summary>
        private void LoadMap()
        {
            // Call the method in EasyAR plugin to set the map
            // Retrieve specified map data based on MapID (actually EasyAR's map API) and map name
            map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
            map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
            // Set feedback for map acquisition
            map.MapLoad += (map, status, error) =>
            {
                if (status)
                {
                    localized = true;
                    gameController.SendMessage("ShowMessage", "Map loaded successfully");
                }
                else
                {
                    gameController.SendMessage("ShowMessage", "Map loading failed:" + error);
                }
            };
            // Set successful positioning event prompt
            map.MapLocalized += () =>
            {
                gameController.SendMessage("ShowMessage", "Successfully entered sparse space localization");
            };
            // Set stop location event prompt
            map.MapStopLocalize += () =>
            {
                gameController.SendMessage("ShowMessage", "Stop sparse space localization");
            };
            gameController.SendMessage("ShowMessage", "Start loading the map");
            mapWorker.Localizer.startLocalization();    // Call the method in EasyAR plugin to start localizing the map
        }
        #region This code is used to implement keypoint control
        /// <summary>
        /// Start loading key points
        /// </summary>
        private void LoadKeyPoints()
        {
            var list = gameController.LoadKeyPoins();
            foreach (var item in list)
            {
                SelectButton btn = Instantiate(prefab, svContent);
                btn.keyPoint = JsonUtility.FromJson<KeyPoint>(item);
                btn.GetComponentInChildren<Text>().text = btn.keyPoint.name;
            }
        }
        /// <summary>
        /// Start saving key points
        /// </summary>
        private void SaveKeyPoints()
        {
            string[] jsons = new string[svContent.childCount];
            for (int i = 0; i < svContent.childCount; i++)
            {
                jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().keyPoint);
            }
            gameController.SaveKeyPoints(jsons);
            info.text = "Save completed";
        }
        /// <summary>
        /// Delete key points

        /// </summary>
        private void DeleteKeyPoint()
        {
            Destroy(selected.gameObject);
            info.text = "The deletion was successful";
            btnDelete.interactable = false;
        }
        /// <summary>
        /// Button click
        /// </summary>
        /// <param name="btnTF"></param>
        public void SelectButtonClicked(Transform btnTF)
        {
            selected = btnTF;
            info.text = btnTF.GetComponentInChildren<Text>().text;
            btnDelete.interactable = true;
            btnAdd.interactable = false;
        }
        /// <summary>
        /// The following method is used to add key points
        /// </summary>
        private void AddKeyPoint()
        {
            if (!string.IsNullOrEmpty(inputField.text) && selected != null)
            {
                SelectButton btn = Instantiate(prefab, svContent);

                btn.keyPoint.name = inputField.text;
                btn.keyPoint.position = selected.localPosition;
                btn.keyPoint.pointType = dropdown.value;

                btn.GetComponentInChildren<Text>().text = inputField.text;

                inputField.text = "";
                selected = null;
                info.text = "Added successfully";
                btnAdd.interactable = false;
            }
        }
        #endregion

        #region This code is used to implement interface control
        /// <summary>
        /// Hide Canvas
        /// Because after clicking on the key beacon, a new canvas will appear
        /// This includes buttons for editing key point names, etc
        /// </summary>
        private void HiddenPanel()
        {
            panel.SetActive(false);
            info.text = "";
        }
        /// <summary>
        /// Show Canvas
        /// </summary>
        private void ShowPanel()
        {
            panel.SetActive(true);
            info.text = "Position:" + selected.localPosition;
        }
        #endregion

        #region  The following code is used to implement 'when the user clicks on a key point'
        void Update()
        {
            if (Input.GetMouseButtonUp(0) && localized)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    selected = hit.transform;
                    btnAdd.interactable = true;
                    HitObject();
                    ShowPanel();
                }
            }
        }
        /// <summary>
        /// When the user clicks on a key point, start saving the location, etc
        /// </summary>
        private void HitObject()
        {
            var tf = new GameObject().transform;
            tf.position = selected.position;
            tf.parent = map.transform;
            selected = tf;
        }
        #endregion
    }
}

