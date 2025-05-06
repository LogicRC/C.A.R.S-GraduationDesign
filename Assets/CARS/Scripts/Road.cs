using UnityEngine;
using System;

namespace CARS
{
    /// <summary>
    /// This script is used to generate path related variables
    /// </summary>
    [Serializable]
    public class Road
    {
        /// <summary>
        /// Declare a variable to store the starting coordinates
        /// </summary>
        public Vector3 startPosition;
        /// <summary>
        /// Declare a variable to store the arrival coordinates
        /// </summary>
        public Vector3 arrivalPosition;
        /// <summary>
        /// Declare a variable to store the starting position name
        /// </summary>
        public string startName;
        /// <summary>
        /// Declare a variable to store the name of the destination location
        /// </summary>
        public string arrivalName;
    }
}

