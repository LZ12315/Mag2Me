using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum MoveType
{
    Destinational, Velocity, Roam, Idle
}

public class PhysicalCharacter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("变换参数")]
    [SerializeField] private Vector2 lastOrientation;

    [Header("移动参数")]
    [SerializeField] private MoveType moveType = MoveType.Idle;
    [SerializeField] private float velocityCorrection = 0f;

    [Header("受力参数")]
    [SerializeField] private float impluseForceTime = 0.4f;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        lastOrientation = new Vector2(1, 0);
        canPhysicalMove = true;
    }

    private void FixedUpdate()
    {
        CalculateOrien();

        if(canPhysicalMove)
            Move();
    }

    #region 变换

    Vector3 lastPos = Vector3.zero;

    private void CalculateOrien()
    {
        Vector2 dir = transform.position - lastPos;
        Vector2 orientation = new Vector2(Mathf.Sign(dir.x), Mathf.Sign(dir.y));

        if(!Mathf.Approximately(orientation.magnitude, 0))
            lastOrientation = orientation;

        //CharacterFlip();
        lastPos = transform.position;
    }

    void CharacterFlip()
    {
        if (lastOrientation.x < 0)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }

    public Vector2 Orientation => lastOrientation;

    #endregion

    #region 移动

    private Tweener tweener;
    [SerializeField] bool canPhysicalMove;
    private float moveSpeed = 1;
    private Vector2 moveDir = Vector2.zero;
    private Transform target;

    public bool isMoving => tweener.IsActive();

    private void Move()
    {
        if (moveType == MoveType.Velocity)
        {
            float moveStep = (moveSpeed + velocityCorrection) * Time.deltaTime;
            Vector3 targetPostion = transform.position + new Vector3(moveStep * moveDir.x, moveStep * moveDir.y, 0);

            if (tweener == null || !tweener.IsActive())
                tweener = transform.DOMove(targetPostion, Time.deltaTime).SetEase(Ease.Linear);  // 创建一个新的 Tween
            else
                tweener.ChangeEndValue(targetPostion, true);  // 如果已有Tween正在运行，更新目标位置
        }
        else if (moveType == MoveType.Destinational)
        {
            if (target == null || tweener == null || !tweener.IsActive()) return;
            tweener.ChangeEndValue(target.position, true);  // 动态更新终点位置
            tweener.timeScale = moveSpeed;
        }
    }

    void SwitchMoveType(MoveType type)
    {
        if (tweener != null)
        {
            tweener.Kill();  // 停止现有动画
            transform.DOComplete();  // 完成当前动画，防止立即停止
        }
        moveType = type;
        canPhysicalMove = true;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    // 开始自由运动
    public void SetVelocity(Vector2 dir, float speed)
    {
        if (!canPhysicalMove) return;
        moveDir = dir;
        moveSpeed = speed;
        if(moveType != MoveType.Velocity)
            SwitchMoveType(MoveType.Velocity);
    }

    // 开始追踪目标
    public void SetTarget(Transform traceTarget, float duration)
    {
        if (!canPhysicalMove) return;

        target = traceTarget;
        if (target == null) return;

        if(moveType != MoveType.Destinational)
            SwitchMoveType(MoveType.Destinational);
        tweener = transform.DOMove(target.position, duration);
    }

    // 开始漫游
    public void ToRoam()
    {
        if (!canPhysicalMove) return;
        SwitchMoveType(MoveType.Roam);
    }

    public void Idle()
    {
        if (!canPhysicalMove) return;
        SwitchMoveType(MoveType.Idle);
    }

    #endregion

    #region 受力

    Vector2 forceDir;
    float force;

    public void AddForceImpluse(Vector2 dir, float force)
    {
        forceDir = dir;
        this.force = force;
        SwitchMoveType(MoveType.Idle);
        canPhysicalMove = false;

        ForceMove(impluseForceTime);
    }

    public void AddForce(Vector2 dir, float force, float forceTime)
    {
        forceDir = dir;
        this.force = force;
        SwitchMoveType(MoveType.Idle);
        canPhysicalMove = false;

        ForceMove(forceTime);
    }

    void ForceMove(float moveTime)
    {
        float moveStep = force + velocityCorrection;
        Vector3 targetPostion = transform.position + new Vector3(moveStep * forceDir.x, moveStep * forceDir.y, 0);

        if (tweener != null)
            tweener.Kill();
        transform.DOMove(targetPostion, moveTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => SwitchMoveType(MoveType.Idle));
    }

    #endregion
}
