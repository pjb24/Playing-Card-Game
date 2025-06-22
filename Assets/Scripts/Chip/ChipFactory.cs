using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipFactory : MonoBehaviour
{
    public List<GameObject> chipPrefabs;

    public Transform spawnPosition;

    public Transform bettingPositionSingle;
    public List<Transform> bettingPositionDouble;
    public List<Transform> bettingPositionTriple;
    public List<Transform> bettingPositionQuadruple;

    private List<ChipView> chipsType1 = new();
    private List<ChipView> chipsType2 = new();
    private List<ChipView> chipsType3 = new();
    private List<ChipView> chipsType4 = new();
    private List<ChipView> chipsType5 = new();

    // 각각의 타입에 맞는 칩을 생성한다.
    // 하위 타입의 칩들이 모여서 상위 타입의 칩의 가치를 넘기면
    // 하위 타입의 칩들을 제거하고 상위 타입의 칩을 1개 생성한다.

    public void CreateChipType1()
    {
        var chipObj = Instantiate(chipPrefabs[0], spawnPosition.position, chipPrefabs[0].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet1);

        chipsType1.Add(chipView);

        if (chipsType1.Count >= (int)E_ChipValue.Bet2 / (int)E_ChipValue.Bet1)
        {
            ResetChipType1();
            CreateChipType2();
        }
    }

    public void CreateChipType2()
    {
        var chipObj = Instantiate(chipPrefabs[1], spawnPosition.position, chipPrefabs[1].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet2);

        chipsType2.Add(chipView);

        if (chipsType2.Count >= (int)E_ChipValue.Bet3 / (int)E_ChipValue.Bet2)
        {
            ResetChipType2();
            CreateChipType3();
        }
    }

    public void CreateChipType3()
    {
        var chipObj = Instantiate(chipPrefabs[2], spawnPosition.position, chipPrefabs[2].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet3);

        chipsType3.Add(chipView);

        if (chipsType3.Count >= (int)E_ChipValue.Bet4 / (int)E_ChipValue.Bet3)
        {
            ResetChipType3();
            CreateChipType4();
        }
    }

    public void CreateChipType4()
    {
        var chipObj = Instantiate(chipPrefabs[3], spawnPosition.position, chipPrefabs[3].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.Bet4);

        chipsType4.Add(chipView);

        if (chipsType4.Count >= (int)E_ChipValue.BetMax / (int)E_ChipValue.Bet4)
        {
            ResetChipType4();
            CreateChipType5();
        }
    }

    public void CreateChipType5()
    {
        var chipObj = Instantiate(chipPrefabs[4], spawnPosition.position, chipPrefabs[4].transform.rotation);
        var chipView = chipObj.GetComponent<ChipView>();
        chipView.SetValue((int)E_ChipValue.BetMax);

        chipsType5.Add(chipView);
    }

    // 칩들의 위치를 조정한다.
    public void UpdateChipPosition()
    {
        int typeCount = 0;
        if (chipsType1.Count > 0)
        {
            typeCount++;
        }
        if (chipsType2.Count > 0)
        {
            typeCount++;
        }
        if (chipsType3.Count > 0)
        {
            typeCount++;
        }
        if (chipsType4.Count > 0)
        {
            typeCount++;
        }
        if (chipsType5.Count > 0)
        {
            typeCount++;
        }

        switch (typeCount)
        {
            case 1:
                {
                    if (chipsType1.Count > 0)
                    {
                        MoveChipsToBettingAreaSingle(chipsType1);
                    }
                    if (chipsType2.Count > 0)
                    {
                        MoveChipsToBettingAreaSingle(chipsType2);
                    }
                    if (chipsType3.Count > 0)
                    {
                        MoveChipsToBettingAreaSingle(chipsType3);
                    }
                    if (chipsType4.Count > 0)
                    {
                        MoveChipsToBettingAreaSingle(chipsType4);
                    }
                    if (chipsType5.Count > 0)
                    {
                        MoveChipsToBettingAreaSingle(chipsType5);
                    }
                }
                break;
            case 2:
                {
                    if (chipsType1.Count == 0 && chipsType2.Count == 0)
                    {
                        MoveChipsToBettingAreaDouble(chipsType3, chipsType4);
                    }
                    if (chipsType1.Count == 0 && chipsType3.Count == 0)
                    {
                        MoveChipsToBettingAreaDouble(chipsType2, chipsType4);
                    }
                    if (chipsType1.Count == 0 && chipsType4.Count == 0)
                    {
                        MoveChipsToBettingAreaDouble(chipsType2, chipsType3);
                    }

                    if (chipsType2.Count == 0 && chipsType3.Count == 0)
                    {
                        MoveChipsToBettingAreaDouble(chipsType1, chipsType4);
                    }
                    if (chipsType2.Count == 0 && chipsType4.Count == 0)
                    {
                        MoveChipsToBettingAreaDouble(chipsType1, chipsType3);
                    }

                    if (chipsType3.Count == 0 && chipsType4.Count == 0)
                    {
                        MoveChipsToBettingAreaDouble(chipsType1, chipsType2);
                    }
                }
                break;
            case 3:
                {
                    if (chipsType1.Count == 0)
                    {
                        MoveChipsToBettingAreaTriple(chipsType2, chipsType3, chipsType4);
                    }
                    if (chipsType2.Count == 0)
                    {
                        MoveChipsToBettingAreaTriple(chipsType1, chipsType3, chipsType4);
                    }
                    if (chipsType3.Count == 0)
                    {
                        MoveChipsToBettingAreaTriple(chipsType1, chipsType2, chipsType4);
                    }
                    if (chipsType4.Count == 0)
                    {
                        MoveChipsToBettingAreaTriple(chipsType1, chipsType2, chipsType3);
                    }
                }
                break;
            case 4:
                {
                    MoveChipsToBettingAreaQuadruple(chipsType1, chipsType2, chipsType3, chipsType4);
                }
                break;
        }
    }

    // 칩의 종류의 개수에 따라 위치를 조정한다.
    public void MoveChipsToBettingAreaSingle(List<ChipView> chips)
    {
        int count = 0;
        foreach (ChipView chip in chips)
        {
            chip.transform.DOMove(bettingPositionSingle.position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }
    }

    public void MoveChipsToBettingAreaDouble(List<ChipView> chips1, List<ChipView> chips2)
    {
        int count = 0;
        foreach (ChipView chip in chips1)
        {
            chip.transform.DOMove(bettingPositionDouble[0].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }

        count = 0;
        foreach (ChipView chip in chips2)
        {
            chip.transform.DOMove(bettingPositionDouble[1].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }
    }

    public void MoveChipsToBettingAreaTriple(List<ChipView> chips1, List<ChipView> chips2, List<ChipView> chips3)
    {
        int count = 0;
        foreach (ChipView chip in chips1)
        {
            chip.transform.DOMove(bettingPositionTriple[0].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }

        count = 0;
        foreach (ChipView chip in chips2)
        {
            chip.transform.DOMove(bettingPositionTriple[1].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }

        count = 0;
        foreach (ChipView chip in chips3)
        {
            chip.transform.DOMove(bettingPositionTriple[2].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }
    }

    public void MoveChipsToBettingAreaQuadruple(List<ChipView> chips1, List<ChipView> chips2, List<ChipView> chips3, List<ChipView> chips4)
    {
        int count = 0;
        foreach (ChipView chip in chips1)
        {
            chip.transform.DOMove(bettingPositionQuadruple[0].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }

        count = 0;
        foreach (ChipView chip in chips2)
        {
            chip.transform.DOMove(bettingPositionQuadruple[1].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }

        count = 0;
        foreach (ChipView chip in chips3)
        {
            chip.transform.DOMove(bettingPositionQuadruple[2].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }

        count = 0;
        foreach (ChipView chip in chips4)
        {
            chip.transform.DOMove(bettingPositionQuadruple[3].position + new Vector3(0, 0.1f * count, 0), 1f);
            count++;
        }
    }

    // 칩들을 제거한다.
    public void ResetChips()
    {
        ResetChipType1();
        ResetChipType2();
        ResetChipType3();
        ResetChipType4();
        ResetChipType5();
    }

    public void ResetChipType1()
    {
        foreach (var chipView in chipsType1)
        {
            Destroy(chipView.gameObject);
        }
        chipsType1.Clear();
    }

    public void ResetChipType2()
    {
        foreach (var chipView in chipsType2)
        {
            Destroy(chipView.gameObject);
        }
        chipsType2.Clear();
    }

    public void ResetChipType3()
    {
        foreach (var chipView in chipsType3)
        {
            Destroy(chipView.gameObject);
        }
        chipsType3.Clear();
    }

    public void ResetChipType4()
    {
        foreach (var chipView in chipsType4)
        {
            Destroy(chipView.gameObject);
        }
        chipsType4.Clear();
    }

    public void ResetChipType5()
    {
        foreach (var chipView in chipsType5)
        {
            Destroy(chipView.gameObject);
        }
        chipsType5.Clear();
    }
}
