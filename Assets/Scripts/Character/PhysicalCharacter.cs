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
    Destinational, Physical, Roam
}

public class PhysicalCharacter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("变换参数")]
    [SerializeField] private Vector2 lastOrientation;

    [Header("移动参数")]
    [SerializeField] private MoveType moveType = MoveType.Physical;
    [SerializeField] private float velocityCorrection = 0f;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private Vector2 moveDir = Vector2.zero;
    [SerializeField] private Transform target;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
        CalculateOrien();
    }

    #region 角色变换

    Vector3 lastPos = Vector3.zero;

    private void CalculateOrien()
    {
        Vector2 dir = transform.position - lastPos;
        Vector2 orientation = new Vector2(Mathf.Sign(dir.x), Mathf.Sign(dir.y));

        if(!Mathf.Approximately(orientation.magnitude, 0))
            lastOrientation = orientation;

        CharacterFlip();
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


    #region 移动相关

    private Tweener tweener;

    public bool isMoving => tweener.IsActive();

    private void Move()
    {
        if (moveType == MoveType.Physical)
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
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    // 开始自由运动
    public void SetVelocity(Vector2 dir, float speed)
    {
        moveDir = dir;
        moveSpeed = speed;
        if(moveType != MoveType.Physical)
            SwitchMoveType(MoveType.Physical);
    }

    // 开始追踪目标
    public void SetTarget(Transform traceTarget, float duration)
    {
        target = traceTarget;
        if (target == null) return;

        if(moveType != MoveType.Destinational)
            SwitchMoveType(MoveType.Destinational);
        tweener = transform.DOMove(target.position, duration);
    }

    // 开始漫游
    public void ToRoam()
    {
        SwitchMoveType(MoveType.Roam);
    }

    #endregion


}
