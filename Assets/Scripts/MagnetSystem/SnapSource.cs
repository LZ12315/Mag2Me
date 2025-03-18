using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(PolygonCollider2D))]
public class SnapSource : MonoBehaviour, IMagnetRelated
{
    private Rigidbody2D rigidBody;
    private PolygonCollider2D polygonCollider;
    private PlayerController playerController;
    private SnaperAnimation snapAnimation;
    private EquipHolder equipHolder;

    [Header("吸附设置")]
    [SerializeField] private float snapPower = 1f; //物体磁力强度
    [SerializeField] private float snapAngle = 60f; //磁力效用角度
    [SerializeField] public float snapDistance = 2f; //磁力效用距离
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
        polygonCollider = GetComponent<PolygonCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        equipHolder = this?.GetComponent<EquipHolder>();
        snapAnimation = GetComponentInChildren<SnaperAnimation>();

        Physics2D.defaultContactOffset = 0.01f;
    }

    private void Update()
    {
        lookDir = playerController.LookDir;
        snap = playerController.SnapTrigger;
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

    void SnapFinalize(HingeJoint2D joint, Rigidbody2D targetBody)
    {
        Magnet magnet = targetBody?.GetComponent<Magnet>();
        if(magnet == null) return;

        if (!ObjectinPlace.Contains(magnet))
            ObjectinPlace.Add(magnet);

        //等待重构 不应该放在这个类
        if (equipHolder != null)
            equipHolder.EquipArmed(targetBody.transform);

        magnet.SnapFinalize(this);
        Destroy(joint);
        snapAnimation.PlayMagneticDeform(targetBody.transform.position - transform.position);
        DeSpeed();
    }

    public void ReleaseMagnet(Magnet magnet)
    {
        if(ObjectinPlace.Contains(magnet))
            ObjectinPlace.Remove(magnet);
    }

    #region 物体连接

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Magnet magnet = collision.collider?.GetComponent<Magnet>();
        if(magnet == null || collision.collider.gameObject.layer != 6) return;

        if (!ObjectinPlace.Contains(magnet))
        {
            CreatePhysicalJoint(collision.collider.GetComponent<Rigidbody2D>(), rigidBody);
        }
    }

    public void OnChildCollisionEnter2D(Collision2D collision, Rigidbody2D child)
    {
        Magnet magnet = collision.collider?.GetComponent<Magnet>();
        if (magnet == null || collision.collider.gameObject.layer != 6) return;

        if (!ObjectinPlace.Contains(magnet))
        {
            CreatePhysicalJoint(collision.collider.GetComponent<Rigidbody2D>(), child);
        }
    }

    void CreatePhysicalJoint(Rigidbody2D targetBody, Rigidbody2D jointBody)
    {
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();

        // Joint参数设置
        joint.connectedBody = targetBody;
        joint.autoConfigureConnectedAnchor = false;
        joint.breakForce = Mathf.Infinity;
        joint.enableCollision = true;

        StartCoroutine(PreciseAnchorPositioning(joint, targetBody, jointBody));
    }

    IEnumerator PreciseAnchorPositioning(HingeJoint2D joint, Rigidbody2D targetBody, Rigidbody2D jointBody)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = 6;
        ContactPoint2D[] contacts = new ContactPoint2D[2];
        jointBody.GetComponent<Collider2D>().GetContacts(filter, contacts);

        if (contacts.Length > 0)
        {
            Vector2 anchorPoint = contacts[0].point;
            if (anchorPoint == Vector2.zero)
                anchorPoint = transform.position;

            joint.anchor = jointBody.transform.InverseTransformPoint(anchorPoint);
            joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(anchorPoint);
        }

        yield return new WaitForSeconds(0.5f);
        SnapFinalize(joint, targetBody);
    }

    void DeSpeed()
    {
        Vector2 latelyVeloviry = rigidBody.velocity;
        rigidBody.velocity = latelyVeloviry * 0.1f;
    }

    #endregion

    #region 其他

    public float MagPower => snapPower;

    float IMagnetRelated.MagPower { get => snapPower; }

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
