using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour
{
    private PlayerController controller;

    [Header("∑¢…‰…Ë÷√")]
    [SerializeField] private float shootPower = 10f;

    Queue<Equip> equipments = new Queue<Equip>();

    private void Start()
    {
        controller = this?.GetComponent<PlayerController>();
    }

    public void EquipArmed(Transform equip)
    {
        Equip newEquip = equip?.GetComponent<Equip>();
        if (newEquip != null)
            equipments.Enqueue(newEquip);
    }

    public void Shoot(Vector2 shootDir)
    {
        if (equipments.Count > 0)
        {
            Equip weapon = equipments.Dequeue();
            Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
            Magnet magnet = weapon?.GetComponent<Magnet>();

            if (magnet != null)
                magnet.MagnetRelease();
            rb.AddForce(shootDir * shootPower, ForceMode2D.Impulse);
        }
    }

}
