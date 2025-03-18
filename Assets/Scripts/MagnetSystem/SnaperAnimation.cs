using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnaperAnimation : MonoBehaviour
{
    [Header("形变参数")]
    [SerializeField] private float squeezeFactorX = 0.7f;  // 横向挤压强度
    [SerializeField] private float stretchFactorY = 1.3f;  // 纵向拉伸强度
    [SerializeField] private float deformDuration = 0.25f;  // 形变持续时间
    [SerializeField] private Ease squeezeEase = Ease.OutElastic; // 形变曲线

    private Transform _deformNode; // 实际受形变控制的视觉节点（与碰撞体分离）

    private void Start()
    {
        _deformNode = transform;
    }

    public void PlayMagneticDeform(Vector2 contactDirection)
    {
        // 通过接触方向决定形变方向（示例：x轴方向为横向）
        bool isHorizontalContact = Mathf.Abs(contactDirection.x) > Mathf.Abs(contactDirection.y);

        // 效果叠加顺序：挤压 → 回弹 → 微颤动
        Sequence deformSequence = DOTween.Sequence();

        // 阶段一：压缩形变
        deformSequence.Append(
            _deformNode.DOScaleX(squeezeFactorX, deformDuration * 0.2f)
            .SetEase(Ease.OutQuad)
        );

        // 同时拉伸Y轴
        deformSequence.Join(
            _deformNode.DOScaleY(stretchFactorY, deformDuration * 0.2f)
            .SetEase(Ease.OutQuad)
        );

        // 阶段二：弹性恢复
        deformSequence.Append(
            _deformNode.DOScale(new Vector3(1f, 1f, 1f), deformDuration * 0.8f)
            .SetEase(squeezeEase)
        );
    }

}
