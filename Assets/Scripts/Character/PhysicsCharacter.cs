using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum MoveType
{
    Destinational, Physical, Roam
}

public class PhysicsCharacter : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float velocityCorrection = 0f;
    [SerializeField] private MoveType moveType = MoveType.Physical;

    private float moveSpeed = 1;
    private Vector2 moveDir = Vector2.zero;
    private Transform target;
    private Tweener tweener;

    private void Update()
    {
        Move();
    }

    #region 移动相关

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
            if (target == null || tweener == null) return;
            tweener.ChangeEndValue(target.position, true);  // 动态更新终点位置
        }
    }

    void SwitchMoveType(MoveType type)
    {
        if (tweener != null)
        {
            tweener.Kill();  // 停止现有动画
            transform.DOComplete();  // 完成当前动画，防止立即停止
            transform.DOLocalMove(transform.position, 1f).SetEase(Ease.OutQuad);  // 缓慢减速停止
        }
        moveType = type;
    }

    // 开始自由运动
    public void SetVelocity(Vector2 dir, float speed)
    {
        moveDir = dir;
        moveSpeed = speed;
        SwitchMoveType(MoveType.Physical);
    }

    // 开始追踪目标
    public void SetTarget(Transform traceTarget, float duration)
    {
        target = traceTarget;
        if (target == null) return;

        SwitchMoveType(MoveType.Destinational);
        tweener = transform.DOMove(target.position, 0.1f).SetEase(Ease.InQuad);
    }

    // 动态调整追踪速度
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void ToRoam()
    {
        SwitchMoveType(MoveType.Roam);
    }

    #endregion

}
