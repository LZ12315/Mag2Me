using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetEnemy : EnemyController
{
    private MagSource snapSource;
    private EquipHolder equipHolder;

    [Header("��������")]
    [SerializeField] private float snapDuration = 2f;

    private void Awake()
    {
        equipHolder = GetComponent<EquipHolder>();
        snapSource = GetComponent<MagSource>();
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
