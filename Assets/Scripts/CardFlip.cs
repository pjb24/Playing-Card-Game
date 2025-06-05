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

    // Flip ũ�� enum
    public enum FlipSize
    {
        One,
        Half
    };

    // Flip�� �� ���� ����
    public FlipDirection flipDirection = FlipDirection.Left;
    // Flip�� �ҿ�� �ð�
    public float duration = 1f;
    // Flip�� �� ũ��
    public FlipSize flipSize = FlipSize.One;

    // Flip �ߺ� ���� ���� �÷���
    private bool isFlipping = false;

    // Inspector���� �׽�Ʈ�� ������ �� ����.
    [ContextMenu("Test Flip Function")]
    private void TestFlipFunction()
    {
        Flip(flipDirection, flipSize);
    }

    // Flip ��� ���� Right ���� Flip�� �⺻ ����.
    private void Flip(FlipDirection direction = FlipDirection.Right, FlipSize size = FlipSize.One)
    {
        if (isFlipping)
        {
            return;
        }
        isFlipping = true;

        // ���� ȸ�� ���ʹϾ�
        Quaternion currentRotation = transform.rotation;

        Vector3 angle3 = new Vector3(0, 0, 0);

        // angleSize�� ���� DOTween�� DORotateQuaternion�� �������� �� ��Ÿ���� ����� ���� ������ ���̴�.
        float angleSize = 0;
        switch (size)
        {
            case FlipSize.One:
                angleSize = 180f;
                break;
            case FlipSize.Half:
                angleSize = -90f;
                break;
        }

        switch (direction)
        {
            case FlipDirection.Left:
                angle3.y = -angleSize;
                break;
            case FlipDirection.Right:
                angle3.y = angleSize;
                break;
            case FlipDirection.Top:
                angle3.x = -angleSize;
                break;
            case FlipDirection.Bottom:
                angle3.x = angleSize;
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
