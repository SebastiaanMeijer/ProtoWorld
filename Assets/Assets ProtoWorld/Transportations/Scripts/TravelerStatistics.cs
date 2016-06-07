using UnityEngine;
using System.Collections;

public class TravelerStatistics : MonoBehaviour
{

    public int travelerCount;

    public int inActiveTravelerCount;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        travelerCount = transform.childCount;
        inActiveTravelerCount = 0;
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                ++inActiveTravelerCount;
            }
        }
    }
}
