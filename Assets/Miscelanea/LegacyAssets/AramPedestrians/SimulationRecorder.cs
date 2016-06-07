using UnityEngine;
using System.Collections;
using System.IO;
using GapslabWCFservice;
using System;
using System.Text;

public class SimulationRecorder : MonoBehaviour
{

    public NavMeshAgent[] Pedestrians;
    public float RecordingDurationInSeconds = 15;
    public float TimeScale = 10;
    public string filename = "Pedestrians.xml";
    // Use this for initialization
    private float InitialTime;
    private int SecondCounter = 0;
    private bool IsRecording = false;
    StreamWriter writer;

    // Update is called once per frame
    void Update()
    {
        if (IsRecording)
        {
            if (SecondCounter < RecordingDurationInSeconds)
            {
                if (Time.time - InitialTime > 1)
                {
                    RecordStep();
                    SecondCounter++;
                    InitialTime = Time.time;
                }
            }
            else StopRecording();
        }
    }
    TimeStep tempTS;
    VehicleFCD[] tempV;
    public void RecordStep()
    {
        Debug.Log("Recording step #" + SecondCounter);
        tempTS = new TimeStep();
        tempTS.time = SecondCounter;
        tempV = new VehicleFCD[Pedestrians.Length];
        for (int i = 0; i < tempV.Length; i++)
        {
            tempV[i] = new VehicleFCD();
            tempV[i].Id = Pedestrians[i].transform.name;
            var pos = Pedestrians[i].transform.position;
            var velocity = Pedestrians[i].velocity;
            tempV[i].Latitude = pos.x;
            tempV[i].Longitude = pos.z;
            tempV[i].Speed = velocity.magnitude;
            tempV[i].Angle = 0;
            tempV[i].Lane = "";
            tempV[i].Pos = 0;
            tempV[i].Slope = 0;
            tempV[i].VehicleType = SUMOSimulationFCDVehicleType.Default;
        }
        tempTS.index = SecondCounter;
        tempTS.Vehicles = tempV;

        writer.WriteLine(GenerateXML(tempTS));
    }
    public void StopRecording()
    {
        Time.timeScale = 1;
        IsRecording = false;
        writer.WriteLine("</fcd-export>");
        writer.Close();
        Debug.Log("Recording is done....");
    }
    public void StartRecording()
    {
        Time.timeScale = TimeScale;
        if (File.Exists(filename))
            File.Delete(filename);
        writer = new StreamWriter(filename);
        writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        writer.WriteLine("\t<!-- generated on " + DateTime.Now.ToString() + " by KTH GaPSlabs multi-modal simulator Alpha Version 1.0 -->");
        writer.WriteLine("\t" + "<fcd-export>");

        InitialTime = Time.time;
        IsRecording = true;

    }
    private String GenerateXML(TimeStep timestep)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("\t<timestep time=\"" + timestep.time + "\">");
        foreach (var v in timestep.Vehicles)
        {
            // <vehicle id="39" x="18.059332" y="59.337872" angle="-156.214767" type="DEFAULT_VEHTYPE" speed="7.527420" pos="11.873972" lane="1767273#2_0" slope="0.000000"/>
            sb.AppendLine(
                string.Format(
                "\t\t<vehicle id=\"{0}\" x=\"{1}\" y=\"{2}\" angle=\"{3}\" type=\"{4}\" speed=\"{5}\" pos=\"{6}\" lane=\"{7}\" slope=\"{8}\" />",
                v.Id, v.Latitude, v.Longitude, v.Angle, "DEFAULT_VEHTYPE", v.Speed, v.Pos, v.Lane, v.Slope));
        }
        sb.AppendLine("\t</timestep>");
        return sb.ToString();
    }
    void OnDisable()
    {
        if (writer != null)
            writer.Dispose();
        Time.timeScale = 1;
    }
}
