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
* ShowHeatmapUI.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;

namespace HeatmapLayer
{
    /// <summary>
    /// Implements the behaviour of the UI toggle show heatmap.
    /// </summary>
    public class ShowHeatmapUI : MonoBehaviour
    {
        /// <summary>
        /// Original mask of the main camera.
        /// </summary>
        private int originalCameraMask;

        /// <summary>
        /// Script starting.
        /// </summary>
        void Start()
        {
            originalCameraMask = Camera.main.cullingMask;
            ShowHeatmap(false);
        }

        /// <summary>
        /// Shows/Hides the heatmap from the culling mask of the camera.
        /// </summary>
        /// <param name="show"></param>
        public void ShowHeatmap(bool show)
        {
            if (show)
            {
                var mask = originalCameraMask | 1 << LayerMask.NameToLayer("Heatmap");
                Camera.main.cullingMask = mask;
            }
            else
            {
                Camera.main.cullingMask = originalCameraMask;
            }
        }
    }
}
