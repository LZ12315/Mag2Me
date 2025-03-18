using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SnapSource : MonoBehaviour
{
    [SerializeField] private EquipHolder equipHolder;

    [Header("吸附设置")]
    [SerializeField] private float snapPower = 1f; //物体磁力强度
    [SerializeField] private float snapAngle = 60f; //磁力效用角度
    [SerializeField] public float snapDistance = 2f; //磁力效用距离
    [SerializeField] public float magnetHoldRadius = 2f; //最大持有距离
    [SerializeField] List<Magnet> ObjectinPlace = new List<Magnet>();

    [Header("磁力梯度")]
    [SerializeField]
    public float farDistanceCoef = 0.55f;       // 远距离起始点（引力开始较弱）
    [SerializeField]
    public float midDistanceCoef = 0.25f;     // 中距离起始点（线性增速）
    [SerializeField]
    public float closeDistanceCoef = 0.15f;     // 近距离起始点（指数加速）
    [SerializeField]
    public float strongAccelRange = 0.05f;    // 临界接触区（最大力冲刺）
    [SerializeField]
    public float minAcceleration = 0.5f;     // 极远距离基准加速度（引力微弱）
    [SerializeField]
    public float midAcceleration = 8f;       // 中距离加速度（线性提升）
    [SerializeField]
    public float maxAcceleration = 80f;      // 极近距离最大加速度（爆炸性增长）

    Vector2 lookDir = Vector2.zero;
    bool snap = false;


    private void Start()
    {
        equipHolder = this?.GetComponent<EquipHolder>();

        Physics2D.defaultContactOffset = 0.01f;
    }

    private void FixedUpdate()
    {
        if (snap)
            ObjectAttract();
    }

    void ObjectAttract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position,
        snapDistance,
        LayerMask.GetMask("SnapLayer")
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            Magnet snapObject = hit?.GetComponent<Magnet>();
            if(snapObject == null || hit.gameObject.layer != 6) continue;

            Vector2 objectDir = (snapObject.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(lookDir, objectDir);
            if (angle >= snapAngle / 2) continue;

            snapObject.BeingAttract(this);
        }
    }

    void SnapMagnet(Magnet magnet)
    {
        if(magnet == null) return;
        if (Vector2.Distance(transform.position, magnet.transform.position) > magnetHoldRadius) return;

        if (!ObjectinPlace.Contains(magnet))
            ObjectinPlace.Add(magnet);

        //等待重构 不应该放在这个类
        if (equipHolder != null)
            equipHolder.EquipArmed(magnet.transform);

        magnet.SnapFinalize(this);
    }

    public void ReleaseMagnet(Magnet magnet)
    {
        if(ObjectinPlace.Contains(magnet))
            ObjectinPlace.Remove(magnet);
    }

    #region 物体连接

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Magnet magnet = collision.collider?.GetComponent<Magnet>();
        if(magnet == null || collision.collider.gameObject.layer != 6) return;
        if (magnet.MagnetParent != null || collision.collider.gameObject.layer != 6) return;

        if (!ObjectinPlace.Contains(magnet))
            StartCoroutine(SnapStart(collision.collider.GetComponent<Rigidbody2D>()));
    }

    IEnumerator SnapStart(Rigidbody2D targetBody)
    {
        yield return new WaitForSeconds(0.1f);
        SnapMagnet(targetBody?.GetComponent<Magnet>());
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
