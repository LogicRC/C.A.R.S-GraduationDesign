using UnityEngine;
using System;

namespace CARS
{
    /// <summary>
    /// This script is used to generate path related variables
    /// </summary>
    [Serializable]
    public class RoadInformation
    {
        /// <summary>
        /// Declare a variable to store the starting coordinates
        /// </summary>
        public Vector3 startPointPosition;
        /// <summary>
        /// Declare a variable to store the arrival coordinates
        /// </summary>
        public Vector3 endPointPosition;
        /// <summary>
        /// Declare a variable to store the starting position name
        /// </summary>
        public string startPointName;
        /// <summary>
        /// Declare a variable to store the name of the destination location
        /// </summary>
        public string endPointName;
    }
}

