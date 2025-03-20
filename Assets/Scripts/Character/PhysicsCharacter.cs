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
    [Header("�ƶ�����")]
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

    #region �ƶ����

    private void Move()
    {
        if (moveType == MoveType.Physical)
        {
            float moveStep = (moveSpeed + velocityCorrection) * Time.deltaTime;
            Vector3 targetPostion = transform.position + new Vector3(moveStep * moveDir.x, moveStep * moveDir.y, 0);

            if (tweener == null || !tweener.IsActive())
                tweener = transform.DOMove(targetPostion, Time.deltaTime).SetEase(Ease.Linear);  // ����һ���µ� Tween
            else
                tweener.ChangeEndValue(targetPostion, true);  // �������Tween�������У�����Ŀ��λ��
        }
        else if (moveType == MoveType.Destinational)
        {
            if (target == null || tweener == null) return;
            tweener.ChangeEndValue(target.position, true);  // ��̬�����յ�λ��
        }
    }

    void SwitchMoveType(MoveType type)
    {
        if (tweener != null)
        {
            tweener.Kill();  // ֹͣ���ж���
            transform.DOComplete();  // ��ɵ�ǰ��������ֹ����ֹͣ
            transform.DOLocalMove(transform.position, 1f).SetEase(Ease.OutQuad);  // ��������ֹͣ
        }
        moveType = type;
    }

    // ��ʼ�����˶�
    public void SetVelocity(Vector2 dir, float speed)
    {
        moveDir = dir;
        moveSpeed = speed;
        SwitchMoveType(MoveType.Physical);
    }

    // ��ʼ׷��Ŀ��
    public void SetTarget(Transform traceTarget, float duration)
    {
        target = traceTarget;
        if (target == null) return;

        SwitchMoveType(MoveType.Destinational);
        tweener = transform.DOMove(target.position, 0.1f).SetEase(Ease.InQuad);
    }

    // ��̬����׷���ٶ�
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
