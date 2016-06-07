/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedestriansProfile.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

public class FlashPedestriansProfile 
{
    public float speed;
    public bool englishSpeaker;
    public bool italianSpeaker;
    public float chanceOfSubscription;
    public float chanceOfBelievingRumours;
    public bool willingToChangeDestination;
    public float chanceOfTakingABike;
    public float weatherFactorOnTakingBikes;
    public bool carAwareness;
    public TravelPreference travelPreference;

    public FlashPedestriansProfile(float speed, bool englishSpeaker, bool italianSpeaker, float chanceOfSubscription, bool willingToChangeDestination, float chanceOfTakingABike, float chanceOfBelievingRumours, bool carAwareness, TravelPreference preference)
    {
        this.speed = speed;
        this.englishSpeaker = englishSpeaker;
        this.italianSpeaker = italianSpeaker;
        this.chanceOfSubscription = chanceOfSubscription;
        this.willingToChangeDestination = willingToChangeDestination;
        this.chanceOfTakingABike = chanceOfTakingABike;
        this.weatherFactorOnTakingBikes = 1.0f;
        this.chanceOfBelievingRumours = chanceOfBelievingRumours;
        this.carAwareness = carAwareness;
        this.travelPreference = preference;
    }
}
