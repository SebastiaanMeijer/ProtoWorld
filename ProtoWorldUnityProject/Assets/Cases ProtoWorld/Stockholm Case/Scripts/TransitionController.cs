/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * Stolen from the dashboard branch.
 * 
 * Berend Wouda
 * 
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour {
	public bool useTransition;

	private Image image;

	private WaitForSeconds transitionWait;
	private WaitForSeconds transitionInterval;


	public void Awake() {
		image = GetComponent<Image>();

		if(useTransition) {
			image.enabled = true;
		}
	}

	void Start () {
		transitionWait = new WaitForSeconds(5.0f);
		transitionInterval = new WaitForSeconds(0.01f);
		
		transitionIn();
	}


	public void transitionIn(Action action = null) {
		if(useTransition) {
			StartCoroutine(startInTransition(action));
		}
	}

	public void transitionOut(Action action = null) {
		if(useTransition) {
			StartCoroutine(startOutTransition(action));
		}
	}


	private IEnumerator startInTransition(Action action) {
		yield return transitionWait;
		
		for(int time = 0; time <= 25; time++) {
			setInTransitionTime(time / 25.0f);

			yield return transitionInterval;
		}

		// No longer block raycasts.
		image.enabled = false;

		if(action != null) {
			action();
		}
	}
	
	private IEnumerator startOutTransition(Action action) {
		// Block raycasts.
		image.enabled = true;
		
		for(int time = 0; time <= 25; time++) {
			setOutTransitionTime(time / 25.0f);

			yield return transitionInterval;
		}

		yield return transitionWait;
		
		if(action != null) {
			action();
		}
	}


	private void setInTransitionTime(float time) {
		Color color = image.color;
		color.a = 1.0f - time;
		image.color = color;
	}
	
	private void setOutTransitionTime(float time) {
		Color color = image.color;
		color.a = time;
		image.color = color;
	}
}
