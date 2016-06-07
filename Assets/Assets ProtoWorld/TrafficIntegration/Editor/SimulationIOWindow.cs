using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Show status of reading simulation in all sub-classes of SimulationIOBase.
/// </summary>
public class SimulationIOWindow : EditorWindow
{
    SimulationIOBase simio;

    void OnGUI()
    {
        if (simio == null)
            return;
        GUILayout.Label(simio.status);
        GUILayout.Label(simio.GetStatus());
        if (simio.IsStopped())
        {
            if (GUILayout.Button("Close"))
            {
                Close();
                DestroyImmediate(this);
            }
        }
        else
        {
            if (GUILayout.Button("Cancel"))
            {
                simio.RequestStop();
                Close();
                DestroyImmediate(this);
            }
        }
    }

    public void SetSimulationIO(SimulationIOBase simio)
    {
        this.simio = simio;
    }
}
