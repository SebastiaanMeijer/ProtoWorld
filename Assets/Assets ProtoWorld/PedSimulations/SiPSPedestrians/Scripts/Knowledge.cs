/*
 * 
 * PEDESTRIANS KTH
 * Knowledge.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Enumerate that defines the possible status of a mode.
/// </summary>
public enum Status
{
    ON_SCHEDULE,
    DELAYED,
    OUT_OF_SCHEDULE
};

/// <summary>
/// Generic class that defines a piece of knowledge.
/// A piece of knowledge is defined by its id and the time it was created.
/// </summary>
public class Knowledge
{
    protected string id { get; set; }
    protected float timeCreated { get; set; }
    public int index { get; set; }

    public Knowledge(string id)
    {
        this.id = id;
        this.timeCreated = Time.time;
    }

    public string GetId()
    {
        return id;
    }

    public float GetTimeCreated()
    {
        return timeCreated;
    }
}

/// <summary>
/// Class that defines a mode information. Extends Knowledge.
/// A mode information is defined by its time of arrival and its status.
/// </summary>
public class ModeInfo : Knowledge
{
    private float timeOfArrival { get; set; }
    protected Status status { get; set; }

    public ModeInfo(String id, float timeOfArrival)
        : base(id)
    {
        this.timeOfArrival = timeOfArrival;
        this.status = Status.ON_SCHEDULE;
    }

    public void SetStatus(Status s)
    {
        this.status = s;
        this.timeCreated = Time.time;
    }

    public Status GetStatus()
    {
        return status;
    }

    public void DelayMode(float delay)
    {
        timeOfArrival += delay;
        status = Status.DELAYED;
        this.timeCreated = Time.time;
    }

    public float GetTimeOfArrival()
    {
        return timeOfArrival;
    }

    public bool Equals(ModeInfo obj)
    {
        return (this.id.Equals(obj.id))
            && (this.timeOfArrival.Equals(obj.timeOfArrival))
            && (this.timeCreated.Equals(obj.timeCreated))
            && (this.status.Equals(obj.status));
    }

    public override string ToString()
    {
        return 
            this.id + ", " + "arrival at: " + this.timeOfArrival + ", status: " + this.status.ToString() + "\n"; 
    }
}

/// <summary>
/// Class that defines a lecture information. Extends Knowledge.
/// A lecture information is defined by the time it starts, the time it ends and the interest. 
/// </summary>
public class LectureInfo : Knowledge
{
    private float timeStarts { get; set; }
    private float timeEnds { get; set; }
    private float interest { get; set; }

    public LectureInfo(String id, float timeStarts, float timeEnds, float interest)
        : base(id)
    {
        this.timeStarts = timeStarts;
        this.timeEnds = timeEnds;
        this.interest = interest;
    }

    public float GetTimeStarts()
    {
        return timeStarts;
    }

    public float GetTimeEnds()
    {
        return timeEnds;
    }

    internal float GetInterest()
    {
        return interest;
    }
}

/// <summary>
/// Class that defines a rumour information. Extends Knowledge.
/// A rumour is defined by a piece of Knowledge and by its credibility.
/// </summary>
public class RumourInfo : Knowledge
{
    private Knowledge rumour;
    private float credibility;

    public RumourInfo(String id, Knowledge rumour, float credibility)
        : base(id)
    {
        this.rumour = rumour;
        this.credibility = credibility;
    }

    internal float GetCredibility()
    {
        return credibility;    
    }

    internal Knowledge GetRumour()
    {
        return rumour;
    }
}

