using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using easyar;

namespace CARS
{
    /// <summary>
    /// This script is used to control AR navigation scenes
    /// </summary>
    public class EARnavigation : MonoBehaviour
    {
        /// <summary>
        /// Used to obtain control over a component
        /// In the following code, it is used to obtain error prompt boxes
        /// </summary>
        private EARcreatePath findObject;
        /// <summary>
        /// Declare the navigation canvas in the scene
        /// </summary>
        private GameObject ARpanel;
        /// <summary>
        /// Declare navigation buttons in the scene
        /// </summary>
        private Button startNavigation;
        /// <summary>
        /// Declare navigation buttons in the scene
        /// </summary>
        public SelectButton findPrefab;
        /// <summary>
        /// Declare the navigation button container in the scene
        /// </summary>
        private Transform navigationPanel;
        /// <summary>
        /// Declare the navigation root node in the scene
        /// </summary>
        public Transform navigationRoot;
        /// <summary>
        /// Declaration of destination prefabricated parts in the scenario
        /// </summary>
        public Transform endPoint;
        /// <summary>
        /// Declare path prefabricated components in the scene
        /// </summary>
        public Transform importPath;
        /// <summary>
        /// Declare the navigation lines in the scene
        /// </summary>
        private LineRenderer setLine;
        /// <summary>
        /// Declare the navigation agent in the scene
        /// </summary>
        private NavMeshAgent NavAgent;
        /// <summary>
        /// Declare the navigation path in the scene
        /// </summary>
        private NavMeshPath NavPath;
        private NavMeshSurface NavSurface;
        /// <summary>
        /// Declare the navigation target in the scene
        /// </summary>
        private Transform setEndPoint;
        /// <summary>
        /// Declare the user's location to achieve navigation effects that follow the user's movements
        /// </summary>
        public Transform SetUserPosition;

        private ARSession basicSession;
        private SparseSpatialMapWorkerFrameFilter EARmapWorker;
        private SparseSpatialMapController EARmap;
        void Start()
        {
            findObject = FindObjectOfType<EARcreatePath>();
            ARpanel = GameObject.Find("/Canvas/Panel");
            startNavigation = GameObject.Find("/Canvas/ButtonNav").GetComponent<Button>();
            startNavigation.onClick.AddListener(ShowNavUI);
            startNavigation.interactable = false;
            ARpanel.transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(CloseNavUI);

            navigationPanel = ARpanel.transform.Find("Scroll View/Viewport/Content").transform;

            basicSession = FindObjectOfType<ARSession>();
            EARmapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
            EARmap = FindObjectOfType<SparseSpatialMapController>();

            SetLine();
            CloseNavUI();
            LoadMap();
        }

        /// <summary>
        /// Oh, of course, the first step is to set up the map part
        /// </summary>
        private void LoadMap()
        {
            //Set map name and ID
            EARmap.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
            EARmap.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
            //Set feedback on map loading, success or failure
            EARmap.MapLoad += (map, status, error) =>
            {
                if (status)
                {
                    findObject.SendMessage("ShowMessage", "Map loaded successfully");
                }
                else
                {
                    findObject.SendMessage("ShowMessage", "Map loading failed:" + error);
                }
            };
            // Set successful positioning event prompt
            EARmap.MapLocalized += () =>
            {
                findObject.SendMessage("ShowMessage", "Successfully entered sparse space localization");
                ClearNav();
                LoadArrivals();
                LoadRoads();
                BakePath();
                startNavigation.interactable = true;
                ShowNavUI();
            };
            // Set stop location event prompt
            EARmap.MapStopLocalize += () =>
            {
                findObject.SendMessage("ShowMessage", "Stop sparse space localization");
            };
            findObject.SendMessage("ShowMessage", "Start loading the EARmap");
            EARmapWorker.Localizer.startLocalization();    // Call the method in EasyAR plugin to start localizing the map
        }
        /// <summary>
        /// Clean up navigation elements
        /// Otherwise, catastrophic multiple navigation overlays may occur
        /// Of course we need to delete it, right?
        /// </summary>
        private void ClearNav()
        {
            // delete Button
            foreach (Transform tf in navigationPanel)
            {
                Destroy(tf.gameObject);
            }
            // Delete destination
            foreach (Transform tf in navigationRoot.Find("Arrivals"))
            {
                Destroy(tf.gameObject);
            }
            // delete path
            foreach (Transform tf in navigationRoot.Find("Roads"))
            {
                Destroy(tf.gameObject);
            }
        }
        /// <summary>
        /// This method is used to implement button clicking
        /// </summary>
        /// <param name="btnTF"></param>
        public void SelectButtonClicked(Transform btnTF)
        {
            CancelInvoke("DisplayPath");
            setEndPoint = btnTF.GetComponent<SelectButton>().endPoint;

            Transform root = navigationRoot.Find("Arrivals");
            for (int i = 0; i < root.childCount; i++)
            {
                root.GetChild(i).gameObject.SetActive(false);
            }
            setEndPoint.gameObject.SetActive(true);

            InvokeRepeating("DisplayPath", 0, 0.5f);
            CloseNavUI();
        }
        /// <summary>
        /// This method is used to display the path
        /// That is, the guiding lines that users see in navigation mode
        /// Although it's simple, it's important
        /// </summary>
        private void DisplayPath()
        {
            NavAgent.transform.position = SetUserPosition.position;
            NavAgent.enabled = true;
            NavAgent.CalculatePath(setEndPoint.position, NavPath);
            setLine.positionCount = NavPath.corners.Length;
            setLine.SetPositions(NavPath.corners);
            NavAgent.enabled = false;
        }
        /// <summary>
        /// Call the method baking path in NavMeshAgent
        /// </summary>
        private void BakePath()
        {
            NavSurface = FindObjectOfType<NavMeshSurface>();
            NavAgent = FindObjectOfType<NavMeshAgent>();
            NavAgent.transform.position = SetUserPosition.position;
            NavAgent.enabled = false;
            NavSurface.BuildNavMesh();
            NavPath = new NavMeshPath();
        }

