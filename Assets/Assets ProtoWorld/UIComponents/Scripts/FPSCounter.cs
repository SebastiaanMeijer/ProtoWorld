/*
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
