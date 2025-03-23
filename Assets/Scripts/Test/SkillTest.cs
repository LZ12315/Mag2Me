using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkillTest : MonoBehaviour
{
    [SerializeField] PlayableDirector theTimeLine;


    public bool canStart;


    bool isOnce;

    private void Start()
    {
        EventCenter.Instance.AddEventListener("Combo", () => PlaySkill());
    }




    private void PlaySkill()
    {
        theTimeLine.Play();
    }




}
