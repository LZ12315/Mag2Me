using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
using System;

public class PlayerTest : MonoBehaviour
{
    [Header("KeySetting")]
    public KeyCode ActionKey = KeyCode.Space;
    [Header("Feedbacks")]
    /// a MMF_Player to play when the Hero starts jumping
    public MMFeedbacks PullFeedback;
    /// a MMF_Player to play when the Hero lands after a jump
    public MMFeedbacks PushFeedback;

    private bool isPulling = false;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(ActionKey))
        {
            IsPulling();
        }
        if (isPulling && Input.GetKeyUp(ActionKey))
        {
            IsPushing();
        }
    }

    private void IsPushing()
    {
        isPulling = false;
        PushFeedback.PlayFeedbacks();
    }

    private void IsPulling()
    {
        isPulling = true;
        PullFeedback.PlayFeedbacks();
    }
}
