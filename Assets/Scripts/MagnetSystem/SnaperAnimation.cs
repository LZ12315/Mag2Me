using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnaperAnimation : MonoBehaviour
{
    [Header("�α����")]
    [SerializeField] private float squeezeFactorX = 0.7f;  // ����ѹǿ��
    [SerializeField] private float stretchFactorY = 1.3f;  // ��������ǿ��
    [SerializeField] private float deformDuration = 0.25f;  // �α����ʱ��
    [SerializeField] private Ease squeezeEase = Ease.OutElastic; // �α�����

    private Transform _deformNode; // ʵ�����α���Ƶ��Ӿ��ڵ㣨����ײ����룩

    private void Start()
    {
        _deformNode = transform;
    }

    public void PlayMagneticDeform(Vector2 contactDirection)
    {
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

}