        /// <summary>
        /// We can set the navigation line style
        /// But anyway, let's start with this
        /// </summary>
        private void SetLine()
        {
            setLine = navigationRoot.Find("Line").gameObject.AddComponent<LineRenderer>();
            Debug.Log(setLine);
            setLine.material = new Material(Shader.Find("Sprites/Default"));
            setLine.positionCount = 0;
            setLine.widthMultiplier = 0.05f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                new GradientColorKey(Color.blue, 0.0f),
                new GradientColorKey(Color.blue, 1.0f) },
                new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.0f),
                new GradientAlphaKey(1f, 1.0f) });
            setLine.colorGradient = gradient;
        }
        /// <summary>
        /// This method is used to load paths
        /// The path needs to be additionally set in another menu
        /// In fact, this only needs to be set up once on the management side, and there is no need for the user side to set it up
        /// </summary>
        private void LoadRoads()
        {
            var list = findObject.LoadRoads();

            var temp = new GameObject().transform;
            temp.parent = navigationRoot.Find("Roads");

            foreach (var item in list)
            {
                var road = JsonUtility.FromJson<RoadInformation>(item);
                var tfRoad = Instantiate(importPath, navigationRoot.Find("Roads"));

                tfRoad.localPosition = (road.startPointPosition + road.endPointPosition) / 2;
                temp.localPosition = road.endPointPosition;
                tfRoad.LookAt(temp);
                tfRoad.localScale = new Vector3(0.02f, 1f, (road.endPointPosition - road.startPointPosition).magnitude * 0.1f + 0.2f);
            }
            Destroy(temp.gameObject);
        }
        /// <summary>
        /// This method is used to load the target
        /// That is, the previously set keyPoint
        /// </summary>
        private void LoadArrivals()
        {
            var list = findObject.LoadKeyPoins();
            foreach (var item in list)
            {
                EARpointData point = JsonUtility.FromJson<EARpointData>(item);
                if (point.KeyPointType == 0)
                {
                    var btn = Instantiate(findPrefab, navigationPanel);
                    btn.keyPoint = point;
                    btn.GetComponentInChildren<Text>().text = point.KeyPointName;

                    var arrivalTemp = Instantiate(endPoint, navigationRoot.Find("Arrivals"));
                    arrivalTemp.localPosition = point.KeyPointPosition;
                    btn.endPoint = arrivalTemp;
                    arrivalTemp.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// Used to display navigation menus, if successfully entering sparse space positioning
        /// </summary>
        private void ShowNavUI()
        {
            ARpanel.SetActive(true);
        }
        /// <summary>
        /// It needs to be closed as it has been opened
        /// </summary>
        private void CloseNavUI()
        {
            ARpanel.SetActive(false);
        }
    }
}

