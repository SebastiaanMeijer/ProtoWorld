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
* HeatmapController.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HeatmapLayer
{
    /// <summary>
    /// This class implements the behaviour of the heatmap layer. 
    /// </summary>
    [RequireComponent(typeof(HeatmapConfig))]
    public class HeatmapController : MonoBehaviour
    {
        /// <summary>
        /// Heatmap array that contains all the information.
        /// </summary>
        private HeatmapUnit[][] heatmapArray;

        /// <summary>
        /// Auxiliar variables to iterate the heatmap array
        /// </summary>
        private int numberOfZElements, numberOfXElements;

        /// <summary>
        /// Configuration of the heatmap.
        /// </summary>
        private HeatmapConfig conf;

        /// <summary>
        /// Time controller of the game.
        /// </summary>
        private TimeController timeCtrl;

        /// <summary>
        /// Tracked flash pedestrians of the scene to add value to the heatmap.
        /// </summary>
        private List<Tuple<GameObject, float>> flashPedElements;

        /// <summary>
        /// Tracked vehicles of the scene to add value to the heatmap.
        /// </summary>
        private List<Tuple<GameObject, ScoreContainer>> vehElements;

        /// <summary>
        /// Tracked general elements of the scene to add value to the heatmap.
        /// </summary>
        private List<Tuple<GameObject, float>> generalElements;

		public class ScoreContainer {
			public float score;

			public ScoreContainer(float score) {
				this.score = score;
			}
		}

        /// <summary>
        /// Script awakening.
        /// </summary>
        void Awake()
        {
            conf = GetComponent<HeatmapConfig>();

            if (conf.heatmapUnitObject == null)
                Debug.LogError("No heatmap unit object defined! ");

            // Get the time controller
            timeCtrl = FindObjectOfType<TimeController>();

            // Initialize the tracked element lists
            flashPedElements = new List<Tuple<GameObject, float>>();
            vehElements = new List<Tuple<GameObject, ScoreContainer>>();
            generalElements = new List<Tuple<GameObject, float>>();

            // Get the number of elements for row Z
            numberOfZElements = Mathf.FloorToInt((conf.maxCoordinates.z - conf.minCoordinates.z)
                                                    / (float)conf.heatmapUnitSize);

            // Get the number of elements for row X
            numberOfXElements = Mathf.FloorToInt((conf.maxCoordinates.x - conf.minCoordinates.x)
                                                    / (float)conf.heatmapUnitSize);

            //Initialize the heatmap first array
            heatmapArray = new HeatmapUnit[numberOfZElements][];

            float halfStep = conf.heatmapUnitSize / 2f;

            //Instantiate each heatmap unit in the scene
            for (int z = 0; z < numberOfZElements; z++)
            {
                //Initialize the heatmap second array
                heatmapArray[z] = new HeatmapUnit[numberOfXElements];

                //Create a new row of heatmap units
                for (int x = 0; x < numberOfXElements; x++)
                {
                    var go = Instantiate(conf.heatmapUnitObject) as GameObject;

                    go.transform.parent = this.transform;

                    go.transform.position = new Vector3(conf.minCoordinates.x + x * conf.heatmapUnitSize + halfStep, 100f,
                                                        conf.minCoordinates.z + z * conf.heatmapUnitSize + halfStep);

                    go.transform.localScale = new Vector3(conf.heatmapUnitSize, conf.heatmapUnitSize * 5, conf.heatmapUnitSize);

                    var rend = go.transform.GetComponent<MeshRenderer>();

                    heatmapArray[z][x] = new HeatmapUnit(rend, conf);
                }
            }
        }

        /// <summary>
        /// Script starting. 
        /// </summary>
        void Start()
        {
            StartCoroutine(RefreshHeatmap());
        }

        /// <summary>
        /// Refreshes the heatmap and all its values.
        /// </summary>
        IEnumerator RefreshHeatmap()
        {
            while (true)
            {
                if (!timeCtrl.IsPaused())
                {
                    if (!conf.accumulativeHeatmap)
                        for (int z = 0; z < numberOfZElements; z++)
                            for (int x = 0; x < numberOfXElements; x++)
                                heatmapArray[z][x].Reset();

                    List<Tuple<GameObject, ScoreContainer>> selectedList;

                    // Select the elements to display in the heatmap
                    //if (conf.visualizationType == HeatmapConfig.HeatmapVisualizationType.Pedestrians)
                    //    selectedList = flashPedElements;
                    //else if (conf.visualizationType == HeatmapConfig.HeatmapVisualizationType.Vehicles)
                        selectedList = vehElements;
                    //else
                    //    selectedList = generalElements;

                    foreach (Tuple<GameObject, ScoreContainer> T in selectedList)
                    {
                        Vector3 pos = T.Item1.transform.position;

                        // Check if the position falls within the bounding box and map it in the heatmap
						// HACK: For the Stockholm case, also check if the vehicle is active. Perhaps it should even do that in general.
                        if (pos.x > conf.minCoordinates.x && pos.x < conf.maxCoordinates.x
                            && pos.z > conf.minCoordinates.z && pos.z < conf.maxCoordinates.z && T.Item1.activeSelf)
                        {
                            int z = Mathf.FloorToInt(Mathf.Abs(pos.z - conf.minCoordinates.z) / (float)conf.heatmapUnitSize);
                            int x = Mathf.FloorToInt(Mathf.Abs(pos.x - conf.minCoordinates.x) / (float)conf.heatmapUnitSize);

                            heatmapArray[z][x].AddToValue(T.Item2.score);

                            //Add to the neighbouring heatmap units
                            if (conf.affectVecinityUnits)
                            {
                                float vecValue = T.Item2.score * conf.vecinityRatio;

                                try
                                {
                                    heatmapArray[z + 1][x + 1].AddToValue(vecValue);
                                    heatmapArray[z + 1][x].AddToValue(vecValue);
                                    heatmapArray[z][x + 1].AddToValue(vecValue);
                                    heatmapArray[z - 1][x].AddToValue(vecValue);
                                    heatmapArray[z][x - 1].AddToValue(vecValue);
                                    heatmapArray[z - 1][x - 1].AddToValue(vecValue);
                                }
                                catch (IndexOutOfRangeException e)
                                {
                                    Debug.LogWarning("Heatmap position beyond the boundaries: " + e.Message);
                                }
                            }
                        }
                    }
                }

                yield return new WaitForSeconds(conf.refreshingPeriod);
            }
        }

        /// <summary>
        /// Tracks the given GameObject in the heatmap.
        /// </summary>
        /// <param name="element">GameObject to track.</param>
        /// <param name="elementValue">Added value of the GameObject int the heatmap. 1f by default.</param>
        public void TrackNewElement(GameObject element, ScoreContainer scoreContainer)
        {
			// HACK: Stockholm case.
            //if (element.GetComponent<FlashPedestriansController>() != null)
                //flashPedElements.Add(new Tuple<GameObject, float>(element, elementValue));

            //else if (element.GetComponent<TrafficIntegrationVehicle>() != null)
                vehElements.Add(new Tuple<GameObject, ScoreContainer>(element, scoreContainer));

            //else
                //generalElements.Add(new Tuple<GameObject, float>(element, elementValue));
        }
    }
}
