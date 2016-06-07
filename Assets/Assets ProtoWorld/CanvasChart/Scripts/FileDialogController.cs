using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class FileDialogController : MonoBehaviour
{
    /// <summary>
    /// Default path.
    /// </summary>
    //public const string defaultPath = @"C:\Users\admgaming\Documents\UNITY_TEST_BUILD";
    public const string defaultPath = @"C:\Users\admgaming\Documents\GapslabsProject\bin\";

    /// <summary>
    /// Prefab directory button for visulization.
    /// </summary>
    public GameObject dirButton;
    /// <summary>
    /// Prefab file button for visualization.
    /// </summary>
    public GameObject fileButton;

    public GameObject fileChooserPanel;
    public GameObject titleObject;
    public InputField directoryField;
    public InputField fileField;
    public RectTransform dirPanel;
    public RectTransform filePanel;
    public RectTransform dirView;
    public RectTransform fileView;

    float buttonSpacing = 2;
    float dirButtonHeight;
    float fileButtonHeight;

    /// <summary>
    /// To indicate that the user has chosen a file.
    /// </summary>
    public bool IsFileChoosen { get; private set; }

    /// <summary>
    /// The Path to the choosen file, can be empty.
    /// </summary>
    public FileInfo FilePath { get; private set; }

    void Awake()
    {
        if (dirButton == null)
        {
            dirButton = transform.Find("DirButton").gameObject;
            if (dirButton == null)
                Debug.LogWarning("Please set the prefab for dirButton");
        }
        if (fileButton == null)
        {
            fileButton = transform.Find("FileButton").gameObject;
            if (fileButton == null)
                Debug.LogWarning("Please set the prefab for fileButton");
        }
    }


    // Use this for initialization
    void Start()
    {
        IsFileChoosen = false;
        SetVisibility(false);
        dirButtonHeight = dirButton.GetComponent<RectTransform>().rect.height;
        fileButtonHeight = fileButton.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (RectTransform item in dirPanel)
        {
            item.sizeDelta = new Vector2(dirPanel.rect.width, dirButtonHeight);
        }
        foreach (RectTransform item in filePanel)
        {
            item.sizeDelta = new Vector2(filePanel.rect.width, fileButtonHeight);

        }
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
            GameObject buttonObject = Instantiate(dirButton, Vector3.zero, Quaternion.identity) as GameObject;
            buttonObject.SetActive(true);
            buttonObject.transform.SetParent(dirPanel);
            buttonObject.transform.localPosition = new Vector3(0, buttonPosition);
            buttonPosition -= dirButtonHeight + buttonSpacing;

            UnityEngine.UI.Text buttonText = buttonObject.GetComponentInChildren<UnityEngine.UI.Text>();
            buttonObject.name = dir.FullName;
            buttonText.text = dir.Name;
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(delegate { HandleDirButton(buttonObject); });
        }
        dirPanel.sizeDelta = new Vector2(dirPanel.rect.width, Mathf.Max(dirView.rect.height, Mathf.Abs(buttonPosition)));
    }

    float AddParentPathButton(float position, DirectoryInfo dir)
    {
        if (dir.Parent == null)
        {
            return position;
        }
        GameObject buttonObject = Instantiate(dirButton, Vector3.zero, Quaternion.identity) as GameObject;
        buttonObject.SetActive(true);
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
            GameObject buttonObject = Instantiate(fileButton, Vector3.zero, Quaternion.identity) as GameObject;
            buttonObject.SetActive(true);
            buttonObject.transform.SetParent(filePanel);
            buttonObject.transform.localPosition = new Vector3(0, buttonPosition);
            buttonPosition -= fileButtonHeight + buttonSpacing;

            UnityEngine.UI.Text buttonText = buttonObject.GetComponentInChildren<UnityEngine.UI.Text>();
            buttonObject.name = file.FullName;
            buttonText.text = file.Name;
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(delegate { HandleFileButton(buttonObject); });
        }
        filePanel.sizeDelta = new Vector2(filePanel.rect.width, Mathf.Max(fileView.rect.height, Mathf.Abs(buttonPosition)));

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

    public void SetTitle(string title)
    {
        titleObject.GetComponent<Text>().text = title;
    }

    public void ShowDialog(string title, string filePath = defaultPath)
    {
        SetVisibility(true);
        SetTitle(title);
        IsFileChoosen = false;
        Visualize(new DirectoryInfo(filePath));
    }

    public void SetVisibility(bool visibility)
    {
        fileChooserPanel.SetActive(visibility);
    }

    public void GoButtonHandler()
    {
        if (!string.IsNullOrEmpty(directoryField.text))
        {
            Visualize(new DirectoryInfo(directoryField.text));
        }
    }

    public void ChooseButtonHandler()
    {
        IsFileChoosen = true;
        FilePath = new FileInfo(fileField.text);
        SetVisibility(false);
    }

    public void CancelButtonHandler()
    {
        IsFileChoosen = true;
        FilePath = new FileInfo("");
        SetVisibility(false);
    }
}
