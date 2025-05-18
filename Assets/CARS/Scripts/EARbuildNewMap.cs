using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;

namespace CARS
{
    /// <summary>
    /// This script is used to establish a map scene controller
    /// </summary>
    public class EARbuildNewMap : MonoBehaviour
    {
        /// <summary>
        /// Declared a field ozf type GameController to reference GameController components
        /// </summary>
        private EARcreatePath findObject;
        /// <summary>
        /// Declare a save button to save the map
        /// </summary>
        private Button functionSave;
        private ARSession basicSession;
        private SparseSpatialMapWorkerFrameFilter EARmapWorker;
        private SparseSpatialMapController EARmap;

        void Start()
        {
            // Initialize save button
            findObject = FindObjectOfType<EARcreatePath>();
            functionSave = GameObject.Find("/Canvas/ButtonSave").GetComponent<Button>();
            functionSave.onClick.AddListener(EARmapSave);
            functionSave.interactable = false;
            // Initialize sparse space map, this is a method called in EasyAR plugin
            basicSession = FindObjectOfType<ARSession>();
            EARmapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
            EARmap = FindObjectOfType<SparseSpatialMapController>();
            // Set tracking status. If motion tracking is in progress, then the tracking status is also included
            basicSession.WorldRootController.TrackingStatusChanged += OnTrackingStatusChanged;
            // This call methods in EasyAR
            if (basicSession.WorldRootController.TrackingStatus == MotionTrackingStatus.Tracking)
            {
                functionSave.interactable = true;
            }
            else
            {
                functionSave.interactable = false;
            }
        }

        /// <summary>
        /// The following method is used to save a recorded sparse space map.
        /// </summary>
        private void EARmapSave()
        {
            functionSave.interactable = false;
            // Set feedback on map saving results
            EARmapWorker.BuilderMapController.MapHost += (mapInfo, isSuccess, error) =>
            {
                if (isSuccess)
                {
                    // Save the map ID and name, and the EasyAR plugin will send it to the cloud
                    PlayerPrefs.SetString("MapID", mapInfo.ID);
                    PlayerPrefs.SetString("MapName", mapInfo.Name);
                    findObject.SendMessage("ShowMessage", "Map saved successfully.");
                }
                else
                {
                    // Prompt when saving fails, with error report attached
                    findObject.SendMessage("ShowMessage", "Map save failed:" + error);
                    functionSave.interactable = true;
                }
            };
            try
            {
                // Attempt to save the map
                EARmapWorker.BuilderMapController.Host(findObject.inputName, null);
                findObject.SendMessage("ShowMessage", "Start saving EARmap, please wait.");
            }
            catch (Exception ex)
            {
                findObject.SendMessage("ShowMessage", "EARmapSave error:" + ex.Message);
                functionSave.interactable = true;
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
                functionSave.interactable = true;
                findObject.SendMessage("ShowMessage", "Enter tracking status.");
            }
            else
            {
                functionSave.interactable = false;
                findObject.SendMessage("ShowMessage", "Tracking error, please restart.");
            }
        }
    }
}

