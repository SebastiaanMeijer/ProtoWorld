using UnityEngine;
using System.Collections;

public interface IFeedable
{
    void Update();
    void ToFileFeed();
    void ToLiveFeed();
    void SwitchFeed();
}