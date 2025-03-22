using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagAnimation : MonoBehaviour
{
    [SerializeField] private PhysicalCharacter physicalCharacter;

    [Header("特效")]
    [SerializeField] private MMFeedbacks PullFeedback;
    [SerializeField] private MMFeedbacks PushFeedback;
    [SerializeField] private MMFeedbacks SnapFeedback;
    [SerializeField] private MMFeedbacks HitFeedback;
    [SerializeField] private MMFeedbacks DeadFeedback;

    private void Start()
    {
        if (physicalCharacter == null)
        {
            physicalCharacter = this?.GetComponent<PhysicalCharacter>();
            if (physicalCharacter == null)
                physicalCharacter = GetComponentInParent<PhysicalCharacter>();
        }
    }

    #region 特效

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

    public void SnapVFX(MagSource magSource)
    {
        if(SnapFeedback == null) return;
        SnapFeedback.PlayFeedbacks();
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

    #region 物理动画

    //[Header("物理动画")]
    //[SerializeField] private float squeezeFactorX = 0.7f;  // 横向挤压强度
    //[SerializeField] private float stretchFactorY = 1.3f;  // 纵向拉伸强度
    //[SerializeField] private float deformDuration = 0.25f;  // 形变持续时间
    //[SerializeField] private Ease squeezeEase = Ease.OutElastic; // 形变曲线
    //private Transform _deformNode; // 实际受形变控制的视觉节点（与碰撞体分离）

    //public void PlayMagneticDeform(Vector2 contactDirection)
    //{
    //    _deformNode = transform;

    //    // 通过接触方向决定形变方向（示例：x轴方向为横向）
    //    bool isHorizontalContact = Mathf.Abs(contactDirection.x) > Mathf.Abs(contactDirection.y);

    //    // 效果叠加顺序：挤压 → 回弹 → 微颤动
    //    Sequence deformSequence = DOTween.Sequence();

    //    // 阶段一：压缩形变
    //    deformSequence.Append(
    //        _deformNode.DOScaleX(squeezeFactorX, deformDuration * 0.2f)
    //        .SetEase(Ease.OutQuad)
    //    );

    //    // 同时拉伸Y轴
    //    deformSequence.Join(
    //        _deformNode.DOScaleY(stretchFactorY, deformDuration * 0.2f)
    //        .SetEase(Ease.OutQuad)
    //    );

    //    // 阶段二：弹性恢复
    //    deformSequence.Append(
    //        _deformNode.DOScale(new Vector3(1f, 1f, 1f), deformDuration * 0.8f)
    //        .SetEase(squeezeEase)
    //    );
    //}

    #endregion

}
