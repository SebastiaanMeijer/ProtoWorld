using UnityEngine;
using System.Collections;

public class SaveLogButtonController : MonoBehaviour
{

    public FileDialogController fileChooser;

    public LogController logController;

    private bool askingForFile;

    // Use this for initialization
    void Start()
    {
        askingForFile = false;
        
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
                    // Check if user wants to overwrite?
                    logController.SaveLog(fileChooser.FilePath.FullName);
                }
                else
                {
                    logController.SaveLog(fileChooser.FilePath.FullName);
                }
                askingForFile = false;
            }
        }
    }

    public void SaveLogButtonHandler()
    {
        askingForFile = true;
        fileChooser.ShowDialog("Save Log File", @"C:\Users\admgaming\Documents\UNITY_TEST_BUILD");
    }

}
