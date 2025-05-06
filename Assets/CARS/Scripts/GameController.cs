using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

namespace CARS
{
    /// <summary>
    /// This is a global control method, although it includes a game, it is not a game.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        private static GameController instance = null;
        /// <summary>
        /// Declare variables to store the input map name.
        /// </summary>
        public string inputName;
        /// <summary>
        /// Declaration Display Information Text Box
        /// </summary>
        private Text txtShow;
        /// <summary>
        /// Used to reference Unity's UI text components in scripts
        /// </summary>
        private string pathKeyPoints;
        /// <summary>
        /// Declare variables to store navigation paths and storage paths
        /// </summary>
        private string pathRoads;

        void Awake()
        {
            //1.Ensure globally unique instances and avoid duplicate object creation through singleton mode
            //2.Cross scenario persistence: Retain critical data controllers.
            //3.Set a reliable persistent path for file storage.
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (this != instance)
            {
                Destroy(gameObject);
            }

            pathKeyPoints = Application.persistentDataPath + "/keypoints.txt";
            pathRoads = Application.persistentDataPath + "/roads.txt";
        }

        void Start()
        {
            txtShow = transform.GetComponentInChildren<Text>();
            txtShow.gameObject.SetActive(false);
        }

        #region Reminder Information
        /// <summary>
        /// This method is used to display information
        /// </summary>
        /// <param name="message">The information you want to display</param>
        public void ShowMessage(string message)
        {
            StopCoroutine("EndShowMessage");
            txtShow.gameObject.SetActive(true);
            txtShow.text = message;
            StartCoroutine("EndShowMessage");
        }
        /// <summary>
        /// This method is used to hide information after you don't want it to be displayed
        /// </summary>
        /// <returns></returns>
        private IEnumerator EndShowMessage()
        {
            yield return new WaitForSeconds(4f);
            txtShow.text = "";
            txtShow.gameObject.SetActive(false);
        }

        #endregion

        #region Read key points and paths
        /// <summary>
        /// The following method is used to save key points
        /// This is when you scan the beacon (such as a pre-set QR code) when starting the keypoint scanning mode
        /// Then you can save the location information of this beacon as a key point
        /// </summary>
        /// <param name="jsons">json string array</param>
        public void SaveKeyPoints(string[] jsons)
        {
            SaveStringArray(jsons, pathKeyPoints);
        }
        /// <summary>
        /// The following method is used to load key points
        /// </summary>
        /// <returns>Load the json list of key points</returns>
        public List<string> LoadKeyPoins()
        {
            return LoadStringList(pathKeyPoints);
        }
        /// <summary>
        /// The following method is used to save the path
        /// The path connects key points to each other
        /// When activating AR navigation mode, it will display according to the connected path
        /// </summary>
        /// <param name="jsons">json string array</param>
        public void SaveRoads(string[] jsons)
        {
            SaveStringArray(jsons, pathRoads);
        }
        /// <summary>
        /// The following method is used to load the path
        /// </summary>
        /// <returns>Display path json list</returns>
        public List<string> LoadRoads()
        {
            return LoadStringList(pathRoads);
        }
        /// <summary>
        /// Save string array
        /// </summary>
        /// <param name="stringArray">This variable is a saved string array</param>
        /// <param name="path">This variable is the path where it was saved</param>
        private void SaveStringArray(string[] stringArray, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (var s in stringArray)
                    {
                        writer.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        /// <summary>
        /// Read text information
        /// </summary>
        /// <param name="path">This variable is a text path</param>
        /// <returns>and then return a list of strings</returns>
        private List<string> LoadStringList(string path)
        {
            List<string> list = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        list.Add(reader.ReadLine());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return list;
        }

        #endregion
    }
}

