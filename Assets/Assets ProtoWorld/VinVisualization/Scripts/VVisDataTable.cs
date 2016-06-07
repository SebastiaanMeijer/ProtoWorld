/*
 * 
 * VIN VISUALIZATION
 * VVisDataTable.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Data;
using GenericParsing;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System;

/// <summary>
/// This script holds the operations for reading, sorting and selecting the data of the VVIS CSV file. 
/// </summary>
[Obsolete("This class is not used anymore in the newer versions of VVIS (0.3 and higher)")]
public class VVisDataTable : MonoBehaviour
{
    public string csvFilePath = "";

    private DataTable vVisDataTable;
    private GenericParserAdapter vVisParser;
    private TimeController timeCtrl;
    private Thread thread;

    private bool parsingCompleted = false;

    private bool selectionRequestCompleted = true;
    private DataRow[] selectedRows;

    /// <summary>
    /// Awakes the script.
    /// </summary>
    void Awake()
    {
        vVisDataTable = new DataTable();
        timeCtrl = FindObjectOfType<TimeController>();
    }

    /// <summary>
    /// Starts the script and loads the CSV file into the DataTable. 
    /// </summary>
    void Start()
    {
        if (File.Exists(csvFilePath))
        {
            ThreadStart ts = new ThreadStart(LoadDataTable);
            thread = new Thread(ts);
            UnityEngine.Debug.Log("Thread for CSV parsing created");
            thread.Start();

            StartCoroutine(DisplayTableWhenParsingCompleted());
        }
        else
        {
            UnityEngine.Debug.LogWarning("The path of the CSV file does not exist");
        }
    }

    /// <summary>
    /// Checks if the parsing of the CSV file is completed.
    /// </summary>
    /// <returns></returns>
    public bool IsParsingCompleted()
    {
        return parsingCompleted;
    }

    /// <summary>
    /// Creates a request of a data selection over the DataTable. 
    /// </summary>
    /// <param name="requestor">Owner of the requested call. A message RequestDataSelection_CallBack 
    /// will be sent to this gameObject with a DataRow[] object containing the selection.</param>
    /// <param name="filterExpression">Expression for the selection.</param>
    /// <param name="sort">Expression for sorting (null by default).</param>
    public void RequestDataSelection(GameObject requestor, string filterExpression, string sort = null)
    {
        if (parsingCompleted && selectionRequestCompleted == true)
        {
            thread.Abort();
            thread = new Thread(() => SelectData(filterExpression, sort));
            UnityEngine.Debug.Log("Thread for selecting request created");
            selectionRequestCompleted = false;
            thread.Start();

            StartCoroutine(CallbackToRequestor(requestor));
        }
    }

    /// <summary>
    ///  Auxiliar method for threading. 
    /// Selects data from the DataTable. 
    /// </summary>
    /// <param name="filterExpression">Expression for the selection.</param>
    /// <param name="sort">Expression for sorting (null by default).</param>
    void SelectData(string filterExpression, string sort = null)
    {
        selectionRequestCompleted = false;
        Stopwatch diagnosticTime = new Stopwatch();
        diagnosticTime.Start();

        try
        {
            UnityEngine.Debug.Log("Calculating...");

            if (sort != null)
                selectedRows = vVisDataTable.Select(filterExpression, sort);
            else
                selectedRows = vVisDataTable.Select(filterExpression);

            UnityEngine.Debug.Log("Select calculation completed");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error calculating the select request: " + e.Message);
        }
        finally
        {
            diagnosticTime.Stop();
            UnityEngine.Debug.Log("Time needed to execute query (s): " + diagnosticTime.Elapsed.TotalSeconds + "; Number of elements selected: " + selectedRows.Length);

            selectionRequestCompleted = true;
        }
    }

    /// <summary>
    /// Auxiliar method for threading. 
    /// Parses the CSV file into an in-memory database (DataTable).
    /// </summary>
    void LoadDataTable()
    {
        parsingCompleted = false;

        if (timeCtrl != null)
            timeCtrl.ShowLoadingIcon(true);

        Stopwatch diagnosticTime = new Stopwatch();
        diagnosticTime.Start();

        try
        {
            UnityEngine.Debug.Log("Parsing...");

            vVisParser = new GenericParserAdapter(csvFilePath);
            vVisParser.FirstRowHasHeader = true;

            vVisDataTable.Clear();
            vVisDataTable = vVisParser.GetDataTable();

            UnityEngine.Debug.Log("Parsing process completed");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error parsing the CSV file: " + e.Message);
        }
        finally
        {
            vVisParser.Close();
            diagnosticTime.Stop();
            UnityEngine.Debug.Log("Time needed to parse (s): " + diagnosticTime.Elapsed.TotalSeconds);

            if (timeCtrl != null)
                timeCtrl.ShowLoadingIcon(false);

            parsingCompleted = true;
        }
    }

    /// <summary>
    /// Coroutine for callback.
    /// </summary>
    /// <param name="requestor">Receiver of the callback.</param>
    IEnumerator CallbackToRequestor(GameObject requestor)
    {
        while (!selectionRequestCompleted)
            yield return new WaitForSeconds(0.1f);

        requestor.SendMessage("RequestDataSelection_CallBack", selectedRows);
    }

    /// <summary>
    /// Corroutine for displaying data table (for debugging purposes).
    /// </summary>
    IEnumerator DisplayTableWhenParsingCompleted()
    {
        while (!parsingCompleted)
            yield return new WaitForSeconds(0.1f);

        UnityEngine.Debug.Log("DataTable info after parsing: ");

        UnityEngine.Debug.Log("Table " + vVisDataTable.TableName + "\n" +
            vVisDataTable.Columns.Count + " columns\n" + vVisDataTable.Rows.Count +
            " rows\n");

        for (int k = 0; k < vVisDataTable.Columns.Count; k++)
        {
            UnityEngine.Debug.Log("Column " + k + ": " + vVisDataTable.Columns[k].ColumnName);
        }
    }

    /// <summary>
    /// Releases the resources when the object containing the script is being destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (thread != null)
        {
            thread.Abort();
        }
    }

    /// <summary>
    /// When application quits, aborts the thread. 
    /// </summary>
    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Application quitting! Abort CSV parser");
        if (thread != null)
            thread.Abort();
        return;
    }
}
