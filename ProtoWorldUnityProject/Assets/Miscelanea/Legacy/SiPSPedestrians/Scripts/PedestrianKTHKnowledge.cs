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
 * PedestrianKTHKnowledge.cs
 * Miguel Ramos Carretero
 * Edited by Furkan Sonmez
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implements the knowledge of a pedestrian regarding mode information, lectures and rumours. 
/// </summary>
public class PedestrianKTHKnowledge : MonoBehaviour
{
    [HideInInspector]
    public bool newKnowledgeToBeCheckedbyController;

    [HideInInspector]
    public ModeDB mdb;

    [HideInInspector]
    public int mode = 0;

    [HideInInspector]
    public Tuple<RumourInfo, int> rumour = null;

    [HideInInspector]
    public bool tellMeWhatYouAreDoing = false;

    [Range(0.0f, 1.0f)]
    public float studyInterest;

    [Range(0.0f, 1.0f)]
    public float delayTolerance;

    [Range(0.0f, 1.0f)]
    public float rumourSusceptibility;

    [Range(0, 100)]
    public int maxTimesRumourIsSpread = 20;

    public bool suscribedToModeDB = true;
    public float timeToGoToMode = 0;

    private List<LectureInfo> currentLectureInfoKnowledge = new List<LectureInfo>();
    private List<ModeInfo> currentModeInfoKnowledge = new List<ModeInfo>();
    private int currentIndex = 0;
    private int timesRumourIsSpread = 0;
	//Added by Furkan, This is the animationStateName which the pedestrian currently is in
	public string animationStateName;

    /// <summary>
    /// Start the script. 
    /// </summary>
    void Start()
    {
        //Set an empty rumour
        rumour = new Tuple<RumourInfo, int>(new RumourInfo("noRumour", null, 0.0f), 0);

        //Add information about the current lecture the student is attending
        currentLectureInfoKnowledge.Add(new LectureInfo("classSession01", 0, 30, 0.5f));

        //Find ModeDB
        mdb = FindObjectOfType<ModeDB>();
        
        GetNextModeFromDB();
    }

    /// <summary>
    /// Checks the next available mode in the pedestrian knowledge. If there is no available mode in the pedestrian knowledge, checks the modeDB. 
    /// </summary>
    /// <returns>ModeInfo with the next available mode.</returns>
    public ModeInfo NextAvailableMode()
    {
        //Check the knowledge of the pedestrian
        foreach (var m in currentModeInfoKnowledge)
        {
            if ((m.GetTimeOfArrival() > Time.time) && (!m.GetStatus().Equals(Status.OUT_OF_SCHEDULE)))
                return m;
        }

        //If not found, get next mode from ModeDB. 
        GetNextModeFromDB();

        //Check the knowledge of the pedestrian again
        foreach (var m in currentModeInfoKnowledge)
        {
            if ((m.GetTimeOfArrival() > Time.time) && (!m.GetStatus().Equals(Status.OUT_OF_SCHEDULE)))
                return m;
        }

        return null;
    }

    /// <summary>
    /// Compares the current knowledge of the pedestrian with ModeDB and updates the information. Takes also the next available mode from the ModeDB.  
    /// </summary>
    public void UpdateKnowledgeFromModeDB()
    {
        if (mode == 1)
        {
            foreach (var m in currentModeInfoKnowledge)
            {
                ModeInfo modefromDB = mdb.GetBusInfoWithId(m.GetId());
                if (modefromDB != null && !modefromDB.Equals(mdb))
                {
                    //Copy mode (not referencing!)
                    currentModeInfoKnowledge[m.index] = new ModeInfo(modefromDB.GetId(), modefromDB.GetTimeOfArrival());
                    currentModeInfoKnowledge[m.index].SetStatus(modefromDB.GetStatus());
                    currentModeInfoKnowledge[m.index].index = m.index;
                }
            }
        }
        else
        {
            foreach (var m in currentModeInfoKnowledge)
            {
                ModeInfo modefromDB = mdb.GetMetroInfoWithId(m.GetId());
                if (modefromDB != null && !modefromDB.Equals(mdb))
                {
                    //Copy mode (not referencing!)
                    currentModeInfoKnowledge[m.index] = new ModeInfo(modefromDB.GetId(), modefromDB.GetTimeOfArrival());
                    currentModeInfoKnowledge[m.index].SetStatus(modefromDB.GetStatus());
                    currentModeInfoKnowledge[m.index].index = m.index;
                }
            }
        }

        GetNextModeFromDB();
    }

    /// <summary>
    /// Gets the next mode from ModeDB and copy it to the pedestrian knowledge.
    /// </summary>
    private void GetNextModeFromDB()
    {
        ModeInfo m;

        if (mode == 1)
        {
            m = mdb.GetBusInfoWithIndex(currentIndex);
        }
        else
        {
            m = mdb.GetMetroInfoWithIndex(currentIndex);
        }

        if (m != null)
        {
            //Copy mode (not referencing!)
            ModeInfo mAux = new ModeInfo(m.GetId(), m.GetTimeOfArrival());
            mAux.SetStatus(m.GetStatus());
            mAux.index = currentIndex;
            currentModeInfoKnowledge.Add(mAux);
            currentIndex++;
        }
    }

