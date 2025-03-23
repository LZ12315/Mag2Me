using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneImage : MonoBehaviour
{
    private static ChangeSceneImage instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
