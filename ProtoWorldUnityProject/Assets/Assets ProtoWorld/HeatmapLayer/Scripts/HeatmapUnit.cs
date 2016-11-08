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
* HeatmapUnit.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;

namespace HeatmapLayer
{
    /// <summary>
    /// Class that defines a heatmap unit.
    /// </summary>
    public class HeatmapUnit
    {
        /// <summary>
        /// Value of the heatmap unit.
        /// </summary>
        private float value;

        /// <summary>
        /// Renderer of the heatmap unit.
        /// </summary>
        private MeshRenderer renderer;

        /// <summary>
        /// Configuration of the heatmap.
        /// </summary>
        private HeatmapConfig conf;

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public HeatmapUnit(MeshRenderer renderer, HeatmapConfig conf)
        {
            this.renderer = renderer;
            this.conf = conf;
            value = conf.minHeatmapValue;
            this.renderer.material.color = conf.minHeatmapColor;
        }

        /// <summary>
        /// Get the current value of the heatmap.
        /// </summary>
        /// <returns>Value of the heatmap.</returns>
        public float GetValue()
        {
            return value;
        }

        /// <summary>
        /// Adds a value to the current value of the heatmap unit and updates the color.
        /// </summary>
        /// <param name="add">Value to add.</param>
        public void AddToValue(float add)
        {
            this.value += add;
            InterpolateToColor();
        }

        /// <summary>
        /// Sets a new value for the heatmap unit and updates the heatmap color.
        /// </summary>
        /// <param name="value"></param>
        public void SetNewValue(float value)
        {
            this.value = value;
            InterpolateToColor();
        }

        /// <summary>
        /// Reset the heatmap unit to its min value.
        /// </summary>
        public void Reset()
        {
            this.value = conf.minHeatmapValue;
            renderer.material.color = conf.minHeatmapColor;
        }

        /// <summary>
        /// Interpolates the color of the heatmap unit according to the current value. 
        /// </summary>
        private void InterpolateToColor()
        {
            float halfStep = conf.GetHalfStep();

            if (value < halfStep)
            {
                // If value below half step, interpolate between min and med color
                renderer.material.color =
                    Color.Lerp(conf.minHeatmapColor, conf.medHeatmapColor,
                                value / halfStep);
            }
            else
            {
                // Interpolate between med and max color
                renderer.material.color =
                    Color.Lerp(conf.medHeatmapColor, conf.maxHeatmapColor,
                                (value - halfStep) / halfStep);
            }
        }

        /// <summary>
        /// Set a random color for the heatmap unit (for testing purposes).
        /// </summary>
        public void SetRandomColor()
        {
            renderer.material.color =
                new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255),
                            (byte)Random.Range(0, 255), 64);
        }
    }
}
