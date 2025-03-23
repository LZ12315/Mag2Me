using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoadScene : MonoBehaviour
{
    bool isLoading;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!isLoading )
            {
                isLoading = true;
                EventCenter.Instance.EventTrigger("ÇÐ»»³¡¾°BattleFieldTest");
            }
        }
    }
}
