/*
 * 
 * SESTAR INTEGRATION
 * SyntheticEntitiesCounterKPI.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Auxiliar script for the KPI manager that counts the number of a certain type of pedestrian according to the SEStar simulation.
/// </summary>
public class SyntheticEntitiesCounterKPI : MonoBehaviour 
{
    public string typeOfPedestrianToCount = null;
    public bool countPedestriansCreated;
    public bool countPedestriansDeleted;

    private SEStar SEStarControl;
    private KPIManager kpiManagerForCounting;
    private int kpiKeyForCounting;
    private int counter = 0;

    /// <summary>
    /// Initializes the script.
    /// </summary>
    void Start()
    {
        SEStarControl = GameObject.FindObjectOfType<SEStar>();
        kpiManagerForCounting = this.transform.GetComponent<KPIManager>();
        kpiKeyForCounting = kpiManagerForCounting.RegisterNewInformer();
    }

    /// <summary>
    /// Update method.
    /// </summary>
    void Update()
    {
        int updatedCounter;

        if (countPedestriansCreated && countPedestriansDeleted)
            updatedCounter = SEStarControl.GetNumberOfSyntheticEntitiesInSimulation(typeOfPedestrianToCount);
        else if (countPedestriansCreated)
            updatedCounter = SEStarControl.GetNumberOfSyntheticEntitiesCreated(typeOfPedestrianToCount);
        else if (countPedestriansDeleted)
            updatedCounter = SEStarControl.GetNumberOfSyntheticEntitiesDeleted(typeOfPedestrianToCount);
        else
            updatedCounter = 0;

        if (updatedCounter != counter)
        {
            counter = updatedCounter;
            kpiManagerForCounting.InformKPI(kpiKeyForCounting, counter);
        }
    }
}
