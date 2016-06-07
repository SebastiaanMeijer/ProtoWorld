/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashOnDestinationBehaviour.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// Actions to take while Flash Pedestrian is at state "Destination".
/// </summary>
public class FlashOnDestinationBehaviour : StateMachineBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private int logSeriesId;
    private bool loggerSetup = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!loggerSetup)
        {
            logSeriesId = LoggerAssembly.GetLogSeriesId();
            log.Info(string.Format("{0}:{1}:{2}", logSeriesId, "title", " Destination log"));
            log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "legend", 0, "Destionation chosen"));
            loggerSetup = true;
        }

        // Recycle the pedestrian object
        var pedestrian = animator.GetComponent<FlashPedestriansController>();
        var destination = pedestrian.routing.destinationPoint.destinationName;
        log.Info(string.Format("{0}:{1}:{2}:{3}", logSeriesId, "string", 0, destination));

        pedestrian.Recycle();
    }
}
