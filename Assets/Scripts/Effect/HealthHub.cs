using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthHub : MonoBehaviour
{
    [SerializeField] private List<GameObject> hearts = new List<GameObject>();
    private int heartIndex = 0;

    private void Start()
    {
        EventCenter.Instance.AddEventListener<int>(EventName.HealthUp.ToString(), value => AddHeart(value));
    }

    void AddHeart(int num)
    {
        foreach (var heart in hearts)
        {
            if (heart.activeSelf)
                heart.SetActive(false);
        }

        for (int i = 0; i < num; i++)
        {
            if (!hearts[i].activeSelf)
                hearts[i].SetActive(true);
        }
    }

}
