/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Micro/Macro Visualization Module
 * 
 * Furkan Sonmez
 * Berend Wouda
 */

using UnityEngine;

public class Visualizer : MonoBehaviour {
	public bool visualizationEnabled = true;

	private int level;

	[System.Serializable]
	public struct LayerLevel {
		public LayerMask layer;
		public int level;
	}

	public LayerLevel[] layerLevels;

	[System.Serializable]
	public struct ObjectLevel {
		public GameObject gameObject;
		public int level;
	}

	public ObjectLevel[] objectLevels;


	public void Update() {
		if(level != ZoomScrollbarMMV.level) {
			level = ZoomScrollbarMMV.level;

			updateVisualization();
		}
	}

	/// <summary>
	/// Activates or deactivates the visualizer when the button is pressed.
	/// </summary>
	public void activateDeactivateMMV() {
		visualizationEnabled = !visualizationEnabled;

		updateVisualization();
	}


	private void updateVisualization() {
		foreach(LayerLevel layerLevel in layerLevels) {
			updateLayerVisualization(layerLevel);
		}

		foreach(ObjectLevel objectLevel in objectLevels) {
			updateObjectVisualization(objectLevel);
		}
	}

	private void updateLayerVisualization(LayerLevel layerLevel) {
		if(visualizationEnabled) {
			if(layerLevel.level > level && layerLevel.level >= 0) {
				setLayerVisualization(layerLevel, true);
			}
			else if(layerLevel.level <= level && layerLevel.level >= 0) {
				setLayerVisualization(layerLevel, false);
			}
			else if(layerLevel.level > -level && layerLevel.level < 0) {
				setLayerVisualization(layerLevel, true);
			}
			else if(layerLevel.level <= -level && layerLevel.level < 0) {
				setLayerVisualization(layerLevel, false);
			}
		}
		else {
			setLayerVisualization(layerLevel, true);
		}
	}

	private void updateObjectVisualization(ObjectLevel objectLevel) {
		if(visualizationEnabled) {
			if(objectLevel.gameObject != null) {
				if(objectLevel.level > level && objectLevel.level >= 0) {
					setObjectVisualization(objectLevel, true);
				}
				else if(objectLevel.level <= level && objectLevel.level >= 0) {
					setObjectVisualization(objectLevel, false);
				}
				else if(objectLevel.level > -level && objectLevel.level < 0) {
					setObjectVisualization(objectLevel, true);
				}
				else if(objectLevel.level <= -level && objectLevel.level < 0) {
					setObjectVisualization(objectLevel, false);
				}
			}
		}
		else {
			setObjectVisualization(objectLevel, true);
		}
	}


	private void setLayerVisualization(LayerLevel layerLevel, bool layerEnabled) {
		if(layerEnabled) {
			Camera.main.cullingMask |= layerLevel.layer.value;
		}
		else {
			Camera.main.cullingMask &= ~layerLevel.layer.value;
		}
	}

	private void setObjectVisualization(ObjectLevel objectLevel, bool objectEnabled) {
		objectLevel.gameObject.SetActive(objectEnabled);
	}
}
