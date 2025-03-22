using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[Serializable]
public class Magnet : MonoBehaviour
{
    private PolygonCollider2D magnetCollider;
    private PhysicalCharacter physicsCharacter;
    private Equip equip;
    private MagAnimation snapAnimation;

    [Header("��������")]
    [SerializeField] private float magPower = 1; // ����ǿ�� Ĭ��Ϊ1
    [SerializeField] private float radius = 1f; //���嵽�ߵľ���
    [SerializeField] private float minSpeed = 2f; // �ٶ�
    [SerializeField] private float speedClamp = 30f; // �ٶ����ޣ���ʧ�٣�

    MagSource magnetParent;
    MagSource magSource;
    bool isAttracted;

    private void Start()
    {
        magnetCollider = GetComponent<PolygonCollider2D>();
        physicsCharacter = GetComponent<PhysicalCharacter>();
        equip = GetComponentInChildren<Equip>();
        snapAnimation = GetComponentInChildren<MagAnimation>();

        gameObject.layer = LayerMask.NameToLayer("MagnetLayer");
    }

    private void Update()
    {
        if (isAttracted && magSource != null)
        {
            float distance = Vector2.Distance(transform.position, magSource.transform.position);
            float attractSpeed = CalculateSpeed(distance);
            physicsCharacter.SetMoveSpeed(attractSpeed);
        }
    }

    public void InvokeAttract(MagSource snapSource)
    {
        if(magSource == snapSource) return;

        magSource = snapSource;
        Transform magTarget = snapSource.transform;
        float distance = Vector3.Distance(transform.position, magTarget.position);

        physicsCharacter.SetTarget(magTarget, CalculateMoveDuration(distance));
        isAttracted = true;
    }

    public void StopAttract(MagSource snapSource)
    {
        isAttracted = false;
        physicsCharacter.ToRoam();
    }

    public void SnapFinalize(MagSource snapSource)
    {
        transform.SetParent(snapSource.transform);
        magnetParent = snapSource;
        isAttracted = false;
        gameObject.layer = 0;

        physicsCharacter.ToRoam();
    }

    public void MagnetRelease(Equip equip)
    {
        magnetParent.ReleaseMagnet(this);
        magnetParent = null;
        transform.SetParent(null);
    }

    #region �����ٶ�

    /*
    �ٶȷֶι���
    1. distance > far �� minSpeed
    2. mid < distance �� far �� ��������
    3. close < distance �� mid �� ��������
    4. strongAccel < distance �� close �� �ݴμ���
    5. distance �� strongAccel �� ���޳��
    */

    float CalculateMoveDuration(float distance)
    {
        if (distance > magSource.farDistanceCoef * magSource.snapDistance)
            return magSource.maxAttractDuration;

        if (distance > magSource.midDistanceCoef * magSource.snapDistance)
            return magSource.maxAttractDuration * magSource.farDistanceCoef;

        if (distance > magSource.closeDistanceCoef * magSource.snapDistance)
            return magSource.maxAttractDuration * magSource.midDistanceCoef;

        return magSource.maxAttractDuration * magSource.closeDistanceCoef;
    }

    float CalculateSpeed(float distance)
    {
        if (distance > magSource.farDistanceCoef * magSource.snapDistance)
            return minSpeed;

        if (distance > magSource.midDistanceCoef * magSource.snapDistance)
        {
            // ���λ���������weak �� medium����ʹ�ø�ƽ���Ĳ�ֵ��ʽ
            float t = Mathf.InverseLerp(magSource.farDistanceCoef, magSource.midDistanceCoef, distance);
            return Mathf.Lerp(minSpeed, minSpeed * 4, Mathf.SmoothStep(0f, 1f, t));  // ʹ�� SmoothStep ��ƽ������
        }

        if (distance > magSource.closeDistanceCoef * magSource.snapDistance)
        {
            // ���������� (medium �� high)
            return Mathf.Lerp(minSpeed * 4, speedClamp * 0.7f,
                Mathf.InverseLerp(magSource.midDistanceCoef, magSource.closeDistanceCoef, distance));
        }

        if (distance > magSource.strongAccelRange * magSource.snapDistance)
        {
            // �ݴμ��� (high �� max)������ͻ�䣬ʹ�ø�ƽ���Ĳ�ֵ��ʽ
            float t = 1 - Mathf.InverseLerp(magSource.closeDistanceCoef, magSource.strongAccelRange, distance);
            return Mathf.Lerp(speedClamp * 0.7f, speedClamp, Mathf.SmoothStep(0f, 1f, t));  // ʹ�� SmoothStep
        }

        // ���޳������max����Ƶ��
        float overflow = Mathf.Clamp01(1 - distance / magSource.strongAccelRange);
        return speedClamp * (1 + overflow * 0.5f);
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
