using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public UIDocument uiPanel;
    public VisualTreeAsset betPanel;
    public VisualTreeAsset playerActionPanel;

    [Header("Betting Buttons")]
    public Button button_Join;
    public Button button_BetReset;
    public Button button_Bet1;
    public Button button_Bet2;
    public Button button_Bet3;
    public Button button_Bet4;
    public Button button_BetMax;
    public Button button_BetConfirm;

    [Header("Player Action Buttons")]
    public Button button_Hit;
    public Button button_Stand;
    public Button button_Split;
    public Button button_DoubleDown;

    public void ChangeToBetPanel()
    {
        if (uiPanel.visualTreeAsset != betPanel)
        {
            uiPanel.visualTreeAsset = betPanel;
            OnChangeToBetPanel();
        }
    }

    public void ChangeToPlayerActionPanel()
    {
        if (uiPanel.visualTreeAsset != playerActionPanel)
        {
            uiPanel.visualTreeAsset = playerActionPanel;
            OnChangeToPlayerActionPanel();
        }
    }

    private void OnChangeToBetPanel()
    {
        VisualElement root = uiPanel.rootVisualElement;
        VisualElement section_BottomButtons = root.Q<VisualElement>("Section_BottomButtons");

        button_Join = root.Q<Button>("Button_Join");
        button_BetReset = section_BottomButtons.Q<Button>("Button_BetReset");
        button_Bet1 = section_BottomButtons.Q<Button>("Button_Bet1");
        button_Bet2 = section_BottomButtons.Q<Button>("Button_Bet2");
        button_Bet3 = section_BottomButtons.Q<Button>("Button_Bet3");
        button_Bet4 = section_BottomButtons.Q<Button>("Button_Bet4");
        button_BetMax = section_BottomButtons.Q<Button>("Button_BetMax");
        button_BetConfirm = section_BottomButtons.Q<Button>("Button_BetConfirm");
    }

    private void OnChangeToPlayerActionPanel()
    {
        VisualElement root = uiPanel.rootVisualElement;
        VisualElement section_Bottom = root.Q<VisualElement>("Section_Bottom");

        button_Hit = section_Bottom.Q<Button>("Button_Hit");
        button_Stand = section_Bottom.Q<Button>("Button_Stand");
        button_Split = section_Bottom.Q<Button>("Button_Split");
        button_DoubleDown = section_Bottom.Q<Button>("Button_DoubleDown");
    }
}
