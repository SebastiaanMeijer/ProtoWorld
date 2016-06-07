using UnityEngine;
using System.Collections;

public class LoadLogButtonController : MonoBehaviour
{
    //public ChartKeeper chartKeeper;
    //public GameObject chartPanel;

    //private LogContainer logContainer;

    //public LogContainer LogData
    //{
    //    get { return logContainer; }
    //    set { logContainer = value; }
    //}

    public FileDialogController fileChooser;

    public LogController logController;

    private bool askingForFile;

    // Use this for initialization
    void Start()
    {
        askingForFile = false;
        
        //if (chartPanel == null)
        //{
        //    Debug.LogError("No ChartPanel in Scene, please add one.");
        //}
        //chartPanel.SetActive(false);
        //if (chartKeeper == null)
        //{
        //    chartKeeper = FindObjectOfType<ChartKeeper>();
        //}

        if (fileChooser == null)
        {
            fileChooser = FindObjectOfType<FileDialogController>();
            if (fileChooser == null)
                Debug.LogError("No FileDialog in Scene, please add one.");
        }

        if (logController == null)
        {
            logController = FindObjectOfType<LogController>();
            if (logController == null)
                Debug.LogError("No LogController in Scene, please add one.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (askingForFile)
        {
            if (fileChooser.IsFileChoosen)
            {
                if (fileChooser.FilePath.Exists)
                {
                    logController.ReadLog(fileChooser.FilePath.FullName);

                    //chartPanel.SetActive(true);
                    //logContainer = ScriptableObject.CreateInstance<LogContainer>();
                    //if (!logContainer.ReadLog(fileChooser.FilePath.FullName))
                    //    chartKeeper.LoadFile(fileChooser.FilePath.FullName);
                }
                askingForFile = false;
            }
        }
    }

    public void LoadLogButtonHandler()
    {
        askingForFile = true;
        fileChooser.ShowDialog("Load Log File", @"C:\Users\admgaming\Documents\UNITY_TEST_BUILD");
    }

}
