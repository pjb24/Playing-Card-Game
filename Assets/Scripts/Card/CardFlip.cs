using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    // Flip 방향 enum
    public enum FlipDirection
    {
        Left,
        Right,
        Top,
        Bottom,
    };

    // Flip 크기 enum
    public enum FlipSize
    {
        One,
        Half
    };

    // Flip을 할 방향 지정
    public FlipDirection flipDirection = FlipDirection.Left;
    // Flip에 소요될 시간
    public float duration = 1f;
    // Flip을 할 크기
    public FlipSize flipSize = FlipSize.One;

    // Flip 중복 동작 방지 플래그
    private bool isFlipping = false;

    // Inspector에서 테스트를 수행할 수 있음.
    [ContextMenu("Test Flip Function")]
    private void TestFlipFunction()
    {
        Flip(flipDirection, flipSize);
    }

    // Flip 기능 구현 Right 방향 Flip이 기본 설정.
    private void Flip(FlipDirection direction = FlipDirection.Right, FlipSize size = FlipSize.One)
    {
        if (isFlipping)
        {
            return;
        }
        isFlipping = true;

        // 현재 회전 쿼터니언
        Quaternion currentRotation = transform.rotation;

        Vector3 angle3 = new Vector3(0, 0, 0);

        // angleSize의 값은 DOTween의 DORotateQuaternion을 수행했을 때 나타나는 결과를 보고 설정한 값이다.
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

        // 추가 회전 쿼터니언
        Quaternion rotationDelta = Quaternion.Euler(angle3);

        // 목표 회전 = 현재 회전 * 추가 회전
        Quaternion targetRotation = currentRotation * rotationDelta;

        // DOTween으로 duration 동안 회전 수행.
        transform.DORotateQuaternion(targetRotation, duration)
            .SetEase(Ease.InOutQuad)
            // 회전이 끝나면 플래그 해제.
            .OnComplete(() => { isFlipping = false; });
    }
}