    /// <summary>
    /// Mark as missed a certain mode in the pedestrian knowledge.
    /// </summary>
    /// <param name="index">Position of the mode in the knowledge list.</param>
    public void MarkModeAsMissed(int index)
    {
        try
        {
            currentModeInfoKnowledge[index].SetStatus(Status.OUT_OF_SCHEDULE);
        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// Auxiliar method used from ModeDB through BroadcastMessage. Handles the reception of a new rumour in the pedestrian knowledge.
    /// </summary>
    /// <param name="information">Tuple containing the RumourInfo object and an integer indicating the mode.</param>
    public void NewRumourReceived(Tuple<RumourInfo, int> information)
    {
        if (information.Item2 == mode)
        {
            RumourInfo newRumour = information.Item1;

            if (!newRumour.GetId().Equals(rumour.Item1.GetId()) 
                && rumourSusceptibility > Random.Range(0.0f, 1.0f) 
                && newRumour.GetCredibility() > Random.Range(0.0f, 1.0f))
            {
                ModeInfo mInfo = (ModeInfo)newRumour.GetRumour();

                foreach (var m in currentModeInfoKnowledge)
                {
                    if (mInfo.GetId().Equals(m.GetId()) && !mInfo.Equals(m) && mInfo.GetTimeCreated() > m.GetTimeCreated())
                    {
                        //Copy mode (not referencing!)
                        currentModeInfoKnowledge[m.index] = new ModeInfo(mInfo.GetId(), mInfo.GetTimeOfArrival());
                        currentModeInfoKnowledge[m.index].SetStatus(mInfo.GetStatus()); 
                        currentModeInfoKnowledge[m.index].index = m.index;
                    }
                }

                // Spread message around
                float rumourCredibility = newRumour.GetCredibility() - 0.1f;

                if (rumourCredibility > 0.0f)
                {
                    RumourInfo rInfo = new RumourInfo(newRumour.GetId(), newRumour.GetRumour(), rumourCredibility);
                    rumour = new Tuple<RumourInfo, int>(rInfo, information.Item2);
                    InvokeRepeating("SpreadRumour", 1, 1.0f + Random.Range(0.0f, 1.0f));
                }

                newKnowledgeToBeCheckedbyController = true;
            }
        }
    }

    /// <summary>
    /// Auxiliar method used from ModeDB through BroadcastMessage. Handles the reception of new ModeDB information in the pedestrian knowledge.
    /// </summary>
    /// <param name="information">Tuple containing the ModeInfo object and an integer indicating the mode.</param>
    public void NewMessagefromModeDBReceived(Tuple<ModeInfo, int> information)
    {
        if (tellMeWhatYouAreDoing)
            Debug.Log("Got a message!");

        if (suscribedToModeDB && information.Item2 == mode)
        {
            ModeInfo mInfo = information.Item1;

            foreach (var m in currentModeInfoKnowledge)
            {
                if (mInfo.GetId().Equals(m.GetId()) && !mInfo.Equals(m) && mInfo.GetTimeCreated() > m.GetTimeCreated())
                {
                    if (tellMeWhatYouAreDoing)
                        Debug.Log("Message received from mode db: " + mInfo.ToString());

                    //Copy mode (not referencing!)
                    currentModeInfoKnowledge[m.index] = new ModeInfo(mInfo.GetId(), mInfo.GetTimeOfArrival());
                    currentModeInfoKnowledge[m.index].SetStatus(mInfo.GetStatus());
                    currentModeInfoKnowledge[m.index].index = m.index;
                }
            }

            RumourInfo rInfo = new RumourInfo("rumour" + mInfo.GetId(), mInfo, 1.0f);
            rumour = new Tuple<RumourInfo, int>(rInfo, information.Item2);

            InvokeRepeating("SpreadRumour", 1, 1.0f + Random.Range(0.0f, 1.0f));

            newKnowledgeToBeCheckedbyController = true;
        }
    }

    /// <summary>
    /// Takes the information of the current lecture. 
    /// </summary>
    /// <returns>LectureInfo with the information of the current lecture, null if there is no lecture found. </returns>
    internal LectureInfo CurrentLecture()
    {
        foreach (var l in currentLectureInfoKnowledge)
        {
            if (l.GetTimeStarts() <= Time.time && l.GetTimeEnds() >= Time.time)
                return l;
        }
        return null;
    }

    /// <summary>
    /// Spreads around the current rumour that the pedestrian has in its knowledge. 
    /// </summary>
    internal void SpreadRumour()
    {
        if (tellMeWhatYouAreDoing)
            Debug.Log("Spreading the rumour!");

        Collider[] pedestriansAffected = Physics.OverlapSphere(this.transform.position, 5.0f, LayerMask.GetMask("Pedestrian"));

        foreach (var p in pedestriansAffected)
        {
            if (p.transform == this.transform)
                continue;

            p.SendMessage("NewRumourReceived", rumour, SendMessageOptions.DontRequireReceiver);
        }

        if (timesRumourIsSpread++ >= maxTimesRumourIsSpread)
        {
            timesRumourIsSpread = 0;
            CancelInvoke();
        }
    }

    /// <summary>
    /// Prints the current pedestrian knowledge. 
    /// </summary>
    public void PrintKnowledge()
    {
        string output = "";

        foreach (ModeInfo m in currentModeInfoKnowledge)
        {
            output = output + m.ToString();
        }

        UnityEngine.Debug.Log("--------------------\n" + "Current knowlegde of " + this.name + " going to mode " + mode + " :\n" + output + "--------------------\n");
    }

    public void Reset()
    {
        currentModeInfoKnowledge.Clear();
        currentIndex = 0;
    }
}
