using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.UI;

public class FileKeeper : MonoBehaviour
{
    public string LogFilePath { get; private set; }
    string defaultPath = @"C:\Users\admgaming\Documents\GapslabsProject\bin\";

    //private ChartKeeper chartKeeper;
    public GameObject fileDialog;
    public InputField directoryField;
    public InputField fileField;
    public GameObject prefabDirButton;
    public GameObject prefabFileButton;
    public RectTransform dirPanel;
    public RectTransform filePanel;
    public RectTransform dirView;
    public RectTransform fileView;

    float buttonSpacing = 2;
    float dirButtonHeight;
    float fileButtonHeight;

    // Use this for initialization
    void Start()
    {
        //chartKeeper = GetComponent<ChartKeeper>();
        //if (chartKeeper == null)
        //{
        //    Debug.LogWarning("no chartkeeper");
        //    return;
        //}

        DirectoryInfo defaultDirectory = new DirectoryInfo(defaultPath);
        dirButtonHeight = prefabDirButton.GetComponent<RectTransform>().rect.height;
        fileButtonHeight = prefabFileButton.GetComponent<RectTransform>().rect.height;
        Visualize(defaultDirectory);
    }

    public void HandleLoadLogButton()
    {
        fileDialog.SetActive(true);
    }

    void Visualize(DirectoryInfo path)
    {
        if (path.Exists)
        {
            VisualizeDirectories(path);
            VisualizeFiles(path);
        }
        else
        {
            directoryField.placeholder.GetComponent<UnityEngine.UI.Text>().text = "Can't find " + path.FullName;
            directoryField.text = "";
        }

    }

    void VisualizeDirectories(DirectoryInfo path)
    {
        foreach (Transform item in dirPanel)
        {
            GameObject.Destroy(item.gameObject);
        }
        directoryField.text = path.FullName;
        //float buttonPosition = -buttonSpacing;
        float buttonPosition = AddParentPathButton(-buttonSpacing, path);
        foreach (var dir in GetDirectories(path))
        {
            GameObject buttonObject = Instantiate(prefabDirButton, Vector3.zero, Quaternion.identity) as GameObject;
            buttonObject.transform.SetParent(dirPanel);
            buttonObject.transform.localPosition = new Vector3(0, buttonPosition);
            buttonPosition -= dirButtonHeight + buttonSpacing;

            UnityEngine.UI.Text buttonText = buttonObject.GetComponentInChildren<UnityEngine.UI.Text>();
            buttonObject.name = dir.FullName;
            buttonText.text = dir.Name;
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(delegate { HandleDirButton(buttonObject); });
        }
        dirPanel.sizeDelta = new Vector2(dirPanel.rect.width, Math.Max(dirView.rect.height, Math.Abs(buttonPosition)));
    }

    float AddParentPathButton(float position, DirectoryInfo dir)
    {
        if (dir.Parent == null)
        {
            return position;
        }
        GameObject buttonObject = Instantiate(prefabDirButton, Vector3.zero, Quaternion.identity) as GameObject;
        buttonObject.transform.SetParent(dirPanel);
        buttonObject.transform.localPosition = new Vector3(0, position);
        position -= fileButtonHeight + buttonSpacing;

        UnityEngine.UI.Text buttonText = buttonObject.GetComponentInChildren<UnityEngine.UI.Text>();
        buttonObject.name = dir.Parent.FullName;
        buttonText.text = "[Up one level..]";
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(delegate { HandleDirButton(buttonObject); });

        return position;
    }

    void VisualizeFiles(DirectoryInfo path)
    {
        foreach (Transform item in filePanel)
        {
            GameObject.Destroy(item.gameObject);
        }
        float buttonPosition = -buttonSpacing;
        foreach (var file in GetFiles(path))
        {
            GameObject buttonObject = Instantiate(prefabFileButton, Vector3.zero, Quaternion.identity) as GameObject;
            buttonObject.transform.SetParent(filePanel);
            buttonObject.transform.localPosition = new Vector3(0, buttonPosition);
            buttonPosition -= fileButtonHeight + buttonSpacing;

            UnityEngine.UI.Text buttonText = buttonObject.GetComponentInChildren<UnityEngine.UI.Text>();
            buttonObject.name = file.FullName;
            buttonText.text = file.Name;
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(delegate { HandleFileButton(buttonObject); });
        }
        filePanel.sizeDelta = new Vector2(filePanel.rect.width, Math.Max(fileView.rect.height, Math.Abs(buttonPosition)));

    }

    DirectoryInfo[] GetDirectories(DirectoryInfo path)
    {
        return path.GetDirectories();
    }

    FileInfo[] GetFiles(DirectoryInfo path)
    {
        return path.GetFiles();
    }

    public void HandleDirButton(GameObject dirButton)
    {
        Visualize(new DirectoryInfo(dirButton.name));
    }

    public void HandleFileButton(GameObject fileButton)
    {
        fileField.text = fileButton.name;
    }

    public void HandleGoButton()
    {
        if (!string.IsNullOrEmpty(directoryField.text))
        {
            Visualize(new DirectoryInfo(directoryField.text));
        }
    }

    public void HandleOpenButton()
    {
        fileDialog.SetActive(false);
        LogFilePath = fileField.text;
        //chartKeeper.LoadFile(fileField.text);
    }

    public void HandleCancelButton()
    {
        fileDialog.SetActive(false);
        LogFilePath = null;
    }




}
