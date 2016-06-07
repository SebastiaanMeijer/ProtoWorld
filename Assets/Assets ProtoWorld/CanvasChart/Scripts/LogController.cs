using UnityEngine;
using System.Collections;

public class LogController : MonoBehaviour
{
    public bool readSuccess = false;
    public bool saveSuccess = false;

    private LogContainer logContainer;

    public LogContainer LogData
    {
        get { return logContainer; }
        set { logContainer = value; }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool ReadLog(string filePath)
    {
        logContainer = new LogContainer();
        readSuccess = logContainer.ReadLog(filePath);
        return readSuccess;
    }

    public void SaveLog(string filePath)
    {
        if (logContainer != null)
        {
            logContainer.SaveAsXML(filePath);
        }
        else
        {
            Debug.Log("no logContainer yet.");
        }
    }
}
