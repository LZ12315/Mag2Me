using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetEnemy : EnemyController
{
    private SnapSource snapSource;
    private EquipHolder equipHolder;

    [Header("µ–»À…Ë÷√")]
    [SerializeField] private float snapDuration = 2f;

    private void Awake()
    {
        equipHolder = GetComponent<EquipHolder>();
        snapSource = GetComponent<SnapSource>();
    }

    private void Start()
    {
        StartCoroutine(ArmEquip());
    }

    IEnumerator ArmEquip()
    {
        snapSource.isSnap = true;
        yield return new WaitForSeconds(snapDuration);
        snapSource.isSnap = false;
    }

}
