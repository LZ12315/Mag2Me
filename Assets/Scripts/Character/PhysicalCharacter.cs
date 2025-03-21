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
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("�任����")]
    [SerializeField] private Vector2 lastOrientation;

    [Header("�ƶ�����")]
    [SerializeField] private MoveType moveType = MoveType.Idle;
    [SerializeField] private float velocityCorrection = 0f;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private Vector2 moveDir = Vector2.zero;
    [SerializeField] private Transform target;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        canPhysicalMove = true;
    }

    private void Update()
    {
        if(canPhysicalMove)
            Move();
        CalculateOrien();
    }

    #region �任

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

    #region �ƶ�

    private Tweener tweener;
    [SerializeField] bool canPhysicalMove;

    public bool isMoving => tweener.IsActive();

    private void Move()
    {
        if (moveType == MoveType.Velocity)
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
            if (target == null || tweener == null || !tweener.IsActive()) return;
            tweener.ChangeEndValue(target.position, true);  // ��̬�����յ�λ��
            tweener.timeScale = moveSpeed;
        }
    }

    void SwitchMoveType(MoveType type)
    {
        if (tweener != null)
        {
            tweener.Kill();  // ֹͣ���ж���
            transform.DOComplete();  // ��ɵ�ǰ��������ֹ����ֹͣ
        }
        moveType = type;
        canPhysicalMove = true;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    // ��ʼ�����˶�
    public void SetVelocity(Vector2 dir, float speed)
    {
        if (!canPhysicalMove) return;

        moveDir = dir;
        moveSpeed = speed;
        if(moveType != MoveType.Velocity)
            SwitchMoveType(MoveType.Velocity);
    }

    // ��ʼ׷��Ŀ��
    public void SetTarget(Transform traceTarget, float duration)
    {
        if (!canPhysicalMove) return;

        target = traceTarget;
        if (target == null) return;

        if(moveType != MoveType.Destinational)
            SwitchMoveType(MoveType.Destinational);
        tweener = transform.DOMove(target.position, duration);
    }

    // ��ʼ����
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

    #region ����

    Vector2 forceDir;
    float force;

    public void AddForce(Vector2 dir, float force)
    {
        forceDir = dir;
        this.force = force;
        SwitchMoveType(MoveType.Idle);
        canPhysicalMove = false;

        ForceMove();
    }

    void ForceMove()
    {
        float moveStep = force + velocityCorrection;
        Vector3 targetPostion = transform.position + new Vector3(moveStep * forceDir.x, moveStep * forceDir.y, 0);

        if (tweener != null)
            tweener.Kill();
        transform.DOMove(targetPostion, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => SwitchMoveType(MoveType.Idle));
    }

    #endregion
}
