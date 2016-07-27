/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * PEDESTRIANS KTH
 * ModeDB.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Implements the ModeDB of the simulation.
/// The ModeDB is composed of a list of metros and a list of buses, both containing <see cref="ModeInfo"/> elements. 
/// </summary>
public class ModeDB : MonoBehaviour
{
    [HideInInspector]
    public bool notificatePedestriansSuscribed = true;

    public GameObject pedestrians;

    private List<ModeInfo> listOfMetros = new List<ModeInfo>();
    private List<ModeInfo> listOfBuses = new List<ModeInfo>();
    private PedestriansKTHConfig conf;

    /// <summary>
    /// Awakes and initializates the script.
    /// </summary>
    void Awake()
    {
        conf = this.GetComponentInParent<PedestriansKTHConfig>();
        notificatePedestriansSuscribed = conf.sendInfoToPedestriansSuscribed;

        //Generate list of metros
        listOfMetros.Add(new ModeInfo("metro01", 300));
        listOfMetros.Add(new ModeInfo("metro02", 600));
        listOfMetros.Add(new ModeInfo("metro03", 900));
        listOfMetros.Add(new ModeInfo("metro04", 1200));
        listOfMetros.Add(new ModeInfo("metro05", 1500));
        listOfMetros.Add(new ModeInfo("metro06", 1800));
        listOfMetros.Add(new ModeInfo("metro07", 2100));
        listOfMetros.Add(new ModeInfo("metro08", 2400));
        listOfMetros.Add(new ModeInfo("metro09", 2700));

        //Generate list of buses
        listOfBuses.Add(new ModeInfo("bus01", 500));
        listOfBuses.Add(new ModeInfo("bus02", 1000));
        listOfBuses.Add(new ModeInfo("bus03", 1500));
        listOfBuses.Add(new ModeInfo("bus04", 2000));
        listOfBuses.Add(new ModeInfo("bus05", 2500));
        listOfBuses.Add(new ModeInfo("bus06", 3000));
        listOfBuses.Add(new ModeInfo("bus07", 3500));
        listOfBuses.Add(new ModeInfo("bus08", 4000));
        listOfBuses.Add(new ModeInfo("bus09", 4500));

        InvokeRepeating("UpdateSchedule", 1, 1.0f);
    }

    /// <summary>
    /// Update the schedule and the status of the modes.
    /// </summary>
    void UpdateSchedule()
    {
        foreach (var m in listOfMetros)
        {
            if (m.GetTimeOfArrival() < Time.time)
            {
                m.SetStatus(Status.OUT_OF_SCHEDULE);
            }
        }

        foreach (var m in listOfBuses)
        {
            if (m.GetTimeOfArrival() < Time.time)
            {
                m.SetStatus(Status.OUT_OF_SCHEDULE);
            }
        }
    }

    /// <summary>
    /// Adds a new metro to the schedule. 
    /// </summary>
    /// <param name="metro">ModeInfo containing the new metro information.</param>
    public void AddNewMetro(ModeInfo metro)
    {
        listOfMetros.Add(metro);
    }

    /// <summary>
    /// Adds a new bus to the schedule. 
    /// </summary>
    /// <param name="metro">ModeInfo containing the new bus information.</param>
    public void AddNewBus(ModeInfo bus)
    {
        listOfMetros.Add(bus);
    }

    /// <summary>
    /// Introduces a delay in all the metros of the schedule.
    /// </summary>
    /// <param name="delay">Float containing the amount of delay in seconds.</param>
    public void DelayMetros(float delay)
    {
        foreach (ModeInfo m in listOfMetros)
        {
            if (!m.GetStatus().Equals(Status.OUT_OF_SCHEDULE))
            {
                m.DelayMode(delay);
                m.SetStatus(Status.DELAYED);
            }
        }

        if (notificatePedestriansSuscribed)
        {
            //Notifies about the delay to those pedestrians that are suscribed to the ModeDB. 
            ModeInfo mInfo = GetNextMetroInfo();

            if (mInfo != null)
            {
                UnityEngine.Debug.Log("Broadcasting this message: " + mInfo.ToString());
                pedestrians.BroadcastMessage("NewMessagefromModeDBReceived", new Tuple<ModeInfo, int>(mInfo, 2));
            }
        }
    }

    /// <summary>
    /// Introduces a delay in all the buses of the schedule.
    /// </summary>
    /// <param name="delay">Float containing the amount of delay in seconds.</param>
    public void DelayBuses(float delay)
    {
        foreach (ModeInfo m in listOfBuses)
        {
            if (!m.GetStatus().Equals(Status.OUT_OF_SCHEDULE))
            {
                m.DelayMode(delay);
                m.SetStatus(Status.DELAYED);
            }
        }

        if (notificatePedestriansSuscribed)
        {
            //Notifies about the delay to those pedestrians that are suscribed to the ModeDB. 
            ModeInfo mInfo = GetNextBusInfo();

            if (mInfo != null)
            {
                UnityEngine.Debug.Log("Broadcasting this message: " + mInfo.ToString());
                pedestrians.BroadcastMessage("NewMessagefromModeDBReceived", new Tuple<ModeInfo, int>(mInfo, 1));
            }
        }
    }

