using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBuffVocal : MonoBehaviour
{

    void Start()
    {
        EventCenter.Instance.EventTrigger("组件增加");
    }


}
