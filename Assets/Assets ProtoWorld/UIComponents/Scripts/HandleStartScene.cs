using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;

public class HandleStartScene : MonoBehaviour {

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public GameObject introCanvas, nameCanvas, settingCanvas;
    public bool confSestar, confSUMO, integSEStar, integSUMO;
    public IPAddress sestarAddress, sumoAddress;
    public string userName;
    public ushort sestarTCP, sestarUDP, sumoTCP, sumoUDP;

    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            //string str = level.ToString();
            //Debug.Log(str);
            log.Info(level.ToString());
        }
    }

    void OnApplicationQuit()
    {
        log.Info("Application Quit");
    }

    //void Awake()
    //{
    //    introCanvas = GameObject.Find("IntroCanvas");
    //    nameCanvas = GameObject.Find("NameCanvas");
    //    settingCanvas = GameObject.Find("SettingCanvas");
    //}

    void Start()
    {
        GotoIntroCanvas();
    }

	
	// Update is called once per frame
	void Update () {
	
	}

    public void GotoIntroCanvas()
    {
        introCanvas.SetActive(true);
        nameCanvas.SetActive(false);
        settingCanvas.SetActive(false);
    }

    public void GotoNameCanvas()
    {
        introCanvas.SetActive(false);
        nameCanvas.SetActive(true);
        settingCanvas.SetActive(false);
    }

    public void GotoSettingCanvas()
    {
        introCanvas.SetActive(false);
        nameCanvas.SetActive(false);
        settingCanvas.SetActive(true);
    }
    public void GotoSimulationScene()
    {
        userName = GameObject.Find("UserNameField").GetComponent<InputField>().text;
        //print(userName);
        Application.LoadLevel(1);
    }

    public void GotoLoadLogScene()
    {
        Application.LoadLevel(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DefaultSettings()
    {
        GameObject.Find("ConfigSEStar").GetComponent<Toggle>().isOn = true;
        GameObject.Find("ConfigSUMO").GetComponent<Toggle>().isOn = true;

        GameObject.Find("IntegrateSEStar").GetComponent<Toggle>().isOn = true;
        GameObject.Find("SEStarAddress").GetComponent<InputField>().text = "127.0.0.1";
        GameObject.Find("SEStarTCP").GetComponent<InputField>().text = "6112";
        GameObject.Find("SEStarUDP").GetComponent<InputField>().text = "10120";

        GameObject.Find("IntegrateSUMO").GetComponent<Toggle>().isOn = true;
        GameObject.Find("SUMOAddress").GetComponent<InputField>().text = "127.0.0.1";
        GameObject.Find("SumoTCP").GetComponent<InputField>().text = "3456";
        GameObject.Find("SumoUDP").GetComponent<InputField>().text = "3654";

    }

    bool CheckSettings()
    {
        string testString;
        
        testString = GameObject.Find("SEStarAddress").GetComponent<InputField>().text;
        if (!IPAddress.TryParse(testString, out sestarAddress))
        {
            Debug.LogWarning("wrong SEStarAddress format");
            return false;
        }
        testString = GameObject.Find("SUMOAddress").GetComponent<InputField>().text;
        if (!IPAddress.TryParse(testString, out sumoAddress))
        {
            Debug.LogWarning("wrong SUMOAddress format");
            return false;
        }
        testString = GameObject.Find("SEStarTCP").GetComponent<InputField>().text;
        if (!ushort.TryParse(testString, out sestarTCP))
        {
            Debug.LogWarning("wrong SEStarTCP format");
            return false;
        }
        testString = GameObject.Find("SEStarUDP").GetComponent<InputField>().text;
        if (!ushort.TryParse(testString, out sestarUDP))
        {
            Debug.LogWarning("wrong SEStarUDP format");
            return false;
        }
        testString = GameObject.Find("SumoTCP").GetComponent<InputField>().text;
        if (!ushort.TryParse(testString, out sumoTCP))
        {
            Debug.LogWarning("wrong SumoTCP format");
            return false;
        }
        testString = GameObject.Find("SumoUDP").GetComponent<InputField>().text;
        if (!ushort.TryParse(testString, out sumoUDP))
        {
            Debug.LogWarning("wrong SumoUDP format");
            return false;
        }

        confSestar = GameObject.Find("ConfigSEStar").GetComponent<Toggle>().isOn;
        confSUMO = GameObject.Find("ConfigSUMO").GetComponent<Toggle>().isOn;
        integSEStar = GameObject.Find("IntegrateSEStar").GetComponent<Toggle>().isOn;
        integSUMO = GameObject.Find("IntegrateSUMO").GetComponent<Toggle>().isOn;

        return true;
    }

    public void ApplySettings()
    {
        bool checkOK = CheckSettings();

        if (checkOK)
        {
            // Apply the settings here...
            GotoIntroCanvas();
        }
    }

}
