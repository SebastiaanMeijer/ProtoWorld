/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * FPS COUNTER
 * FPSCounter.cs
 * Miguel Ramos Carretero
 * Aram Azhari
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Counts the frames-per-second of a scene.
/// </summary>
public class FPSCounter : MonoBehaviour 
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// The actual frame count
    /// </summary>
    int frameCount = 0;

    /// <summary>
    /// Frames per second
    /// </summary>
    public float fps = 0.0f;

    /// <summary>
    /// The text that shows the FPS
    /// </summary>
    private UnityEngine.UI.Text text;

    private float period = 1.0f;
    private float nextActionTime = 0.0f;

    /// <summary>
    /// Initializes the class
    /// </summary>
    void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>();
    }

    /// <summary>
    /// Updates the fps value and the text.
    /// </summary>
    void Update()
    {
        frameCount++;

        // Log the fps 
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            fps = frameCount / period;

            if (text != null)
                text.text = /*"FPS: " +*/ fps.ToString();

            frameCount = 0;
            log.Info("FPS: " + fps);
        }
    }
}
