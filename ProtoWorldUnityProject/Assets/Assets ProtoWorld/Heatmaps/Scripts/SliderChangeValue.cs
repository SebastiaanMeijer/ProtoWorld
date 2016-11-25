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
 * Berend Wouda
 */

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderChangeValue : MonoBehaviour {
	public Slider slider;
	public Text text;
	public int idNumber;


	void OnEnable() {
		if(idNumber == 1) {
			slider.onValueChanged.AddListener(ChangeValueIntensityHM);
			ChangeValueIntensityHM(slider.value);
		}
		else if(idNumber == 2) {
			slider.onValueChanged.AddListener(ChangeValueRadiusHM);
			ChangeValueRadiusHM(slider.value);
		}
	}

	void OnDisable() {
		slider.onValueChanged.RemoveAllListeners();
	}


	void ChangeValueIntensityHM(float value) {
		Heatmap.changeParameterIntensityHM(value);
		text.text = value.ToString("n0");
	}

	void ChangeValueRadiusHM(float value) {
		Heatmap.changeParameterRadiusHM(value);
		text.text = "x" + value.ToString("n1");
	}
}