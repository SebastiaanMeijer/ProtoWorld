/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 *
 * UI COMPONENTS
 * CameraScreenshot.cs
 * Miguel Ramos Carretero
 * 
 * Based on solution from Unity Answers: 
 * http://goo.gl/Dk3pCI
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This script takes screenshots from the camera attached to it. 
/// </summary>
/// <remarks>
/// For taking screenshots from the game screen <see cref="TakeScreenshot"/>
/// </remarks>
[RequireComponent(typeof(Camera))]
public class CameraScreenshot : MonoBehaviour
{
    public int resWidth = 4000;
    public int resHeight = 2000;

    private Camera screenshotCamera;

    private bool takeScreenshot = false;

    /// <summary>
    /// Awake method.
    /// </summary>
    void Awake()
    {
        screenshotCamera = this.GetComponent<Camera>();

        // The screenshot camera should be disable to avoid 
        // conflicts with other cameras in the game
        screenshotCamera.enabled = false;
    }

    /// <summary>
    /// Take a a shot from the camera.
    /// </summary>
    public void TakeScreenshot()
    {
        takeScreenshot = true;
    }

    /// <summary>
    /// Late update method. 
    /// </summary>
    void LateUpdate()
    {
        if (takeScreenshot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            screenshotCamera.targetTexture = rt;

            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

            screenshotCamera.Render();

            RenderTexture.active = rt;

            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            screenshotCamera.targetTexture = null;

            RenderTexture.active = null; // JC: added to avoid errors

            Destroy(rt);

            byte[] bytes = screenShot.EncodeToPNG();
            string filename = GetScreenshotName();

            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Screenshot taken: {0}", filename));

            takeScreenshot = false;
        }
    }

    /// <summary>
    /// Gets the name of the next screenshot.
    /// </summary>
    /// <returns>Full path and name for a screenshot.</returns>
    private string GetScreenshotName()
    {
        string screenshotFolderPath = Application.dataPath + "/Screenshots";

        if (!System.IO.Directory.Exists(screenshotFolderPath))
            System.IO.Directory.CreateDirectory(screenshotFolderPath);

        return string.Format("{0}/screenshot_{1}x{2}_{3}.png", screenshotFolderPath, resWidth, resHeight, System.DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"));
    }
}