    /// <summary>
    /// Gets the next upcoming metro in the schedule. 
    /// </summary>
    /// <returns>ModeInfo with the information of the upcoming metro.</returns>
    public ModeInfo GetNextMetroInfo()
    {
        foreach (ModeInfo m in listOfMetros)
        {
            if (m.GetTimeOfArrival() > Time.time && !m.GetStatus().Equals(Status.OUT_OF_SCHEDULE))
                return m;
        }

        return null;
    }

    /// <summary>
    /// Gets the next upcoming bus in the schedule. 
    /// </summary>
    /// <returns>ModeInfo with the information of the upcoming bus.</returns>
    public ModeInfo GetNextBusInfo()
    {
        foreach (ModeInfo m in listOfBuses)
        {
            if (m.GetTimeOfArrival() > Time.time && !m.GetStatus().Equals(Status.OUT_OF_SCHEDULE))
                return m;
        }

        return null;
    }

    /// <summary>
    /// Gets the metro information situated in a certain position of the list. 
    /// </summary>
    /// <param name="index">Index containing the position of the metro in the list.</param>
    /// <returns>ModeInfo with the information of the metro requested, or null if not found.</returns>
    public ModeInfo GetMetroInfoWithIndex(int index)
    {
        try
        {
            return listOfMetros[index];
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the metro information that has a certain id. 
    /// </summary>
    /// <param name="id">Id of the metro requested.</param>
    /// <returns>ModeInfo with the information of the metro requested, or null if not found.</returns>
    public ModeInfo GetMetroInfoWithId(string id)
    {
        foreach (var m in listOfMetros)
        {
            if (m.GetId().Equals(id))
                return m;
        }

        return null;
    }

    /// <summary>
    /// Gets the bus information situated in a certain position of the list. 
    /// </summary>
    /// <param name="index">Index containing the position of the bus in the list.</param>
    /// <returns>ModeInfo with the information of the bus requested, or null if not found.</returns>
    public ModeInfo GetBusInfoWithIndex(int index)
    {
        try
        {
            return listOfBuses[index];
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the bus information that has a certain id. 
    /// </summary>
    /// <param name="id">Id of the bus requested.</param>
    /// <returns>ModeInfo with the information of the bus requested, or null if not found.</returns>
    public ModeInfo GetBusInfoWithId(string id)
    {
        foreach (var m in listOfBuses)
        {
            if (m.GetId().Equals(id))
                return m;
        }

        return null;
    }

    /// <summary>
    /// Cancels the next upcoming bus in the schedule. 
    /// </summary>
    public void CancelNextBus()
    {
        ModeInfo mInfo = null;

        foreach (ModeInfo m in listOfBuses)
        {
            if (m.GetTimeOfArrival() > Time.time && !m.GetStatus().Equals(Status.OUT_OF_SCHEDULE))
            {
                m.SetStatus(Status.OUT_OF_SCHEDULE);
                mInfo = m;
                break;
            }
        }

        if (notificatePedestriansSuscribed)
        {
            //Notifies about the cancellation to those pedestrians that are suscribed to the ModeDB. 
            UnityEngine.Debug.Log("Broadcasting this message: " + mInfo.ToString());
            pedestrians.BroadcastMessage("NewMessagefromModeDBReceived", new Tuple<ModeInfo, int>(mInfo, 1));
        }
    }

    /// <summary>
    /// Cancels the next upcoming metro in the schedule. 
    /// </summary>
    public void CancelNextMetro()
    {
        ModeInfo mInfo = null;

        foreach (ModeInfo m in listOfMetros)
        {
            if (m.GetTimeOfArrival() > Time.time && !m.GetStatus().Equals(Status.OUT_OF_SCHEDULE))
            {
                m.SetStatus(Status.OUT_OF_SCHEDULE);
                mInfo = m;
                break;
            }
        }

        if (notificatePedestriansSuscribed)
        {
            //Notifies about the cancellation to those pedestrians that are suscribed to the ModeDB. 
            UnityEngine.Debug.Log("Broadcasting this message: " + mInfo.ToString());
            pedestrians.BroadcastMessage("NewMessagefromModeDBReceived", new Tuple<ModeInfo, int>(mInfo, 2));
        }
    }

    /// <summary>
    /// Prints the metro schedule. 
    /// </summary>
    public void PrintMetroSchedule()
    {
        string output = "";

        foreach (ModeInfo m in listOfMetros)
        {
            output = output + m.ToString();
        }

        UnityEngine.Debug.Log("--------------------\n" + "Metro schedule:\n" + output + "--------------------\n");
    }

    /// <summary>
    /// Prints the bus schedule.
    /// </summary>
    public void PrintBusSchedule()
    {
        string output = "";

        foreach (ModeInfo m in listOfBuses)
        {
            output = output + m.ToString();
        }

        UnityEngine.Debug.Log("--------------------\n" + "Bus schedule:\n" + output + "--------------------\n");
    }
}