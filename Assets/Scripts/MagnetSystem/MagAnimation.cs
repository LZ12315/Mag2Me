using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PhysicalCharacter physicalCharacter;

    [Header("������")]
    [SerializeField] private float squeezeFactorX = 0.7f;  // ����ѹǿ��
    [SerializeField] private float stretchFactorY = 1.3f;  // ��������ǿ��
    [SerializeField] private float deformDuration = 0.25f;  // �α����ʱ��
    [SerializeField] private Ease squeezeEase = Ease.OutElastic; // �α�����
    private Transform _deformNode; // ʵ�����α���Ƶ��Ӿ��ڵ㣨����ײ����룩

    [Header("֡����")]
    [SerializeField] private float animatorSpeed = 1;

    [Header("��Ч")]
    [SerializeField] private MMFeedbacks PullFeedback;
    [SerializeField] private MMFeedbacks PushFeedback;
    [SerializeField] private MMFeedbacks HitFeedback;
    [SerializeField] private MMFeedbacks DeadFeedback;

    private void Start()
    {
        animator = this?.GetComponent<Animator>();
        if(animator != null)
            animator.speed = animatorSpeed;

        if (physicalCharacter == null)
        {
            physicalCharacter = this?.GetComponent<PhysicalCharacter>();
            if (physicalCharacter == null)
                physicalCharacter = GetComponentInParent<PhysicalCharacter>();
        }
    }

    private void Update()
    {
        SetAnimator();
    }

    void SetAnimator()
    {
        if (animator == null) return;

        animator.SetBool("isMoving", physicalCharacter.isMoving);
    }

    #region ������

    public void PlayMagneticDeform(Vector2 contactDirection)
    {
        _deformNode = transform;

        // ͨ���Ӵ���������α䷽��ʾ����x�᷽��Ϊ����
        bool isHorizontalContact = Mathf.Abs(contactDirection.x) > Mathf.Abs(contactDirection.y);

        // Ч������˳�򣺼�ѹ �� �ص� �� ΢����
        Sequence deformSequence = DOTween.Sequence();

        // �׶�һ��ѹ���α�
        deformSequence.Append(
            _deformNode.DOScaleX(squeezeFactorX, deformDuration * 0.2f)
            .SetEase(Ease.OutQuad)
        );

        // ͬʱ����Y��
        deformSequence.Join(
            _deformNode.DOScaleY(stretchFactorY, deformDuration * 0.2f)
            .SetEase(Ease.OutQuad)
        );

        // �׶ζ������Իָ�
        deformSequence.Append(
            _deformNode.DOScale(new Vector3(1f, 1f, 1f), deformDuration * 0.8f)
            .SetEase(squeezeEase)
        );
    }

    #endregion

    #region ��Ч

    public void PullVFX(MagSource magSource, bool play)
    {
        if (PullFeedback == null) return;

        if(play)
            PullFeedback.PlayFeedbacks();
        else
            PullFeedback.StopFeedbacks();
    }

    public void PushVFX(MagSource magSource, bool play)
    {
        if (PushFeedback == null) return;

        if (play)
            PushFeedback.PlayFeedbacks();
        else
            PushFeedback.StopFeedbacks();
    }

    public void HitVFX(Character character)
    {
        if(HitFeedback!= null)
            HitFeedback.PlayFeedbacks();
    }

    public void DeadVFX(Character character)
    {
        if(DeadFeedback!= null) 
            DeadFeedback.PlayFeedbacks();
    }

    #endregion
}
