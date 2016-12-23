/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * GENERIC SCRIPTS
 * TakeScreenshot.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This script takes general screenshots from the game screen.
/// </summary>
/// <remarks>
/// For taking screenshots from a certain camera <see cref="CameraScreenshot"/>
/// </remarks>
public class TakeScreenshot : MonoBehaviour
{
	public bool takeScreenshotOnSpace = false;

	public bool takeScreenshotOnQuit = false;

    public bool autoScreenshot = false;

    [Range(1.0f, 300.0f)]
    public float autoScreenshotFrequencyInSeconds = 5.0f;

    /// <summary>
    /// Start method.
    /// </summary>
    void Start()
    {
        if (autoScreenshot)
            StartCoroutine(AutoScreenshotCoroutine());
    }

	/// <summary>
    /// Update method.
    /// </summary>
	private void Update()
	{
		if (takeScreenshotOnSpace)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				TakeNewScreenshot();
			}
		}
	}

    /// <summary>
    /// Coroutine to handle auto screenshots.
    /// </summary>
    /// <returns></returns>
    IEnumerator AutoScreenshotCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoScreenshotFrequencyInSeconds);

            TakeNewScreenshot();

            yield return null;
        }
    }

    /// <summary>
    /// On application quit, takes a screenshot of the game (if option enabled)
    /// </summary>
    void OnApplicationQuit()
    {
        if (takeScreenshotOnQuit)
        {
            Application.CancelQuit();
            TakeNewScreenshot();
            Application.Quit();
        }
    }

    /// <summary>
    /// Captures a screenshot of the screen.
    /// </summary>
    public void TakeNewScreenshot()
    {
        string screenshotFolderPath = Application.dataPath + "/Screenshots";
        string filePath;
        int count = 0;

        if (!System.IO.Directory.Exists(screenshotFolderPath))
            System.IO.Directory.CreateDirectory(screenshotFolderPath);

        do
        {
            filePath = string.Format("{0}/{1}_screenshot.png", screenshotFolderPath, count++);
        }
        while (System.IO.File.Exists(filePath));

        Application.CaptureScreenshot(filePath, 2);
    }
}
