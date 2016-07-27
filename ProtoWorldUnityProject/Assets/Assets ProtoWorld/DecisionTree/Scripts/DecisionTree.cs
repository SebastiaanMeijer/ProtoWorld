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
 * DecisionTree.cs
 * Johnson Ho
 * 
 */

using UnityEngine;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;

[XmlRoot("decision_tree")]
public class DecisionTree
{
    [XmlArrayItem("scenario")]
    public List<Scenario> scenarios = new List<Scenario>();

    private bool sorted = false;

    public Scenario GetFirstScenario()
    {
        if (!sorted)
        {
            scenarios.Sort((s1, s2) => s1.id.CompareTo(s2.id));
            sorted = true;
        }
        return scenarios[0];
    }

    public Scenario GetScenario(int id)
    {
        return scenarios.Find(s => s.id.Equals(id));
    }

    public void WriteToDebug()
    {
        foreach (var s in scenarios)
        {
            s.WriteToDebug();

        }
    }

    public static DecisionTree Load(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(DecisionTree));
            return serializer.Deserialize(stream) as DecisionTree;
        }
    }

    public void Save(string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            var serializer = new XmlSerializer(typeof(DecisionTree));
            serializer.Serialize(stream, this);
        }
    }
}

public class Scenario
{
    [XmlAttribute]
    public int id;

    [XmlAttribute]
    public string name;

    [XmlElement("simulation")]
    public SimulationPath simulation;

    [XmlElement("event")]
    public SimulationEvent[] events;

    [XmlElement("question")]
    public SimQuestion[] questions;

    private List<SimulationEvent> sortedEventList;

    private int currentEventIndex = 0;

    public void WriteToDebug()
    {
        string str = "SCENARIO id: " + id + " name: " + name;
        Debug.Log(str);
        foreach (var e in events)
        {
            e.WriteToDebug();
        }

        foreach (var q in questions)
        {
            q.WriteToDebug();
        }
    }

    public string FindSimulationPath()
    {
        foreach (var e in events)
        {
            var path = e.FindSimulationPath();
            if (path != null)
                return path;
        }
        return null;
    }

    public List<SimulationEvent> GetSortedEvents()
    {
        if (sortedEventList == null)
        {
            sortedEventList = new List<SimulationEvent>();

            if (events != null)
                sortedEventList.AddRange(events);

            if (questions != null)
                sortedEventList.AddRange(questions);

            sortedEventList.Sort((ev1, ev2) => ev1.time.CompareTo(ev2.time));
        }
        return sortedEventList;
    }

    public void TryDoNextEvent(float time)
    {
        var stopIndex = FindNextEventIndex(time);
        for (int i = currentEventIndex; i < stopIndex; i++)
        {
            // Play the event
            Debug.Log("Playing event: ");
            GetSortedEvents()[i].WriteToDebug();
            GetSortedEvents()[i].Play();
        }
        currentEventIndex = stopIndex;
    }


    public int FindNextEventIndex(float time)
    {
        int index = GetSortedEvents().FindIndex(ev => ev.time >= time);

        if (index == -1)
            // For the last event, return the count
            return GetSortedEvents().Count;
        else
            return index;
    }

    /// <summary>
    /// This method is used every time the scenario is loaded again.
    /// </summary>
    /// <param name="time">Game time.</param>
    public void UpdateEventIndex(float time)
    {
        currentEventIndex = FindNextEventIndex(time);
    }

    public SimQuestion FindQuestion(int id)
    {
        foreach (var q in questions)
        {
            if (q.id.Equals(id))
            {
                return q;
            }
        }
        return null;
    }

    public void AnswerQuestion(int questionId, int choiceIndex)
    {
        var question = FindQuestion(questionId);

        if (question != null)
            question.ExecuteChoice(choiceIndex);
    }
}

public class SimulationPath
{
    [XmlAttribute]
    public string path;
}

public class SimulationEvent
{
    [XmlAttribute]
    public int id;

    [XmlAttribute]
    public float time;

    [XmlAnyElement]
    public XmlElement[] actions;

    public enum EventType
    {
        simulation,
        defaultEvent,
        weather,
        spawn,
        inform
    }

    public virtual void WriteToDebug()
    {
        string str = "EVENT id: " + id + " t: " + time + " actions: ";
        Debug.Log(str);
        foreach (var a in actions)
        {
            Debug.Log("name: " + a.LocalName + " action: " + a.InnerText);
        }
    }

    public string FindSimulationPath()
    {
        foreach (var a in actions)
        {
            if (a.LocalName.Equals("simulation"))
                return a.InnerText;
        }
        return null;
    }

    public virtual void Play()
    {
        EventType et;
        foreach (var a in actions)
        {
            et = (EventType)Enum.Parse(typeof(EventType), a.LocalName);
            switch (et)
            {
                case EventType.weather:
                    // Change the weather here.
                    Debug.Log("Changing weather");
                    break;
                case EventType.spawn:
                    // Spawn something here.
                    Debug.Log("Spawning something");
                    break;
                case EventType.inform:
                    // Show something on the Canvas here.
                    Debug.Log("Informing: " + a.InnerText);
                    break;
            }
        }
    }
}

public class SimQuestion : SimulationEvent
{
    [XmlAttribute]
    public string text;

    [XmlAttribute]
    public int landmark;

    [XmlElement("choice")]
    public SimChoice[] choices;

    private DecisionUIController decisionUI;

    // Constructor
    SimQuestion()
    {
        decisionUI = GameObject.FindObjectOfType<DecisionUIController>();
    }

    public override void WriteToDebug()
    {
        string str = "QUESTION id: " + id + " q: " + text + " t: " + time + " choices: ";
        Debug.Log(str);

        if (choices != null)
        {
            foreach (var c in choices)
            {
                c.WriteToDebug();
            }
        }
    }

    public override void Play()
    {
        Debug.Log("Time for a question");

        if (decisionUI != null)
            decisionUI.ShowQuestionCanvas(this);
    }

    public void ExecuteChoice(int choiceIndex)
    {
        if (choices != null && choiceIndex < choices.Length)
            choices[choiceIndex].Execute();
    }
}

public class SimChoice
{
    [XmlAttribute]
    public string text;

    [XmlAnyElement]
    public XmlElement[] executables;

    public enum ExecType
    {
        defaultExec,
        execScen,
        execEvent
    }

    private ScenarioController scenarioController;

    SimChoice()
    {
        scenarioController = GameObject.FindObjectOfType<ScenarioController>();
    }

    public void WriteToDebug()
    {
        string str = "CHOICE EVENT:";

        if (executables != null)
            foreach (var e in executables)
            {
                str += "/ name: " + e.LocalName + " ch: " + e.InnerText;

            }
        Debug.Log(str);
    }

    public void Execute()
    {
        ExecType et;

        if (executables != null)
            foreach (var e in executables)
            {
                et = (ExecType)Enum.Parse(typeof(ExecType), e.LocalName);
                switch (et)
                {
                    case ExecType.execScen:
                        Debug.Log("Executing a new scenario");
                        scenarioController.LoadScenario(int.Parse(e.InnerText));
                        break;
                    case ExecType.execEvent:
                        Debug.Log("Executing a new event");
                        break;
                }
            }
    }
}
