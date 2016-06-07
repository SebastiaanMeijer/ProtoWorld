/*
 * 
 * DECISION TREE MODULE
 * DecisionTreeController.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections;
using System.IO;

public class DecisionTreeController : MonoBehaviour
{
    public bool useConfigFile = false;

    public string pathDecisionXMLFile = "";

    /// <summary>
    /// Make sure that it is TypeOfIntegration.NoTrafficIntegration in TrafficIntegrationController,
    /// or it will start reading some other simulation if defined in a config file.
    /// </summary>
    void Awake()
    {
        string path = Path.Combine(Application.dataPath, "confDecision.cfg");
        //Debug.Log("This is the path of the decision tree config file: " + path);

        //Use the confDecision.cgf in order to make the project portable
        if (File.Exists(path) == true && useConfigFile)
        {
            Debug.Log("Exists confDecision.cfg");
            string[] allLines = File.ReadAllLines(path);

            foreach (var line in allLines)
            {
                var split = line.Split(' ');
                if (split[0].Equals("pathDecisionXMLFile"))
                {
                    if (!split[1].Equals(null))
                        pathDecisionXMLFile = split[1];
                }
            }
        }
    }
}
