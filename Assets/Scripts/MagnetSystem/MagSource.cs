using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MagSource : MonoBehaviour
{
    [SerializeField] private Collider2D magCollider;
    [SerializeField] private EquipHolder equipHolder;

    [Header("吸附设置")]
    [SerializeField] private float snapPower = 1f; //物体磁力强度
    [SerializeField] private float snapAngle = 60f; //磁力效用角度
    [SerializeField] public float snapDistance = 2f; //磁力效用距离
    [SerializeField] List<Magnet> ObjectBeingAttract = new List<Magnet>();
    [SerializeField] List<Magnet> ObjectinPlace = new List<Magnet>();

    [Header("磁力梯度")]
    [SerializeField]
    public float maxAttractDuration = 6f; //最大吸引时间
    [SerializeField]
    public float farDistanceCoef = 0.85f; //远距离起始点（引力开始较弱）
    [SerializeField]
    public float midDistanceCoef = 0.6f; //中距离起始点（线性增速）
    [SerializeField]
    public float closeDistanceCoef = 0.35f; //近距离起始点（指数加速）
    [SerializeField]
    public float strongAccelRange = 0.1f; //临界接触区（最大力冲刺）

    Vector2 lookDir = Vector2.zero;
    bool snap = false;


    private void Start()
    {
        equipHolder = this?.GetComponent<EquipHolder>();

        Physics2D.defaultContactOffset = 0.01f;
    }

    private void Update()
    {
        if (snap)
            ObjectAttract();
        DetectMagnet();
    }

    void ObjectAttract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position,
        snapDistance
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            Magnet magnet = hit?.GetComponent<Magnet>();
            if(magnet == null || ObjectBeingAttract.Contains(magnet)) continue;

            Vector2 objectDir = (magnet.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(lookDir, objectDir);
            if (angle >= snapAngle / 2) continue;

            magnet.InvokeAttract(this);
            ObjectBeingAttract.Add(magnet);
        }
    }

    void DetectMagnet()
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

            if (!ObjectinPlace.Contains(magnet))
                SnapMagnet(collisions[i]?.GetComponent<Magnet>());
        }
    }

    public void ReleaseMagnet(Magnet magnet)
    {
        if(ObjectinPlace.Contains(magnet))
            ObjectinPlace.Remove(magnet);
    }



    #region 物体连接

    void SnapMagnet(Magnet magnet)
    {
        if (magnet == null) return;

        if (ObjectBeingAttract.Contains(magnet))
            ObjectBeingAttract.Remove(magnet);
        if (!ObjectinPlace.Contains(magnet))
            ObjectinPlace.Add(magnet);
        if (equipHolder != null)
            equipHolder.ArmEquip(magnet.transform);

        magnet.SnapFinalize(this);
    }

    #endregion

    #region 其他

    public float MagPower => snapPower;

    public int NumInPlace => ObjectinPlace.Count;

    public Vector2 SnapDir {  get => lookDir;  set => lookDir = value; }

    public bool isSnap { get => snap; set => snap = value; }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, snapDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, snapDistance * farDistanceCoef);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, snapDistance * midDistanceCoef);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, snapDistance * closeDistanceCoef);
    }

    #endregion


}
