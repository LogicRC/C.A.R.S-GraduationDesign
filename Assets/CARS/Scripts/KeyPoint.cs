using UnityEngine;
using System;

namespace CARS
{
    /// <summary>
    /// The following method is used to save key point's data
    /// This is when you scan the beacon (such as a pre-set QR code) when starting the keypoint scanning mode
    /// Then you can save the location information of this beacon as a key point
    /// All methods provided in EasyAR
    /// </summary>
    [Serializable]
    public class KeyPoint
    {
        /// <summary>
        /// Declare location information
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Declare angle information
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// Declaration of Key Point Names
        /// </summary>
        public string name;
        /// <summary>
        /// Setting type: 0=destination; 1=Passing point
        /// </summary>
        public int pointType;
    }
}

