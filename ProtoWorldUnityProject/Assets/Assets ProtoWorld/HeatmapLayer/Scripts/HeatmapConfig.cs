/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* 
* HEATMAP LAYER
* HeatmapConfig.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;

namespace HeatmapLayer
{
    /// <summary>
    /// Holds the data configuration to be used by the heatmap controller and the heatmap units. 
    /// </summary>
    public class HeatmapConfig : MonoBehaviour
    {
        /// <summary>
        /// Unit of a heatmap pixel in the scene.
        /// </summary>
        [Range(1, 100)]
        public int heatmapUnitSize = 25;

        /// <summary>
        /// Object that represents a heatmap unit.
        /// </summary>
        public GameObject heatmapUnitObject;

        /// <summary>
        /// Min coordinates of the heatmap.
        /// </summary>
        public Vector3 minCoordinates;

        /// <summary>
        /// Max coordinates of the heatmap.
        /// </summary>
        public Vector3 maxCoordinates;

        /// <summary>
        /// Refreshing period of the heatmap in seconds.
        /// </summary>
        [Range(0.1f, 10.0f)]
        public float refreshingPeriod = 1.0f;

        /// <summary>
        /// True to make an acumulative heatmap (values do not reset every new cycle).
        /// </summary>
        public bool accumulativeHeatmap = false;

        /// <summary>
        /// True to affect vecinity units when a heatmap unit changes its value.
        /// </summary>
        public bool affectVecinityUnits = false;

        /// <summary>
        /// Tell how is the ratio of vecinity effect.
        /// </summary>
        [Range(0.1f, 1.0f)]
        public float vecinityRatio = 0.5f;

        /// <summary>
        /// Min and max values for the heatmap units. 
        /// </summary>
        public float minHeatmapValue = 0f,
                     maxHeatmapValue = 100f;

        /// <summary>
        /// Colors to cover the value spectrum of the heatmap.
        /// </summary>
        public Color minHeatmapColor = Color.green,
                     medHeatmapColor = Color.yellow,
                     maxHeatmapColor = Color.red;

        /// <summary>
        /// Enumerate for the types of heatmap visualization.
        /// </summary>
        public enum HeatmapVisualizationType
        {
            Pedestrians,
            Vehicles,
            Others
        }

        /// <summary>
        /// Type of visualization to show
        /// </summary>
        public HeatmapVisualizationType visualizationType = HeatmapVisualizationType.Pedestrians;

        /// <summary>
        /// The heatmap camera.
        /// </summary>
        public Camera heatmapCamera;

        /// <summary>
        /// Half step to the max value.
        /// </summary>
        private float halfStep;

        /// <summary>
        /// Script starting.
        /// </summary>
        void Start()
        {
            halfStep = maxHeatmapValue / 2f;

            if (heatmapCamera != null)
                heatmapCamera.clearFlags = CameraClearFlags.Depth;
        }

        /// <summary>
        /// Set the heatmap intensity (the max. heatmap value).
        /// </summary>
        /// <param name="value">New max value for the heatmap.</param>
        /// <returns></returns>
        public void SetIntensity(float value)
        {
            maxHeatmapValue = value;
            halfStep = maxHeatmapValue / 2f;
        }

        /// <summary>
        /// Gets the half step to the max value.
        /// </summary>
        /// <returns>Half step value.</returns>
        public float GetHalfStep()
        {
            return halfStep;
        }

        /// <summary>
        /// Changes the visualization type given the input value.
        /// </summary>
        /// <param name="value">Index of the visualization type in the enumerate.</param>
        public void ChangeVisualization(int value)
        {
            switch (value)
            {
                case 0:
                    visualizationType = HeatmapVisualizationType.Pedestrians;
                    break;
                case 1:
                    visualizationType = HeatmapVisualizationType.Vehicles;
                    break;
                default:
                    visualizationType = HeatmapVisualizationType.Others;
                    break;
            }
        }
    }
}
