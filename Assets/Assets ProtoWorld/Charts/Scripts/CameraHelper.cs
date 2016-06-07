using UnityEngine;
using System.Collections;

//[assembly: log4net.Config.XmlConfigurator(ConfigFile = "UnityLog4Net.config", Watch = true)]

public static class CameraHelper {

    //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private static float xPos = 0;
    private static float yPos = -2000;

    public static void SetCameraPosition(Camera camera)
    {
        camera.transform.position = new Vector2(xPos, yPos);
        xPos += 500;
    }
}
