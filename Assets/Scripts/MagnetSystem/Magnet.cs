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

    [Header("磁体设置")]
    [SerializeField] private float magPower = 1; // 磁力强度 默认为1
    [SerializeField] private float radius = 1f; //物体到边的距离
    [SerializeField] private float minSpeed = 2f; // 速度
    [SerializeField] private float speedClamp = 30f; // 速度上限（防失速）

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

    #region 吸引速度

    /*
    速度分段规则：
    1. distance > far → minSpeed
    2. mid < distance ≤ far → 缓动提升
    3. close < distance ≤ mid → 线性爬升
    4. strongAccel < distance ≤ close → 幂次加速
    5. distance ≤ strongAccel → 极限冲刺
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
            // 二次缓动提升（weak → medium），使用更平滑的插值方式
            float t = Mathf.InverseLerp(magSource.farDistanceCoef, magSource.midDistanceCoef, distance);
            return Mathf.Lerp(minSpeed, minSpeed * 4, Mathf.SmoothStep(0f, 1f, t));  // 使用 SmoothStep 来平滑过渡
        }

        if (distance > magSource.closeDistanceCoef * magSource.snapDistance)
        {
            // 线性增速区 (medium → high)
            return Mathf.Lerp(minSpeed * 4, speedClamp * 0.7f,
                Mathf.InverseLerp(magSource.midDistanceCoef, magSource.closeDistanceCoef, distance));
        }

        if (distance > magSource.strongAccelRange * magSource.snapDistance)
        {
            // 幂次加速 (high → max)，避免突变，使用更平滑的插值方式
            float t = 1 - Mathf.InverseLerp(magSource.closeDistanceCoef, magSource.strongAccelRange, distance);
            return Mathf.Lerp(speedClamp * 0.7f, speedClamp, Mathf.SmoothStep(0f, 1f, t));  // 使用 SmoothStep
        }

        // 极限冲刺区（max及超频）
        float overflow = Mathf.Clamp01(1 - distance / magSource.strongAccelRange);
        return speedClamp * (1 + overflow * 0.5f);
    }

    #endregion

    #region 初始化

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

        // 计算投影比例
        float t = Vector3.Dot(AP, AB) / AB.sqrMagnitude;

        if (t <= 0)
            return Vector3.Distance(P, A); // 距离到起点A
        else if (t >= 1)
            return Vector3.Distance(P, B); // 距离到终点B
        else
        {
            Vector3 Q = A + t * AB; // 垂足坐标
            return Vector3.Distance(P, Q); // 点到垂足的距离
        }
    }

    #endregion

    #region 其他 

    public MagSource MagnetParent => magnetParent;

    #endregion

}
