using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Magnet : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private PolygonCollider2D magnetCollider;
    private SnaperAnimation snapAnimation;
    private Equip equip;

    [Header("��������")]
    [SerializeField] private float magPower = 0; // ����ǿ�� Ĭ��Ϊ0
    [SerializeField] private float radius = 1f; //���嵽�ߵľ���
    [SerializeField] private float velocityClamp = 30f; // �ٶ����ޣ���ʧ�٣�

    [SerializeField] MagSource magnetParent;
    MagSource magSource;
    Sequence sequence = null;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        magnetCollider = GetComponent<PolygonCollider2D>();
        equip = GetComponentInChildren<Equip>();
        snapAnimation = GetComponentInChildren<SnaperAnimation>();
        radius = CalculateRadius();

        gameObject.layer = 6;
        Physics2D.defaultContactOffset = 0.01f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider?.GetComponent<Magnet>() != null && magnetParent != null)
            magnetParent.OnCollisionEnter2D(collision);
    }

    public void BeingAttract(MagSource snapSource)
    {
        this.magSource = snapSource;
        Transform snapTarget = snapSource.transform;

        Vector2 targetDir = (Vector2)snapTarget.position - (Vector2)transform.position;
        float distance = Vector3.Distance(transform.position, snapTarget.position);

        float acceleration = CalculateAcceleration(distance, snapSource);
        Vector2 force = targetDir.normalized * acceleration * snapSource.MagPower;

        if (rigidBody != null)
        {
            rigidBody.AddForce(force, ForceMode2D.Force);
            rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, velocityClamp);
        }
    }

    public void SnapFinalize(MagSource snapSource)
    {
        rigidBody.transform.SetParent(snapSource.transform);
        magnetParent = snapSource;

        rigidBody.isKinematic = true;
        rigidBody.gravityScale = 0;
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0;

        if(snapAnimation)
        snapAnimation.PlayMagneticDeform(snapSource.transform.position - transform.position);
    }

    public void MagnetRelease()
    {
        magnetParent.ReleaseMagnet(this);
        magnetParent = null;
        rigidBody.transform.SetParent(null);
        gameObject.layer = 0;
        rigidBody.isKinematic = false;
        magnetCollider.isTrigger = true;
    }

    #region �����ٶ�

    float CalculateAcceleration(float distance, MagSource snapSource)
    {
        /*
        �ٶȷֶι���
        1. distance > far �� minAcceleration
        2. mid < distance �� far �� minAcceleration
        3. close < distance �� mid �� midAcceleration
        4. strongAccel < distance �� close �� maxAcceleration
        5. distance �� strongAccel �� maxAcceleration * 2
        */

        if (distance > snapSource.farDistanceCoef * snapSource.snapDistance)
            return snapSource.minAcceleration;

        else if (distance > snapSource.midDistanceCoef * snapSource.snapDistance)
            return Mathf.Lerp(snapSource.minAcceleration, snapSource.midAcceleration,
                Mathf.InverseLerp(snapSource.farDistanceCoef, snapSource.midDistanceCoef, distance));

        else if (distance > snapSource.closeDistanceCoef * snapSource.snapDistance)
            return Mathf.Lerp(snapSource.midAcceleration, snapSource.maxAcceleration * 0.6f,
                Mathf.InverseLerp(snapSource.midDistanceCoef, snapSource.closeDistanceCoef, distance));

        else if (distance > snapSource.strongAccelRange * snapSource.snapDistance)
            return Mathf.Lerp(snapSource.maxAcceleration * 0.6f, snapSource.maxAcceleration,
                Mathf.InverseLerp(snapSource.closeDistanceCoef, snapSource.strongAccelRange, distance));

        else
            return snapSource.maxAcceleration * 2; // �Ӵ����������
    }

    #endregion

    #region ��ʼ��

    float CalculateRadius()
    {
        if (magnetCollider == null)
            return 0;

        float distance = DistanceToLine(magnetCollider.points[1], magnetCollider.points[0], Vector3.zero);
        return distance * transform.localScale.x;
    }

    float DistanceToLine(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 AB = B - A;
        Vector3 AP = P - A;

        // ����ͶӰ����
        float t = Vector3.Dot(AP, AB) / AB.sqrMagnitude;

        if (t <= 0)
            return Vector3.Distance(P, A); // ���뵽���A
        else if (t >= 1)
            return Vector3.Distance(P, B); // ���뵽�յ�B
        else
        {
            Vector3 Q = A + t * AB; // ��������
            return Vector3.Distance(P, Q); // �㵽����ľ���
        }
    }

    #endregion

    #region ���� 

    public MagSource MagnetParent => magnetParent;

    #endregion

}
