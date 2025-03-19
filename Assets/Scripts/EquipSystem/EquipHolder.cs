using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour
{
    [Header("∑¢…‰…Ë÷√")]
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float scatterAngle = 360f;
    [SerializeField] private List<Equip> equipments = new List<Equip>();

    public void ArmEquip(Transform equip)
    {
        Equip newEquip = equip?.GetComponent<Equip>();
        if (newEquip == null) return;

        equipments.Add(newEquip);
        newEquip.EquipArmed(this);
    }

    Equip GetEquip()
    {
        if (equipments.Count == 0) return null;

        Equip tmpEquip = equipments[0];
        float tmpDistance = Vector2.Distance(tmpEquip.transform.position, transform.position);
        foreach (var equip in equipments)
        {
            float distance = Vector2.Distance(equip.transform.position, transform.position);
            if (distance <= tmpDistance) continue;
            tmpEquip = equip;
            tmpDistance = distance;
        }

        equipments.Remove(tmpEquip);
        return tmpEquip;
    }

    public void GetDamage(Equip equip, int damage)
    {
        Debug.Log("EquipHolder : " + damage);
    }

    public void Shoot(Vector2 shootDir)
    {
        if (equipments.Count == 0) return;

        Equip equip = GetEquip();
        Rigidbody2D rb = equip.GetComponent<Rigidbody2D>();

        equip.BulletShoot(this);
        rb.AddForce(shootDir * shootPower, ForceMode2D.Impulse);
    }

    public void Scatter(Vector2 scatterDir)
    {
        if (equipments.Count == 0) return;

        while (equipments.Count > 0)
        {
            Equip equip = GetEquip();
            Rigidbody2D rb = equip.GetComponent<Rigidbody2D>();
            equip.BulletShoot(this);

            Vector2 shootDir = Vector2.zero;
            Vector2 equipDir = (equip.transform.position - transform.position).normalized;

            if(Vector2.Angle(scatterDir,equipDir) <= scatterAngle)
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
