using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Equip: MonoBehaviour
{
    [SerializeField] protected Magnet magnet;
    [SerializeField] protected EquipHolder equipHolder;

    [Header("装备设置")]
    [SerializeField] protected int endurance = 3;

    [Header("发射设置")]
    [SerializeField] protected int bulletPower = 1;
    [SerializeField] protected bool serviceable;
    [SerializeField] protected bool isBullet;

    EquipHolder lastEquipHolder; // 防止发射时判定为原来的holder

    protected virtual void Start()
    {
        magnet = GetComponent<Magnet>();
        serviceable = true;
        isBullet = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == gameObject) return;

        if (isBullet)
        {
            EquipHolder target = collision?.GetComponent<EquipHolder>();
            if (target == null || target == lastEquipHolder || target == equipHolder) return;
            target.GetDamage(this, bulletPower);
        }
    }

    public void EquipArmed(EquipHolder holder)
    {
        equipHolder = holder;
        lastEquipHolder = null;
        serviceable = true;
    }

    protected void UseEquip()
    {
        endurance--;
        if (endurance <= 0)
            EquipBreak();
    }

    protected void EquipBreak()
    {
        Destroy(gameObject);
    }

    void EquipRelieve(EquipHolder holder)
    {
        if (magnet != null)
            magnet.MagnetRelease();
        if (equipHolder = holder)
        {
            lastEquipHolder = equipHolder;
            equipHolder = null;
        }
    }

    public void BulletShoot(EquipHolder holder)
    {
        EquipRelieve(holder);
        isBullet = true;
        serviceable = false;
    }

    #region 其他

    public EquipHolder EquipHolder => equipHolder;

    public bool Serviceable => serviceable;

    #endregion

}
