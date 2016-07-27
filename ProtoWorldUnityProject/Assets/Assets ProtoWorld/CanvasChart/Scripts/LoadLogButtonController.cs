/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * KPI MODULE
 * Johnson Ho
 * 
 */

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
