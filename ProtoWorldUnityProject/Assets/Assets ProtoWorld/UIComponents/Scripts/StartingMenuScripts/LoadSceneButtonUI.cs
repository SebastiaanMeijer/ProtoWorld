/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * STARTING MENU
 * LoadSceneButtonUI.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Implements the behaviour of the UI button in the start menu to load the given scene. 
/// </summary>
public class LoadSceneButtonUI : MonoBehaviour
{
    /// <summary>
    /// Information element to appear when loading new scene. 
    /// </summary>
    public GameObject informationElement;

    /// <summary>
    /// True if persistent information should be destroyed on load. 
    /// </summary>
    public bool destroyPersistentInformation = false;

    /// <summary>
    /// Script awakening. 
    /// </summary>
    void Awake()
    {
        if (informationElement != null)
            informationElement.SetActive(false);
    }

    /// <summary>
    /// Loads the scene specified in the property sceneName.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (informationElement != null)
            informationElement.SetActive(true);

        var pers = FindObjectOfType<PersistentInformation>();
        if (destroyPersistentInformation && pers != null)
            DestroyImmediate(pers);

        SceneManager.LoadScene(sceneName);
    }
}
