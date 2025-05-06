using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;

namespace CARS
{
    /// <summary>
    /// This script is used to establish a map scene controller
    /// </summary>
    public class BuildMapController : MonoBehaviour
    {
        /// <summary>
        /// Declared a field of type GameController to reference GameController components
        /// </summary>
        private GameController gameController;
        /// <summary>
        /// Declare a save button to save the map
        /// </summary>
        private Button btnSave;
        private ARSession session;
        private SparseSpatialMapWorkerFrameFilter mapWorker;
        private SparseSpatialMapController map;

        void Start()
        {
            // Initialize save button
            gameController = FindObjectOfType<GameController>();
            btnSave = GameObject.Find("/Canvas/ButtonSave").GetComponent<Button>();
            btnSave.onClick.AddListener(Save);
            btnSave.interactable = false;
            // Initialize sparse space map, this is a method called in EasyAR plugin
            session = FindObjectOfType<ARSession>();
            mapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
            map = FindObjectOfType<SparseSpatialMapController>();
            // Set tracking status. If motion tracking is in progress, then the tracking status is also included
            session.WorldRootController.TrackingStatusChanged += OnTrackingStatusChanged;
            // This call methods in EasyAR
            if (session.WorldRootController.TrackingStatus == MotionTrackingStatus.Tracking)
            {
                btnSave.interactable = true;
            }
            else
            {
                btnSave.interactable = false;
            }
        }

        /// <summary>
        /// The following method is used to save a recorded sparse space map.
        /// </summary>
        private void Save()
        {
            btnSave.interactable = false;
            // Set feedback on map saving results
            mapWorker.BuilderMapController.MapHost += (mapInfo, isSuccess, error) =>
            {
                if (isSuccess)
                {
                    // Save the map ID and name, and the EasyAR plugin will send it to the cloud
                    PlayerPrefs.SetString("MapID", mapInfo.ID);
                    PlayerPrefs.SetString("MapName", mapInfo.Name);
                    gameController.SendMessage("ShowMessage", "Map saved successfully.");
                }
                else
                {
                    // Prompt when saving fails, with error report attached
                    gameController.SendMessage("ShowMessage", "Map save failed:" + error);
                    btnSave.interactable = true;
                }
            };
            try
            {
                // Attempt to save the map
                mapWorker.BuilderMapController.Host(gameController.inputName, null);
                gameController.SendMessage("ShowMessage", "Start saving map, please wait.");
            }
            catch (Exception ex)
            {
                gameController.SendMessage("ShowMessage", "Save error:" + ex.Message);
                btnSave.interactable = true;
            }
        }
        /// <summary>
        /// Detecting changes in camera status of equipment
        /// </summary>
        /// <param name="status">state</param>
        private void OnTrackingStatusChanged(MotionTrackingStatus status)
        {
            if (status == MotionTrackingStatus.Tracking)
            {
                btnSave.interactable = true;
                gameController.SendMessage("ShowMessage", "Enter tracking status.");
            }
            else
            {
                btnSave.interactable = false;
                gameController.SendMessage("ShowMessage", "Tracking error, please restart.");
            }
        }
    }
}

