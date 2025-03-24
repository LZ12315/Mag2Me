using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour
{
    [SerializeField] private PhysicalCharacter physicalCharacter;
    [SerializeField] private Character character;

    [Header("防御设置")]
    [SerializeField] private bool defenceBreak;

    [Header("攻击设置")]
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float scatterAngle = 360f;
    [SerializeField] private List<Equip> equipments = new List<Equip>();

    private void Start()
    {
        physicalCharacter = GetComponent<PhysicalCharacter>();
        character = GetComponent<Character>();
    }

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

    public void HolderDefence(Equip attackEquip, int damage)
    {
        int attackTime = damage;
        if(!defenceBreak)
        {
            foreach (var equip in equipments)
            {
                if (equip.IsServiceable())
                {
                    equip.EquipDamage(this);
                    attackTime--;
                }
            }
        }

        if (attackTime <= 0)
            attackTime = 0;

        Vector2 forceDir = (Vector2)(transform.position - attackEquip.transform.position);
        if (physicalCharacter != null)
            physicalCharacter.AddForceImpluse(forceDir, 1f);
        if (character != null)
            character.GetDamage(attackTime);
    }

    public void Shoot(Vector2 lookDir)
    {
        if (equipments.Count == 0) return;

        Equip equip = GetEquip();
        Rigidbody2D rb = equip.GetComponent<Rigidbody2D>();

        equip.transform.position = transform.position;
        equip.ShootEquip(this, lookDir, shootPower);
    }

    public void Scatter(Vector2 scatterDir)
    {
        if (equipments.Count == 0) return;

        while (equipments.Count > 0)
        {
            Equip equip = GetEquip();
            Rigidbody2D rb = equip.GetComponent<Rigidbody2D>();

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

            equip.transform.position = transform.position;
            equip.ShootEquip(this, shootDir, shootPower);
        }
    }

    #region 其他

    public void PowerUp(BuffManager manager, float power)
    {
        shootPower += power;
    }

    #endregion
}
