using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipFactory : MonoBehaviour
{
    public List<GameObject> chipPrefabs;

    public Transform spawnPosition;

    [SerializeField] private Transform bettingPositionRoot;
    [SerializeField] private float bettingPositionSpacing = 0.5f;

    [SerializeField] private float chipOffsetY = 0.1f;

    // 각각의 타입에 맞는 칩을 생성한다.
    // 하위 타입의 칩들이 모여서 상위 타입의 칩의 가치를 넘기면
    // 하위 타입의 칩들을 제거하고 상위 타입의 칩을 1개 생성한다.

    public void CreateChipType1(PlayerHand hand)
    {
        var chipObj = Instantiate(chipPrefabs[0], spawnPosition.position, chipPrefabs[0].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet1);

        hand.AddChip(chipView);

        if (hand.GetChipCount(chipView) >= (int)E_ChipValue.Bet2 / (int)E_ChipValue.Bet1)
        {
            ResetChipType1(hand);
            CreateChipType2(hand);
        }
    }

    public void CreateChipType2(PlayerHand hand)
    {
        var chipObj = Instantiate(chipPrefabs[1], spawnPosition.position, chipPrefabs[1].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet2);

        hand.AddChip(chipView);

        if (hand.GetChipCount(chipView) >= (int)E_ChipValue.Bet3 / (int)E_ChipValue.Bet2)
        {
            ResetChipType2(hand);
            CreateChipType3(hand);
        }
    }

    public void CreateChipType3(PlayerHand hand)
    {
        var chipObj = Instantiate(chipPrefabs[2], spawnPosition.position, chipPrefabs[2].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet3);

        hand.AddChip(chipView);

        if (hand.GetChipCount(chipView) >= (int)E_ChipValue.Bet4 / (int)E_ChipValue.Bet3)
        {
            ResetChipType3(hand);
            CreateChipType4(hand);
        }
    }

    public void CreateChipType4(PlayerHand hand)
    {
        var chipObj = Instantiate(chipPrefabs[3], spawnPosition.position, chipPrefabs[3].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet4);

        hand.AddChip(chipView);

        if (hand.GetChipCount(chipView) >= (int)E_ChipValue.BetMax / (int)E_ChipValue.Bet4)
        {
            ResetChipType4(hand);
            CreateChipType5(hand);
        }
    }

    public void CreateChipType5(PlayerHand hand)
    {
        var chipObj = Instantiate(chipPrefabs[4], spawnPosition.position, chipPrefabs[4].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.BetMax);

        hand.AddChip(chipView);
    }

    // 칩들의 위치를 조정한다.
    public void UpdateHandChipPosition(PlayerHand hand)
    {
        int typeCount = hand.GetChipTypeCount();

        Vector3 handPosition = GameManager.Instance.GetHandPosition(hand);

        List<E_ChipType> tempListChipType = new();

        foreach (var chip in hand.ListChips)
        {
            int index = hand.GetChipTypeIndex(chip);

            Vector3 destination;
            destination.x = GetChipPosition(index, typeCount, bettingPositionSpacing);
            destination.y = bettingPositionRoot.position.y;
            destination.z = bettingPositionRoot.position.z;

            destination.x += handPosition.x;

            int chipTypeCount = 0;
            foreach (var chipType in tempListChipType)
            {
                if (chipType == chip.chipType)
                {
                    chipTypeCount++;
                }
            }

            tempListChipType.Add(chip.chipType);

            destination.y += chipTypeCount * chipOffsetY;

            MoveChip(destination, chip);
        }

        tempListChipType.Clear();
    }

    // 칩들을 제거한다.
    public void ResetChips(PlayerHand hand)
    {
        ResetChipType1(hand);
        ResetChipType2(hand);
        ResetChipType3(hand);
        ResetChipType4(hand);
        ResetChipType5(hand);
    }

    public void ResetChipType1(PlayerHand hand)
    {
        hand.ResetChip(E_ChipType.Green);
    }

    public void ResetChipType2(PlayerHand hand)
    {
        hand.ResetChip(E_ChipType.Red);
    }

    public void ResetChipType3(PlayerHand hand)
    {
        hand.ResetChip(E_ChipType.Blue);
    }

    public void ResetChipType4(PlayerHand hand)
    {
        hand.ResetChip(E_ChipType.Purple);
    }

    public void ResetChipType5(PlayerHand hand)
    {
        hand.ResetChip(E_ChipType.Black);
    }

    public int CreateChipsToFitValue(int value, PlayerHand hand)
    {
        int tempChips = value;
        int countType5 = tempChips / (int)E_ChipValue.BetMax;
        tempChips -= countType5 * (int)E_ChipValue.BetMax;
        int countType4 = tempChips / (int)E_ChipValue.Bet4;
        tempChips -= countType4 * (int)E_ChipValue.Bet4;
        int countType3 = tempChips / (int)E_ChipValue.Bet3;
        tempChips -= countType3 * (int)E_ChipValue.Bet3;
        int countType2 = tempChips / (int)E_ChipValue.Bet2;
        tempChips -= countType2 * (int)E_ChipValue.Bet2;
        int countType1 = tempChips / (int)E_ChipValue.Bet1;

        for (int i = 0; i < countType1; i++)
        {
            CreateChipType1(hand);
        }
        for (int i = 0; i < countType2; i++)
        {
            CreateChipType2(hand);
        }
        for (int i = 0; i < countType3; i++)
        {
            CreateChipType3(hand);
        }
        for (int i = 0; i < countType4; i++)
        {
            CreateChipType4(hand);
        }
        for (int i = 0; i < countType5; i++)
        {
            CreateChipType5(hand);
        }

        int betAmount = countType5 * (int)E_ChipValue.BetMax
                + countType4 * (int)E_ChipValue.Bet4
                + countType3 * (int)E_ChipValue.Bet3
                + countType2 * (int)E_ChipValue.Bet2
                + countType1 * (int)E_ChipValue.Bet1;

        return betAmount;
    }

    private void MoveChip(Vector3 chipsDestination, ChipView moveTargetChip)
    {
        moveTargetChip.transform.DOMove(chipsDestination, 1f);
    }

    private float GetChipPosition(int index, int totalChipTypes, float spacing = 0.5f)
    {
        float centerOffset = (totalChipTypes - 1) * spacing * 0.5f;
        return index * spacing - centerOffset;
    }

    public void UpdateAllChipsPosition()
    {
        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateHandChipPosition(hand);
            }
        }
    }
}
