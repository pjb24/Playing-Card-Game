using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerHand : Hand
{
    // ����Ʈ 17�� �� ��Ʈ ���� ����
    public bool DealerHitOnSoftCount => true;

    public int SoftCount = 17;

    // ������ ��Ʈ�ؾ��ϴ��� Ȯ��
    public bool ShouldHit()
    {
        int value = GetValue();

        if (value < SoftCount)
        {
            return true;
        }

        if (value == SoftCount && DealerHitOnSoftCount)
        {
            return IsSoftCount();
        }

        return false;
    }

    // �ڵ尡 Soft 17 ���� Ȯ��
    // Ace�� 11�� ����� 17�� �ǹ�
    public bool IsSoftCount()
    {
        int total = 0;
        int aceCount = 0;

        foreach (var card in Cards)
        {
            int value = card.GetValue();
            total += value;

            if (card.Rank == E_Rank.Ace)
            {
                aceCount++;
            }
        }

        return total == SoftCount && aceCount > 0;
    }
}
