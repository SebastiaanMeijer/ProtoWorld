/*
 * 
 * CANVAS CHART MODULE
 * ChartKeeper.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// ToBeDecided, Bar, Pie, Line, StackedArea
/// </summary>
public enum UIChartTypes { ToBeDecided = 0, Bar = 1, Pie = 2, Line = 3, StackedArea = 4 }

/// <summary>
/// Old(?) Implementation of controlling the chart in UI canvas.
/// </summary>
public class ChartKeeper : MonoBehaviour
{

    public GameObject[] chartObjects;
    public GameObject prefabEventButton;

    private float buttonSpacing = 2;
    private Button lastButton { get; set; }
    private float DefaultTime { get; set; }

    private Dictionary<string, List<string>> messageDictionary = new Dictionary<string, List<string>>();
    /// <summary>
    /// key is the kpi index, value is the chat index.
    /// </summary>
    private Dictionary<string, string> kpiToChartMapping = new Dictionary<string, string>();

    private string[] actionStrings = new string[] { "Control" };
    private string[] dataClassStrings = new string[] { "KPIManager" /*, "SumoTrafficSpawner"*/ };
    private string[] specialDataClassStrings = new string[] { "SumoTrafficSpawner" };
    private Dictionary<string, string> defaultLegends = new Dictionary<string, string>
    {
        { "0", "Pedestrians per minute" },
        { "1", "Pedestrians outside the stadium" },
        { "2", "Pedestrians in the stadium" },
        { "3", "SecurityControl" }
    };

    // Use this for initialization
    void Start()
    {
        chartObjects = GameObject.FindGameObjectsWithTag("Chart");
        chartObjects = chartObjects.OrderBy(chart => (chart.transform.position - transform.position).sqrMagnitude).ToArray();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadIntroCanvas()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadFile(string path)
    {
        chartObjects = GameObject.FindGameObjectsWithTag("Chart");
        chartObjects = chartObjects.OrderBy(chart => (chart.transform.position - transform.position).sqrMagnitude).ToArray();
        CreateChartFromLogFile(path);
        //Debug.Log("Opening: " + path);
    }

    private void CreateChartFromLogFile(string path)
    {
        string[] logLines = File.ReadAllLines(path);
        InitCharts(logLines, dataClassStrings);
        ExtractAndSortDataFromLines(logLines, dataClassStrings);
        ParseDataToChart();
        //ConformColorToLegends();
        //DrawCharts();
        ExtractActionsToButtons(logLines, actionStrings);
    }



    /// <summary>
    /// Gather messages generate from the same KPIManager/Chart for later parsing
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="includeStrings"></param>
    /// <param name="methodStr"></param>
    private void ExtractAndSortDataFromLines(string[] lines, string[] includeStrings, string methodStr = "Update")
    {
        // 0 = chartIdx
        // 1 = seriesIdx
        // 2 = value
        //637   [1]  INFO KPIManager Update - 0:0:0 
        //6661  [1]  INFO SumoTrafficSpawner UpdateVehiclesInScene - Current number of vehicles in SUMO: 1
        foreach (var logLine in lines)
        {
            foreach (var str in includeStrings)
            {
                if (logLine.Contains(str) && logLine.Contains(methodStr))
                {
                    AddLogLineToDictionary(logLine);
                }
            }
        }
    }

    private void AddLogLineToDictionary(string logLine)
    {
        string str = GetLogMessage(logLine);
        int pos = str.LastIndexOf(":");
        string id = str.Substring(0, pos); // eg. "0:0" = chart 0: line 0
        List<string> list;
        if (!messageDictionary.TryGetValue(id, out list))
        {
            list = new List<string>();
            messageDictionary.Add(id, list);
        }
        list.Add(logLine);
    }

    /// <summary>
    /// Reconstruct the graphs from the log.
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="methodStr"></param>
    void InitCharts(string[] lines, string[] includeStrings, string methodStr = "Start")
    {
        // eg.: 600[1]  INFO KPIManager Start - 3:lineChart 0              
        foreach (var logLine in lines)
        {
            foreach (var str in includeStrings)
            {
                if (logLine.Contains(str) && logLine.Contains(methodStr))
                {
                    string logStr = GetLogMessage(logLine);
                    //Debug.Log(logStr);
                    string[] splits = logStr.Split(new Char[] { ':', ' ' });
                    kpiToChartMapping.Add(splits[0], splits[2]);
                    if (logLine.Contains("lineChart"))
                    {
                        //Debug.Log(logLine);
                        //Debug.Log(splits[2]);
                        SetChartType(UIChartTypes.Line, int.Parse(splits[2]));

                    }
                    else if (logLine.Contains("stackedAreaChart"))
                    {
                        SetChartType(UIChartTypes.StackedArea, int.Parse(splits[2]));
                    }
                }
            }
        }
    }

    private void SetChartType(UIChartTypes chartType, int chartIndex)
    {
        if (chartObjects.Length > chartIndex)
        {
            ChartController controller = chartObjects[chartIndex].GetComponent<ChartController>();
            //controller.SeriesCount += 1;
            controller.SetChartType(chartType);
        }
        else
        {
            Debug.LogError("Not enough graphs!");
        }
    }

    private void ParseDataToChart()
    {
        float time;
        float fValue;
        int idx;
        foreach (var item in messageDictionary)
        {
            string[] splits = item.Key.Split(':');
            string chartStr;
            string seriesName;
            if (kpiToChartMapping.TryGetValue(splits[0], out chartStr) && defaultLegends.TryGetValue(splits[0], out seriesName))
            {
                int chartIdx;
                if (int.TryParse(chartStr, out chartIdx))
                {
                    ChartController controller = chartObjects[chartIdx].GetComponent<ChartController>();
                    //controller.SeriesCount += 1;
                    //idx = controller.SeriesCount - 1;
                    idx = controller.SeriesCount;
                    controller.SetSeriesName(idx, seriesName);
                    foreach (string str in item.Value)
                    {
                        time = GetLogTimeInSecond(str);
                        splits = GetLogMessage(str).Split(':');
                        fValue = float.Parse(splits[2]);
                        controller.AddTimedData(idx, time, fValue);
                    }
                }
            }
        }
    }


    void ExtractActionsToButtons(string[] lines, string[] methodStr)
    {
        if (prefabEventButton == null)
        {
            Debug.LogError("You have to assign the prefabButton...");
            return;
        }
        float buttonHeight = prefabEventButton.GetComponent<RectTransform>().rect.height;

        GameObject panel = GameObject.Find("EventButtonPanel");
        if (panel == null)
        {
            Debug.LogError("EventButtonPanel missing...");
            return;
        }
        Vector3 startPosition = panel.transform.position;
        RectTransform panelTransform = panel.GetComponent<RectTransform>();

        float buttonPosition = -buttonSpacing;
        GameObject DefaultButton = Instantiate(prefabEventButton, startPosition, Quaternion.identity) as GameObject;
        DefaultButton.GetComponentInChildren<UnityEngine.UI.Text>().fontStyle = FontStyle.Bold;
        DefaultButton.transform.SetParent(panel.transform);
        DefaultButton.transform.localPosition = new Vector3(0, buttonPosition);
        Button defbutton = DefaultButton.GetComponent<Button>();
        defbutton.onClick.AddListener(delegate { HandleButtonEvent(defbutton, DefaultTime); });
        buttonPosition -= buttonHeight + buttonSpacing;

        foreach (var line in lines)
        {
            foreach (var str in methodStr)
            {
                if (line.Contains(str))
                {
                    GameObject buttonObject = Instantiate(prefabEventButton, startPosition, Quaternion.identity) as GameObject;
                    buttonObject.transform.SetParent(panel.transform);
                    buttonObject.transform.localPosition = new Vector3(0, buttonPosition);

                    buttonPosition -= buttonHeight + buttonSpacing;

                    UnityEngine.UI.Text buttonText = buttonObject.GetComponentInChildren<UnityEngine.UI.Text>();
                    //string timeString = GetLogTime(line);
                    float time = GetLogTimeInSecond(line);
                    buttonText.text = string.Concat(" ", ChartUtils.SecondsToTime(time), "\r\n [", GetLogMessage(line), "]");
                    //buttonText.text = string.Concat(" ", time.ToString(), " s\r\n [", GetLogMessage(line), "]");
                    Button button = buttonObject.GetComponent<Button>();
                    button.onClick.AddListener(delegate { HandleButtonEvent(button, time); });

                }
            }
        }
        panelTransform.sizeDelta = new Vector2(panelTransform.rect.width, Math.Abs(buttonPosition));
    }

    /// <summary>
    /// Change the normal and highlighted color of the button to indicate what 
    /// event the user has chosen to show on the charts. 
    /// Then it calls MoveEventLine(time) to move the event indicator to the 
    /// corresponding time in the chart.
    /// </summary>
    /// <param name="button">The button the user has pressed</param>
    /// <param name="time">The time of the event that is associated with the button</param>
    public void HandleButtonEvent(Button button, float time)
    {
        ColorBlock cb;
        if (lastButton != null)
        {
            cb = lastButton.colors;
            cb.normalColor = Color.white;
            lastButton.colors = cb;
        }
        cb = button.colors;
        cb.normalColor = new Color32(255, 209, 159, 255);
        cb.highlightedColor = new Color32(255, 209, 159, 255);
        button.colors = cb;
        lastButton = button;
        MoveEventLine(time);
    }

    public void MoveEventLine(float time)
    {
        foreach (var chart in chartObjects)
        {
            ChartController controller = chart.GetComponent<ChartController>();
            if (controller != null)
            {
                controller.SetEventTime(time);
            }
        }
    }

    /// <summary>
    /// Change this will change the way to read the log message.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static string GetLogMessage(string line)
    {
        return line.Substring(line.LastIndexOf("-") + 2);
    }

    static string GetLogValue(string line)
    {
        return line.Substring(line.LastIndexOf(":") + 1);
    }

    /// <summary>
    /// Get the timestamp in milliseconds.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static string GetLogTime(string line)
    {
        return line.Substring(0, line.IndexOf(" "));
    }

    /// <summary>
    /// Get the timestamp in seconds.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    static float GetLogTimeInSecond(string line)
    {
        return Mathf.RoundToInt(float.Parse(GetLogTime(line)) / 1000);
    }
}
