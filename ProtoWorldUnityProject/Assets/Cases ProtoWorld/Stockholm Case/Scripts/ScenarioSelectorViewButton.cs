/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 *
 * Stockholm MatSIM integration.
 * 
 * Berend Wouda
 * 
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenarioSelectorViewButton : MonoBehaviour {
	public Dropdown scenarioDropdown;
	public Dropdown hourDropdown;
	public Dropdown employmentDropdown;
	public Dropdown parameterDropdown;

	public string scenePath = "Stockholm Case v2.0";

	public TransitionController transitionController;


	public void view() {
		StockholmMatSIMParameters.Instance.Location = (StockholmMatSIMLocation) scenarioDropdown.value;
		StockholmMatSIMParameters.Instance.Hour = (StockholmMatSIMHour) hourDropdown.value;
		StockholmMatSIMParameters.Instance.Employment = (StockholmMatSIMEmployment) employmentDropdown.value;
		StockholmMatSIMParameters.Instance.Commitment = (StockholmMatSIMCommitment) parameterDropdown.value;

		transitionController.transitionOut(load);
	}

	private void load() {
		SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
	}
}
