using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

public class MatsimIO : SimulationIOBase
{
    public override bool CanRead(string fileName)
    {
        Debug.Log("Not implemented yet");
        return false;
    }

    public override bool CanWrite(string fileName)
    {
        Debug.Log("Not implemented yet");
        return false;
    }

    public override void Read(string fileName)
    {
        Debug.Log("Not implemented yet");
        // TODO
    }

    public override void Write(object sender, DoWorkEventArgs e)
    {
        //throw new NotImplementedException();
    }
}
