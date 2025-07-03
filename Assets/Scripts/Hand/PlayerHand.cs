using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : Hand
{
    public int BetAmount { get; private set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsBetConfirmed { get; set; } = false;

    private List<ChipView> listChips = new();
    public IReadOnlyList<ChipView> ListChips => listChips;

    public void Bet(int amount)
    {
        BetAmount = amount;
    }

    // 카드가 2장이며 같은 숫자 또는 문자여야함
    public bool CanSplit()
    {
        if (Cards.Count != 2)
        {
            return false;
        }

        if (Cards[0].Rank != Cards[1].Rank)
        {
            return false;
        }

        return true;
    }

    public bool CanDoubleDown()
    {
        if (Cards.Count != 2)
        {
            return false;
        }

        return true;
    }

    public void AddChip(ChipView chip)
    {
        listChips.Add(chip);
    }

    public void ResetChipAll()
    {
        foreach (var chip in listChips)
        {
            GameObject.Destroy(chip.gameObject);
        }
        listChips.Clear();
    }

    public void ResetChip(E_ChipType chipType)
    {
        List<ChipView> listToRemove = new();

        foreach (var chip in listChips)
        {
            if (chip.chipType == chipType)
            {
                GameObject.Destroy(chip.gameObject);
                listToRemove.Add(chip);
            }
        }
        
        foreach (var item in listToRemove)
        {
            listChips.Remove(item);
        }
        listToRemove.Clear();
    }

    public int GetChipCount(E_ChipType chipType)
    {
        int count = 0;

        foreach (var chip in listChips)
        {
            if (chip.chipType == chipType)
            {
                count++;
            }
        }

        return count;
    }

    public int GetChipCount(ChipView targetChip)
    {
        int count = 0;

        foreach (var chip in listChips)
        {
            if (chip.chipType == targetChip.chipType)
            {
                count++;
            }
        }

        return count;
    }

    public int GetChipTypeCount()
    {
        HashSet<E_ChipType> setChipType = new();

        foreach (var chip in listChips)
        {
            setChipType.Add(chip.chipType);
        }

        int count = setChipType.Count;

        setChipType.Clear();

        return count;
    }

    public int GetChipTypeIndex(ChipView targetChip)
    {
        int index = 0;

        SortedSet<E_ChipType> setChipType = new();

        foreach (var chip in listChips)
        {
            setChipType.Add(chip.chipType);
        }

        foreach (var type in setChipType)
        {
            if (type == targetChip.chipType)
            {
                break;
            }
            else
            {
                index++;
            }
        }

        setChipType.Clear();

        return index;
    }
}
