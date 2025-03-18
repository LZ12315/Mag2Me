using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour
{
    private PlayerController controller;

    [Header("∑¢…‰…Ë÷√")]
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float scatterAngle = 360f;

    Queue<Equip> equipments = new Queue<Equip>();

    private void Start()
    {
        controller = this?.GetComponent<PlayerController>();
    }

    public void EquipArmed(Transform equip)
    {
        Equip newEquip = equip?.GetComponent<Equip>();
        if (newEquip == null) return;

        equipments.Enqueue(newEquip);
        newEquip.EquipArmed(this);
    }

    public void GetDamage(Equip equip, int damage)
    {
        Debug.Log("EquipHolder : " + damage);
    }

    public void Shoot(Vector2 shootDir)
    {
        if (equipments.Count == 0) return;

        Equip weapon = equipments.Dequeue();
        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();

        weapon.EquipRelieve(this);
        rb.AddForce(shootDir * shootPower, ForceMode2D.Impulse);
    }

    public void Scatter(Vector2 scatterDir)
    {
        if (equipments.Count == 0) return;

        while (equipments.Count > 0)
        {
            Equip weapon = equipments.Dequeue();
            Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
            weapon.EquipRelieve(this);

            Vector2 shootDir = Vector2.zero;
            Vector2 equipDir = (weapon.transform.position - transform.position).normalized;

            if(Vector2.Angle(scatterDir,equipDir)*Mathf.Rad2Deg <= scatterAngle)
                shootDir = equipDir;
            else
            {
                float randomRadian = Random.Range(-scatterAngle / 2, scatterAngle/2) * Mathf.Deg2Rad;
                float cosAngle = Mathf.Cos(randomRadian);
                float sinAngle = Mathf.Sin(randomRadian);
                float x = cosAngle * scatterDir.x - sinAngle * scatterDir.y;
                float y = sinAngle * scatterDir.x + cosAngle * scatterDir.y;
                shootDir = new Vector2(x, y).normalized;
            }

            rb.AddForce(shootDir * shootPower, ForceMode2D.Impulse);
        }
    }

}
