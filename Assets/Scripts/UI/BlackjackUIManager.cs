using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackjackUIManager : MonoBehaviour
{
    [SerializeField] private FOVManager fovManager;

    [Header("Panels")]
    [SerializeField] private UIDocument uiPanel;
    [SerializeField] private VisualTreeAsset betPanel;
    [SerializeField] private VisualTreeAsset playerActionPanel;

    [Header("Betting Buttons")]
    private Button button_Join;
    private Button button_BetReset;
    private Button button_Bet1;
    private Button button_Bet2;
    private Button button_Bet3;
    private Button button_Bet4;
    private Button button_BetMax;
    private Button button_BetConfirm;

    [Header("Player Action Buttons")]
    private Button button_Hit;
    private Button button_Stand;
    private Button button_Split;
    private Button button_DoubleDown;

    [Header("Player Infos")]
    [SerializeField] private UIDocument uiPlayerInfo;

    private List<VisualElement> list_section_text = new();
    private List<Label> list_label_BetAmount = new();
    private List<Label> list_label_PlayerName = new();
    private List<Label> list_label_PlayerChip = new();

    [Header("Card Values")]
    [SerializeField] private UIDocument uiCardValue;
    private Label label_CardValue_Dealer;
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

    private void UpdateCardValueUIPosition_Y(int index)
    {
        float referenceResolution_Y = uiPlayerInfo.panelSettings.referenceResolution.y;
        float panelHeight = uiCardValue.rootVisualElement.resolvedStyle.height;
        float scaledPanelY = panelHeight * fovManager.ScaledY;

        float heightRatio;
        if (fovManager.ScaleHeight <= 1.0f)
        {
            heightRatio = 1.0f;
        }
        else
        {
            heightRatio = panelHeight / referenceResolution_Y;
        }
        float yPos = list_label_CardValue_Player[index].resolvedStyle.top;
        list_label_CardValue_Player[index].style.top = yPos * heightRatio + scaledPanelY;
    }

    public void CreateLabelCardValuePlayer(int index)
    {
        Label label = new();
        label.AddToClassList("label_CardValue_Player");
        label.visible = false;
        uiCardValue.rootVisualElement.Add(label);

        list_label_CardValue_Player.Insert(index, label);
    }

    public void RemoveAllLabelCardValue()
    {
        foreach (var label in list_label_CardValue_Player)
        {
            uiCardValue.rootVisualElement.Remove(label);
        }

        list_label_CardValue_Player.Clear();
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
            CreateLabelCardValuePlayer(index);
        }

        list_label_CardValue_Player[index].visible = true;
    }

    public void RequestCardValueUIPositionUpdate(Vector3 objectPosition, int index)
    {
        list_label_CardValue_Player[index].schedule.Execute(() => UpdateCardValueUIPosition(objectPosition, index)).ExecuteLater(0);
    }

    public void RequestCardValueUIPositionUpdate_Y(int index)
    {
        list_label_CardValue_Player[index].schedule.Execute(() => UpdateCardValueUIPosition_Y(index)).ExecuteLater(0);
    }

    public void RequestCardValueUIPositionUpdate_Register(Vector3 objectPosition, int index)
    {
        list_label_CardValue_Player[index].RegisterCallbackOnce<GeometryChangedEvent>((evt) => UpdateCardValueUIPosition(objectPosition, index));
    }

    public void RequestCardValueUIPositionUpdate_Y_Register(int index)
    {
        list_label_CardValue_Player[index].RegisterCallbackOnce<GeometryChangedEvent>((evt) => UpdateCardValueUIPosition_Y(index));
    }

    public void CreatePlayerInfo(int index)
    {
        VisualElement section = new();
        section.AddToClassList("section_text");
        section.visible = false;
        uiPlayerInfo.rootVisualElement.Add(section);
        list_section_text.Insert(index, section);

        Label label_BetAmount = new();
        label_BetAmount.AddToClassList("label");
        section.Add(label_BetAmount);
        list_label_BetAmount.Insert(index, label_BetAmount);

        Label label_PlayerName = new();
        label_PlayerName.AddToClassList("label");
        section.Add(label_PlayerName);
        list_label_PlayerName.Insert(index, label_PlayerName);

        Label label_PlayerChip = new();
        label_PlayerChip.AddToClassList("label");
        section.Add(label_PlayerChip);
        list_label_PlayerChip.Insert(index, label_PlayerChip);
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

    public void RemoveAllPlayerInfos()
    {
        foreach (var section in list_section_text)
        {
            uiPlayerInfo.rootVisualElement.Remove(section);
        }

        list_label_BetAmount.Clear();
        list_label_PlayerName.Clear();
        list_label_PlayerChip.Clear();

        list_section_text.Clear();
    }

    public void PlayerInfoVisible(int index)
    {
        if (list_section_text.Count < index + 1)
        {
            CreatePlayerInfo(index);
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

        float left = xPos * widthRatio - sectionWidthHalf;

        list_section_text[index].style.left = left;
    }

    private void UpdatePlayerInfoPosition_Y(int index)
    {
        float referenceResolution_Y = uiPlayerInfo.panelSettings.referenceResolution.y;
        float panelHeight = uiPlayerInfo.rootVisualElement.resolvedStyle.height;
        float scaledPanelY = panelHeight * fovManager.ScaledY;

        float heightRatio;
        if (fovManager.ScaleHeight <= 1.0f)
        {
            heightRatio = 1.0f;
        }
        else
        {
            heightRatio = panelHeight / referenceResolution_Y;
        }
        float yPos = list_section_text[index].resolvedStyle.top;
        float top = yPos * heightRatio + scaledPanelY;
        list_section_text[index].style.top = top;
    }

    public void RequestPlayerInfoPositionUpdate(Vector3 objectPosition, int index)
    {
        list_section_text[index].schedule.Execute(() => UpdatePlayerInfoPosition(objectPosition, index)).ExecuteLater(0);
    }

    public void RequestPlayerInfoPositionUpdate_Y(int index)
    {
        list_section_text[index].schedule.Execute(() => UpdatePlayerInfoPosition_Y(index)).ExecuteLater(0);
    }

    public void RequestPlayerInfoPositionUpdate_Register(Vector3 objectPosition, int index)
    {
        list_section_text[index].RegisterCallbackOnce<GeometryChangedEvent>((evt) => UpdatePlayerInfoPosition(objectPosition, index));
    }

    public void RequestPlayerInfoPositionUpdate_Y_Register(int index)
    {
        list_section_text[index].RegisterCallbackOnce<GeometryChangedEvent>((evt) => UpdatePlayerInfoPosition_Y(index));
    }

    public void RemoveDealerCardValue()
    {
        if (label_CardValue_Dealer == null)
        {
            return;
        }

        uiCardValue.rootVisualElement.Remove(label_CardValue_Dealer);

        label_CardValue_Dealer = null;
    }

    public void CreateLabelCardValueDealer()
    {
        label_CardValue_Dealer = new();
        label_CardValue_Dealer.AddToClassList("label_CardValue_Dealer");
        label_CardValue_Dealer.visible = false;
        uiCardValue.rootVisualElement.Add(label_CardValue_Dealer);
    }

    private void UpdateCardValueDealerPosition()
    {
        label_CardValue_Dealer.visible = true;

        float referenceResolution_Y = uiPlayerInfo.panelSettings.referenceResolution.y;
        float panelHeight = uiCardValue.rootVisualElement.resolvedStyle.height;
        float scaledPanelY = panelHeight * fovManager.ScaledY;

        float heightRatio;
        if (fovManager.ScaleHeight <= 1.0f)
        {
            heightRatio = 1.0f;
        }
        else
        {
            heightRatio = panelHeight / referenceResolution_Y;
        }
        float yPos = label_CardValue_Dealer.resolvedStyle.top;
        label_CardValue_Dealer.style.top = yPos * heightRatio + scaledPanelY;
    }

    public void RequestUpdateCardValueDealerPosition()
    {
        label_CardValue_Dealer.RegisterCallbackOnce<GeometryChangedEvent>((evt) => UpdateCardValueDealerPosition());
    }

    public void CardValueDealerSetText(string text)
    {
        if (label_CardValue_Dealer == null)
        {
            return;
        }

        label_CardValue_Dealer.text = text;
    }

    public void SetButtonJoinInvisible()
    {
        button_Join.visible = false;
    }

    public void SetButtonJoinVisible()
    {
        button_Join.visible = true;
    }

    public void SubscribeButtonJoinClicked(Action action)
    {
        button_Join.clicked += action;
    }

    public void UnsubscribeButtonJoinClicked(Action action)
    {
        button_Join.clicked += action;
    }

    public void SubscribeButtonBetResetClicked(Action action)
    {
        button_BetReset.clicked += action;
    }

    public void UnsubscribeButtonBetResetClicked(Action action)
    {
        button_BetReset.clicked += action;
    }

    public void SubscribeButtonBet1Clicked(Action action)
    {
        button_Bet1.clicked += action;
    }

    public void UnsubscribeButtonBet1Clicked(Action action)
    {
        button_Bet1.clicked += action;
    }

    public void SubscribeButtonBet2Clicked(Action action)
    {
        button_Bet2.clicked += action;
    }

    public void UnsubscribeButtonBet2Clicked(Action action)
    {
        button_Bet2.clicked += action;
    }

    public void SubscribeButtonBet3Clicked(Action action)
    {
        button_Bet3.clicked += action;
    }

    public void UnsubscribeButtonBet3Clicked(Action action)
    {
        button_Bet3.clicked += action;
    }

    public void SubscribeButtonBet4Clicked(Action action)
    {
        button_Bet4.clicked += action;
    }

    public void UnsubscribeButtonBet4Clicked(Action action)
    {
        button_Bet4.clicked += action;
    }

    public void SubscribeButtonBetMaxClicked(Action action)
    {
        button_BetMax.clicked += action;
    }

    public void UnsubscribeButtonBetMaxClicked(Action action)
    {
        button_BetMax.clicked += action;
    }

    public void SubscribeButtonBetConfirmClicked(Action action)
    {
        button_BetConfirm.clicked += action;
    }

    public void UnsubscribeButtonBetConfirmClicked(Action action)
    {
        button_BetConfirm.clicked += action;
    }

    public void SubscribeButtonHitClicked(Action action)
    {
        button_Hit.clicked += action;
    }

    public void UnsubscribeButtonHitClicked(Action action)
    {
        button_Hit.clicked += action;
    }

    public void SubscribeButtonStandClicked(Action action)
    {
        button_Stand.clicked += action;
    }

    public void UnsubscribeButtonStandClicked(Action action)
    {
        button_Stand.clicked += action;
    }

    public void SubscribeButtonSplitClicked(Action action)
    {
        button_Split.clicked += action;
    }

    public void UnsubscribeButtonSplitClicked(Action action)
    {
        button_Split.clicked += action;
    }

    public void SubscribeButtonDoubleDownClicked(Action action)
    {
        button_DoubleDown.clicked += action;
    }

    public void UnsubscribeButtonDoubleDownClicked(Action action)
    {
        button_DoubleDown.clicked += action;
    }
}
