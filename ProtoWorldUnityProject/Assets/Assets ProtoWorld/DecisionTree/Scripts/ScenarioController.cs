/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DECISION TREE MODULE
 * ScenarioController.cs
 * Johnson Ho
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

public class ScenarioController : MonoBehaviour 
{

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static string moduleName = "DecisionTreeModule";

    protected DecisionTree decisionTree;

    protected DecisionTreeController decisionTreeController;

    protected TrafficIntegrationData trafficDB;

    protected DecisionUIController decisionUI;

    private Scenario currentScenario;

    private TimeController timeController;

    void Awake()
    {
        // Load the decision tree
        decisionTreeController = FindObjectOfType<DecisionTreeController>();
        if (decisionTreeController != null)
        {
            var path = decisionTreeController.pathDecisionXMLFile;
            decisionTree = DecisionTree.Load(path);
        }
        else
            Debug.LogWarning("Scenario controller could not get the Decision Tree Controller");

        // Get the traffic DB
        TrafficIntegrationController tic = FindObjectOfType<TrafficIntegrationController>();
        if (tic != null)
        {
            if (tic.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.DecisionTreeIntegration)
            {
                trafficDB = FindObjectOfType<TrafficIntegrationData>();

                if (trafficDB == null)
                    Debug.LogWarning("Scenario controller could not get the Traffic Integration Data");
            }
            else
                Debug.LogWarning("Traffic Integration Controller not set for using decision tree integration");
        }

        // Get the DecisionUIController
        decisionUI = FindObjectOfType<DecisionUIController>();

        // Get the TimeController
        timeController = FindObjectOfType<TimeController>();
    }

    // Use this for initialization
    void Start()
    {
        LoadFirstScenario();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScenario != null && !timeController.IsPaused())
        {
            currentScenario.TryDoNextEvent(timeController.gameTime);
        }
    }

    public void AnswerQuestion(int questionId, int choice)
    {
        if (currentScenario != null)
        {
            currentScenario.AnswerQuestion(questionId, choice);
        }
    }

    public void ShowQuestionCanvas(SimQuestion q)
    {
        if (decisionUI != null)
            decisionUI.ShowQuestionCanvas(q);
    }

    public void LoadFirstScenario()
    {
        if (decisionTree != null)
        {
            var id = decisionTree.GetFirstScenario().id;
            LoadScenario(id);
        }
    }

    public void LoadScenario(int index)
    {
        Debug.Log("Loading scenario " + index);
        log.Info("Loading scenario " + index);

        if (decisionTree == null)
            return;
        if (trafficDB == null)
            return;

        var sc = decisionTree.GetScenario(index);
        if (sc == null)
            return;

        // Round up game time to avoid loops when going back
        timeController.gameTime += 0.1f;

        if (currentScenario != null && currentScenario.id == sc.id)
        {
            Debug.Log("Scenario already in memory, no need to load it again");
            return;
        }

        trafficDB.ResetTrafficDB();

        var reader = SimulationReader.FindSimulationReader();

        if (sc.simulation != null)
        {
            //reader.SetFileName(sc.FindSimulationPath());
            reader.SetFileName(sc.simulation.path);
            reader.SetUseNetworkStream(false);
            reader.SetIO(new ProtoMetaIO());
            reader.SetTrafficDB(trafficDB);

            // Start a threaded read.
            reader.ThreadUpdate();
        }

        // Set the current scenario with a delay to allow threading start
        StartCoroutine("SetCurrentScenario", sc);
    }

    private IEnumerator SetCurrentScenario(Scenario sc)
    {
        yield return new WaitForSeconds(2.0f);

        Debug.Log("Setting current scenario now");
        sc.UpdateEventIndex(timeController.gameTime);
        timeController.ChangeScenarioTitle(sc.name);
        currentScenario = sc;
    }
}
