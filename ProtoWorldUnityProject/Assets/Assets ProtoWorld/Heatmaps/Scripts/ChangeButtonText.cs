/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Heatmap Module
 * 
 * Furkan Sonmez
 */

using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonText : MonoBehaviour {
	public Text ButtonText;
	public Text TypeText;
	public string[] buttonText;
	public string[] typeText;

	public void Awake() {
		ButtonText = transform.parent.Find("TargetText").GetComponent<Text>();
		TypeText = transform.parent.Find("TypeText").GetComponent<Text>();
	}

	public void resetHMText() {
		ButtonText.text = buttonText[Heatmap.heatmapNumber - 1];
		TypeText.text = "Locations";
	}

	public void resetTypeText() {
		//TypeText.text = typeText[Heatmap.heatmapTypeNumber -1];
		//TypeText.text = Heatmap.typeString;
	}
}
