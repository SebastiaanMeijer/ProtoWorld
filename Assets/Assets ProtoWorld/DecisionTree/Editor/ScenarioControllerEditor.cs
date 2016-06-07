using UnityEngine;
using UnityEditor;
using System.Collections;

public class ScenarioControllerEditor : MonoBehaviour 
{
    [MenuItem("Traffic Integration/Scenarios/Read Tree (.xml)", false, 1)]
    public static void ReadTree()
    {
        var path = EditorUtility.OpenFilePanel("Load Tree Data", "", "xml");

        if (path.Length == 0)
        {
            EditorUtility.DisplayDialog("Loading Cancelled", "No file was provided", "OK");
            return;
        }
        var tree = DecisionTree.Load(path);

        var doTest = true;

        if (doTest)
        {
            tree.WriteToDebug();
            var sc = tree.GetFirstScenario();
            Debug.Log(sc.FindSimulationPath());
            Debug.Log(" Time 0: " + sc.FindNextEventIndex(0));
            Debug.Log(" Time 1: " + sc.FindNextEventIndex(1));
            sc = tree.GetScenario(3);
            if (sc != null)
            {
                Debug.Log(sc.FindSimulationPath());
                Debug.Log(" Time 0: " + sc.FindNextEventIndex(0));
                Debug.Log(" Time 1: " + sc.FindNextEventIndex(1));
                Debug.Log(" Time 5.5: " + sc.FindNextEventIndex(5.5f));
                Debug.Log(" Time 30: " + sc.FindNextEventIndex(30));
                Debug.Log(" Time 31: " + sc.FindNextEventIndex(31));
            }
        }
    }
}
