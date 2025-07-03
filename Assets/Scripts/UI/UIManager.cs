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

    private List<VisualElement> list_section_text = new();
    private List<Label> list_label_BetAmount = new();
    private List<Label> list_label_PlayerName = new();
    private List<Label> list_label_PlayerChip = new();

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

    private void CreatePlayerInfo()
    {
        VisualElement section = new VisualElement();
        section.AddToClassList("section_text");
        section.visible = false;
        uiPlayerInfo.rootVisualElement.Add(section);
        list_section_text.Add(section);

        Label label_BetAmount = new Label();
        label_BetAmount.AddToClassList("label");
        section.Add(label_BetAmount);
        list_label_BetAmount.Add(label_BetAmount);

        Label label_PlayerName = new Label();
        label_PlayerName.AddToClassList("label");
        section.Add(label_PlayerName);
        list_label_PlayerName.Add(label_PlayerName);

        Label label_PlayerChip = new Label();
        label_PlayerChip.AddToClassList("label");
        section.Add(label_PlayerChip);
        list_label_PlayerChip.Add(label_PlayerChip);
    }

    public void PlayerInfoBetAmountSetText(string text, int index)
    {
        if (list_label_BetAmount.Count < index + 1)
        {
            return;
        }

        list_label_BetAmount[index].text = text;
    }

    public void PlayerInfoNameSetText(string text, int index)
    {
        if (list_label_PlayerName.Count < index + 1)
        {
            return;
        }

        list_label_PlayerName[index].text = text;
    }

    public void PlayerInfoChipSetText(string text, int index)
    {
        if (list_label_PlayerChip.Count < index + 1)
        {
            return;
        }

        list_label_PlayerChip[index].text = text;
    }

    public void PlayerInfoAllInvisible()
    {
        foreach (var section in list_section_text)
        {
            section.visible = false;
        }
    }

    public void PlayerInfoInvisible(int index)
    {
        if (list_section_text.Count < index + 1)
        {
            return;
        }

        list_section_text[index].visible = false;
    }

    public void PlayerInfoVisible(int index)
    {
        if (list_section_text.Count < index + 1)
        {
            for (int i = list_section_text.Count; i < index + 1; i++)
            {
                CreatePlayerInfo();
            }
        }

        list_section_text[index].visible = true;
    }

    private void UpdatePlayerInfoPosition(Vector3 objectPosition, int index)
    {
        if (list_section_text.Count < index + 1)
        {
            return;
        }

        float xPos = Camera.main.WorldToScreenPoint(objectPosition).x;

        float panelWidth = uiPlayerInfo.rootVisualElement.resolvedStyle.width;

        float widthRatio = panelWidth / Screen.width;

        float sectionWidthHalf = list_section_text[index].resolvedStyle.width / 2;

        list_section_text[index].style.left = xPos * widthRatio - sectionWidthHalf;
    }

    public void RequestPlayerInfoPositionUpdate(Vector3 objectPosition, int index)
    {
        list_section_text[index].schedule.Execute(() => UpdatePlayerInfoPosition(objectPosition, index)).ExecuteLater(0);
    }
}
