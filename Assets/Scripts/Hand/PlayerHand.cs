using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : Hand
{
    private int _betAmount = 0;
    public int BetAmount => _betAmount;
    
    private bool _isCompleted = false;
    public bool IsCompleted => _isCompleted;

    private bool _isBetConfirmed = false;
    public bool IsBetConfirmed => _isBetConfirmed;

    private List<ChipView> _listChips = new();
    public IReadOnlyList<ChipView> ListChips => _listChips;

    private string _id;
    public string Id => _id;

    public void SetHandId(string id)
    {
        _id = id;
    }

    public void Bet(int amount)
    {
        _betAmount = amount;
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
        _listChips.Add(chip);
    }

    public void ResetChipAll()
    {
        foreach (var chip in _listChips)
        {
            GameObject.Destroy(chip.gameObject);
        }
        _listChips.Clear();
    }

    public void ResetChip(E_ChipType chipType)
    {
        List<ChipView> listToRemove = new();

        foreach (var chip in _listChips)
        {
            if (chip.chipType == chipType)
            {
                GameObject.Destroy(chip.gameObject);
                listToRemove.Add(chip);
            }
        }
        
        foreach (var item in listToRemove)
        {
            _listChips.Remove(item);
        }
        listToRemove.Clear();
    }

    public int GetChipCount(E_ChipType chipType)
    {
        int count = 0;

        foreach (var chip in _listChips)
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

        foreach (var chip in _listChips)
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

        foreach (var chip in _listChips)
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

        foreach (var chip in _listChips)
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

    public void SetIsBetConfirmed()
    {
        _isBetConfirmed = true;
    }

    public void ResetIsBetConfirmed()
    {
        _isBetConfirmed = false;
    }

    public void SetIsCompleted()
    {
        _isCompleted = true;
    }

    public void ResetIsCompleted()
    {
        _isCompleted = false;
    }
}
