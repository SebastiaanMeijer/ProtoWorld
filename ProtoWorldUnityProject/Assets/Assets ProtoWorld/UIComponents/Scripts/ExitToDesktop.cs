/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * UI Components
 * 
 * Berend Wouda
 */
using UnityEngine;

public class ExitToDesktop : MonoBehaviour {
	public TransitionController transitionController;


	public void exitToDesktop() {
		transitionController.transitionOut(exit);
	}

	private void exit() {
		// TODO This is a hack to prevent Unity from crashing upon exit. It is likely caused by one
		// of the modules as it only happens in the case scenes, but no errors are being logged.
		// For now, this will have to do, as we are closing the application anwyays.
		if(!Application.isEditor) {
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

		//Application.Quit();
	}
}
