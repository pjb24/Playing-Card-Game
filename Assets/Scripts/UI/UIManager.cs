using System;
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

    [Header("Player Infos")]
    public UIDocument uiPlayerInfo;
    public Label label_BetAmount;
    public Label label_PlayerName;
    public Label label_PlayerChip;

    [Header("Card Values")]
    public UIDocument uiCardValue;
    public Label label_CardValue_Dealer;
    private List<Label> list_label_CardValue_Player = new();

    public void ChangeToBetPanel()
    {
        uiPanel.visualTreeAsset = betPanel;
        OnChangeToBetPanel();
    }

    public void ChangeToPlayerActionPanel()
    {
        uiPanel.visualTreeAsset = playerActionPanel;
        OnChangeToPlayerActionPanel();
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

    public void SetPlayerInfos()
    {
        VisualElement root = uiPlayerInfo.rootVisualElement;
        VisualElement section_Labels = root.Q<VisualElement>("Section_Labels");

        label_BetAmount = section_Labels.Q<Label>("Label_BetAmount");
        label_PlayerName = section_Labels.Q<Label>("Label_PlayerName");
        label_PlayerChip = section_Labels.Q<Label>("Label_PlayerChip");
    }

    public void SetCardValues()
    {
        VisualElement root = uiCardValue.rootVisualElement;

        label_CardValue_Dealer = root.Q<Label>("Label_CardValue_Dealer");
    }

    private void UpdateCardValueUIPosition(Vector3 objectPosition, int index)
    {
        if (list_label_CardValue_Player.Count < index + 1)
        {
            return;
        }

        float xPos = Camera.main.WorldToScreenPoint(objectPosition).x;

        float panelWidth = uiCardValue.rootVisualElement.resolvedStyle.width;

        float widthRatio = panelWidth / Screen.width;

        float labelWidthHalf = list_label_CardValue_Player[index].resolvedStyle.width / 2;

        list_label_CardValue_Player[index].style.left = xPos * widthRatio - labelWidthHalf;
    }

    private void CreateLabelCardValuePlayer()
    {
        Label label = new Label();
        label.AddToClassList("label_CardValue_Player");
        label.visible = false;
        uiCardValue.rootVisualElement.Add(label);

        list_label_CardValue_Player.Add(label);
    }

    public void CardValuePlayerAllInvisible()
    {
        foreach (var label in list_label_CardValue_Player)
        {
            label.visible = false;
        }
    }

    public void CardValuePlayerSetText(string text, int index)
    {
        if (list_label_CardValue_Player.Count < index + 1)
        {
            return;
        }

        list_label_CardValue_Player[index].text = text;
    }

    public void CardValuePlayerVisible(int index)
    {
        if (list_label_CardValue_Player.Count < index + 1)
        {
            for (int i = list_label_CardValue_Player.Count; i < index + 1; i++)
            {
                CreateLabelCardValuePlayer();
            }
        }

        list_label_CardValue_Player[index].visible = true;
    }

    public void RequestCardValueUIPositionUpdate(Vector3 objectPosition, int index)
    {
        list_label_CardValue_Player[index].schedule.Execute(() => UpdateCardValueUIPosition(objectPosition, index)).ExecuteLater(0);
    }
}
