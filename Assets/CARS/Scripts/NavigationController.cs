using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using easyar;

namespace CARS
{
    /// <summary>
    /// This script is used to control AR navigation scenes
    /// </summary>
    public class NavigationController : MonoBehaviour
    {
        /// <summary>
        /// Declare game control, it's just called this, temporarily
        /// </summary>
        private GameController gameController;
        /// <summary>
        /// Declare the navigation canvas in the scene
        /// </summary>
        private GameObject panel;
        /// <summary>
        /// Declare navigation buttons in the scene
        /// </summary>
        private Button btnNav;
        /// <summary>
        /// Declare navigation buttons in the scene
        /// </summary>
        public SelectButton prefabButton;
        /// <summary>
        /// Declare the navigation button container in the scene
        /// </summary>
        private Transform svContent;
        /// <summary>
        /// Declare the navigation root node in the scene
        /// </summary>
        public Transform navRoot;
        /// <summary>
        /// Declaration of destination prefabricated parts in the scenario
        /// </summary>
        public Transform prefabArrival;
        /// <summary>
        /// Declare path prefabricated components in the scene
        /// </summary>
        public Transform prefabRoad;
        /// <summary>
        /// Declare the navigation lines in the scene
        /// </summary>
        private LineRenderer lineRenderer;
        /// <summary>
        /// Declare the navigation agent in the scene
        /// </summary>
        private NavMeshAgent agent;
        /// <summary>
        /// Declare the navigation path in the scene
        /// </summary>
        private NavMeshPath path;
        private NavMeshSurface surface;
        /// <summary>
        /// Declare the navigation target in the scene
        /// </summary>
        private Transform arrival;
        /// <summary>
        /// Declare the user's location to achieve navigation effects that follow the user's movements
        /// </summary>
        public Transform player;

        private ARSession session;
        private SparseSpatialMapWorkerFrameFilter mapWorker;
        private SparseSpatialMapController map;
        void Start()
        {
            gameController = FindObjectOfType<GameController>();
            panel = GameObject.Find("/Canvas/Panel");
            btnNav = GameObject.Find("/Canvas/ButtonNav").GetComponent<Button>();
            btnNav.onClick.AddListener(ShowNavUI);
            btnNav.interactable = false;
            panel.transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(CloseNavUI);

            svContent = panel.transform.Find("Scroll View/Viewport/Content").transform;

            session = FindObjectOfType<ARSession>();
            mapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
            map = FindObjectOfType<SparseSpatialMapController>();

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
            map.MapManagerSource.ID = PlayerPrefs.GetString("MapID");
            map.MapManagerSource.Name = PlayerPrefs.GetString("MapName");
            //Set feedback on map loading, success or failure
            map.MapLoad += (map, status, error) =>
            {
                if (status)
                {
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
                ClearNav();
                LoadArrivals();
                LoadRoads();
                BakePath();
                btnNav.interactable = true;
                ShowNavUI();
            };
            // Set stop location event prompt
            map.MapStopLocalize += () =>
            {
                gameController.SendMessage("ShowMessage", "Stop sparse space localization");
            };
            gameController.SendMessage("ShowMessage", "Start loading the map");
            mapWorker.Localizer.startLocalization();    // Call the method in EasyAR plugin to start localizing the map
        }
        /// <summary>
        /// Clean up navigation elements
        /// Otherwise, catastrophic multiple navigation overlays may occur
        /// Of course we need to delete it, right?
        /// </summary>
        private void ClearNav()
        {
            // delete Button
            foreach (Transform tf in svContent)
            {
                Destroy(tf.gameObject);
            }
            // Delete destination
            foreach (Transform tf in navRoot.Find("Arrivals"))
            {
                Destroy(tf.gameObject);
            }
            // delete path
            foreach (Transform tf in navRoot.Find("Roads"))
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
            arrival = btnTF.GetComponent<SelectButton>().arrival;

            Transform root = navRoot.Find("Arrivals");
            for (int i = 0; i < root.childCount; i++)
            {
                root.GetChild(i).gameObject.SetActive(false);
            }
            arrival.gameObject.SetActive(true);

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
            agent.transform.position = player.position;
            agent.enabled = true;
            agent.CalculatePath(arrival.position, path);
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
            agent.enabled = false;
        }
        /// <summary>
        /// Call the method baking path in NavMeshAgent
        /// </summary>
        private void BakePath()
        {
            surface = FindObjectOfType<NavMeshSurface>();
            agent = FindObjectOfType<NavMeshAgent>();
            agent.transform.position = player.position;
            agent.enabled = false;
            surface.BuildNavMesh();
            path = new NavMeshPath();
        }

        /// <summary>
        /// We can set the navigation line style
        /// But anyway, let's start with this
        /// </summary>
        private void SetLine()
        {
            lineRenderer = navRoot.Find("Line").gameObject.AddComponent<LineRenderer>();
            Debug.Log(lineRenderer);
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.positionCount = 0;
            lineRenderer.widthMultiplier = 0.05f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                new GradientColorKey(Color.blue, 0.0f),
                new GradientColorKey(Color.blue, 1.0f) },
                new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.0f),
                new GradientAlphaKey(1f, 1.0f) });
            lineRenderer.colorGradient = gradient;
        }
        /// <summary>
        /// This method is used to load paths
        /// The path needs to be additionally set in another menu
        /// In fact, this only needs to be set up once on the management side, and there is no need for the user side to set it up
        /// </summary>
        private void LoadRoads()
        {
            var list = gameController.LoadRoads();

            var temp = new GameObject().transform;
            temp.parent = navRoot.Find("Roads");

            foreach (var item in list)
            {
                var road = JsonUtility.FromJson<Road>(item);
                var tfRoad = Instantiate(prefabRoad, navRoot.Find("Roads"));

                tfRoad.localPosition = (road.startPosition + road.arrivalPosition) / 2;
                temp.localPosition = road.arrivalPosition;
                tfRoad.LookAt(temp);
                tfRoad.localScale = new Vector3(0.02f, 1f, (road.arrivalPosition - road.startPosition).magnitude * 0.1f + 0.2f);
            }
            Destroy(temp.gameObject);
        }
        /// <summary>
        /// This method is used to load the target
        /// That is, the previously set keyPoint
        /// </summary>
        private void LoadArrivals()
        {
            var list = gameController.LoadKeyPoins();
            foreach (var item in list)
            {
                KeyPoint point = JsonUtility.FromJson<KeyPoint>(item);
                if (point.pointType == 0)
                {
                    var btn = Instantiate(prefabButton, svContent);
                    btn.keyPoint = point;
                    btn.GetComponentInChildren<Text>().text = point.name;

                    var arrivalTemp = Instantiate(prefabArrival, navRoot.Find("Arrivals"));
                    arrivalTemp.localPosition = point.position;
                    btn.arrival = arrivalTemp;
                    arrivalTemp.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// Used to display navigation menus, if successfully entering sparse space positioning
        /// </summary>
        private void ShowNavUI()
        {
            panel.SetActive(true);
        }
        /// <summary>
        /// It needs to be closed as it has been opened
        /// </summary>
        private void CloseNavUI()
        {
            panel.SetActive(false);
        }
    }
}

