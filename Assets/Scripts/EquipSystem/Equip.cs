using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Equip: MonoBehaviour
{
    protected Magnet magnet;
    protected EquipHolder equipHolder;

    [Header("装备属性")]
    [SerializeField] protected int endurance = 3;
    [SerializeField] protected int bulletPower = 1;
    [SerializeField] protected bool isBullet;

    protected virtual void Start()
    {
        magnet = GetComponent<Magnet>();
        equipHolder = GetComponent<EquipHolder>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBullet)
        {
            if (collision.collider?.GetComponent<EquipHolder>() == null) return;
            collision.collider?.GetComponent<EquipHolder>().GetDamage(this, bulletPower);
        }
    }

    public void EquipArmed(EquipHolder holder)
    {
        equipHolder = holder;
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

    public void EquipRelieve(EquipHolder holder)
    {
        if (equipHolder = holder)
            equipHolder = null;

        if (magnet != null)
            magnet.MagnetRelease();

        isBullet = true;
    }

    #region 其他

    public EquipHolder EquipHolder => equipHolder;

    #endregion

}
