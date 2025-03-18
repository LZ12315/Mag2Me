using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip: MonoBehaviour
{
    private Magnet magnet;

    [Header("×°±¸ÊôÐÔ")]
    [SerializeField] private int endurance;
    private EquipHolder equipHolder;

    public void EquipArmed(EquipHolder holder)
    {
        equipHolder = holder;
    }

    public void EquipRelieve(EquipHolder holder)
    {
        if (equipHolder = holder)
            equipHolder = null;

        if (magnet != null)
            magnet.MagnetRelease();
    }

}
