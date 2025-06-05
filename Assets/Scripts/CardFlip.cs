using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    // Flip ���� enum
    public enum FlipDirection
    {
        Left,
        Right,
        Top,
        Bottom,
    };

    // Flip�� �� ���� ����
    public FlipDirection flipDirection = FlipDirection.Left;
    // Flip�� �ҿ�� �ð�
    public float duration = 1f;

    // Flip �ߺ� ���� ���� �÷���
    private bool isFlipping = false;

    // Inspector���� �׽�Ʈ�� ������ �� ����.
    [ContextMenu("Test Flip Function")]
    private void TestFlipFunction()
    {
        Flip(flipDirection);
    }

    // Flip ��� ���� Right ���� Flip�� �⺻ ����.
    private void Flip(FlipDirection direction = FlipDirection.Right)
    {
        if (isFlipping)
        {
            return;
        }
        isFlipping = true;

        // ���� ȸ�� ���ʹϾ�
        Quaternion currentRotation = transform.rotation;

        Vector3 angle3 = new Vector3(0, 0, 0);

        switch (direction)
        {
            case FlipDirection.Left:
                angle3.y = -180f;
                break;
            case FlipDirection.Right:
                angle3.y = 180f;
                break;
            case FlipDirection.Top:
                angle3.x = -180f;
                break;
            case FlipDirection.Bottom:
                angle3.x = 180f;
                break;
        }

        // �߰� ȸ�� ���ʹϾ�
        Quaternion rotationDelta = Quaternion.Euler(angle3);

        // ��ǥ ȸ�� = ���� ȸ�� * �߰� ȸ��
        Quaternion targetRotation = currentRotation * rotationDelta;

        // DOTween���� duration ���� ȸ�� ����.
        transform.DORotateQuaternion(targetRotation, duration)
            .SetEase(Ease.InOutQuad)
            // ȸ���� ������ �÷��� ����.
            .OnComplete(() => { isFlipping = false; });
    }
}
