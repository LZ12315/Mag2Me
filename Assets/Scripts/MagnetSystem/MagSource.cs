using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class MagSourceInfo
{
    [Header("吸附设置")]
    [SerializeField] 
    public float snapPower = 1f; //物体磁力强度
    [SerializeField] 
    public float snapAngle = 60f; //磁力效用角度
    [SerializeField] 
    public float snapDistance = 2f; //磁力效用距离
    [SerializeField] 
    public int maxHoldNum = 2; //最大持有磁体数量

    [Header("磁力梯度")]
    [SerializeField]
    public float maxAttractDuration = 6f; //最大吸引时间
    [SerializeField]
    [Range(0, 1)] public float farDistanceCoef = 0.85f; //远距离起始点（引力开始较弱）
    [SerializeField]
    [Range(0, 1)] public float midDistanceCoef = 0.6f; //中距离起始点（线性增速）
    [SerializeField]
    [Range(0, 1)] public float closeDistanceCoef = 0.35f; //近距离起始点（指数加速）
    [SerializeField]
    [Range(0, 1)] public float strongAccelRange = 0.1f; //临界接触区（最大力冲刺）

    public MagSourceInfo Clone()
    {
        return (MagSourceInfo)this.MemberwiseClone();
    }
}

[RequireComponent(typeof(Collider2D))]
public class MagSource : MonoBehaviour
{
    private Collider2D magCollider;
    private EquipHolder equipHolder;
    private IMagSourceControl controller;

    [Header("磁源设置")]
    [SerializeField] private MagSourceInfo sourceInfo = new MagSourceInfo();

    [Header("磁体列表")]
    [SerializeField] List<Magnet> MagnetBeingAttract = new List<Magnet>();
    [SerializeField] List<Magnet> MagnetInPlace = new List<Magnet>();

    Vector2 snapDir = Vector2.zero;
    bool snap = false;

    private void Start()
    {
        equipHolder = this?.GetComponent<EquipHolder>();
        controller = this?.GetComponent<IMagSourceControl>();
        magCollider = GetComponent<Collider2D>();

        Physics2D.defaultContactOffset = 0.01f;
    }

    private void Update()
    {
        if (CanAttract())
            AttractMagnet();
        DetectMagInPlace();
    }

    bool CanAttract()
    {
        int magNum = MagnetInPlace.Count;
        if (magNum < sourceInfo.maxHoldNum && snap)
            return true;
        else
            return false;
    }

    void AttractMagnet()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position,
        sourceInfo.snapDistance
        );

        List<Magnet> magnetsBeingDetect = new List<Magnet>();
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Magnet magnet = hit?.GetComponent<Magnet>();
            if (magnet == null || magnet.MagnetParent != null) continue;

            Vector2 objectDir = (magnet.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(snapDir, objectDir);
            if (angle >= sourceInfo.snapAngle / 2) continue;

            magnetsBeingDetect.Add(magnet);
        }

        List<Magnet> magnetsToRemove = new List<Magnet>();
        foreach (var maget in MagnetBeingAttract)
        {
            if (!magnetsBeingDetect.Contains(maget))
            {
                maget.StopAttract(this);
                magnetsToRemove.Add(maget);
            }
        }

        foreach (var magnet in magnetsToRemove)
        {
            if(MagnetBeingAttract.Contains(magnet))
                MagnetBeingAttract.Remove(magnet);
        }

        foreach (var magnet in magnetsBeingDetect)
        {
            if(!MagnetBeingAttract.Contains(magnet))
            {
                magnet.InvokeAttract(this, sourceInfo.Clone());
                MagnetBeingAttract.Add(magnet);
            }
        }
    }

    void DetectMagInPlace()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("MagnetLayer"));
        contactFilter.useTriggers = true;
        Collider2D[] collisions = new Collider2D[20];

        int overlapCount = magCollider.OverlapCollider(contactFilter, collisions);
        for (int i = 0; i < overlapCount; i++)
        {
            Magnet magnet = collisions[i]?.GetComponent<Magnet>();
            if (magnet == null || magnet.MagnetParent != null) continue;

            if (!MagnetInPlace.Contains(magnet))
                SnapMagInPlace(collisions[i]?.GetComponent<Magnet>());
        }
    }

    void SnapMagInPlace(Magnet magnet)
    {
        if (magnet == null) return;

        if (MagnetBeingAttract.Contains(magnet))
            MagnetBeingAttract.Remove(magnet);
        if (!MagnetInPlace.Contains(magnet))
            MagnetInPlace.Add(magnet);
        if (equipHolder != null)
            equipHolder.ArmEquip(magnet.transform);

        magnet.SnapFinalize(this);
        if (controller != null)
            controller.SnapObject(this);
    }

    public void ReleaseMagnet(Magnet magnet)
    {
        if(MagnetInPlace.Contains(magnet))
            MagnetInPlace.Remove(magnet);
    }

    #region 操作输入

    public void ExcuteSnap(IMagSourceControl controller)
    {
        snap = true;
    }

    public void SnapStop(IMagSourceControl controller)
    {
        snap = false;
        foreach (var magnet in MagnetBeingAttract)
            magnet.StopAttract(this);
        MagnetBeingAttract.Clear();
    }

    public void SetSnapDir(IMagSourceControl controller, Vector2 dir)
    {
        if(!Mathf.Approximately(dir.magnitude, 0))
            snapDir = dir;
    }

    #endregion

    #region 其他

    public float MagPower => sourceInfo.snapPower;

    public int NumInPlace => MagnetInPlace.Count;

    public MagSourceInfo GetSourceInfo(BuffManager manager)
    {
        return sourceInfo;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sourceInfo.snapDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sourceInfo.snapDistance * sourceInfo.farDistanceCoef);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sourceInfo.snapDistance * sourceInfo.midDistanceCoef);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sourceInfo.snapDistance * sourceInfo.closeDistanceCoef);
    }

    #endregion


}
