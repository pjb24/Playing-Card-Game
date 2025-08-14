using System.Collections;

public interface IWelcomeMessageHandler
{
    void Welcome(WelcomeDTO dto);
}

public interface IOnJoinLobbySuccessMessageHandler
{
    void OnJoinLobbySuccess(OnJoinLobbySuccessDTO dto);
}

public interface IOnFullExistRoomListMessageHandler
{
    void OnFullExistRoomList(OnFullExistRoomListDTO dto);
}

public interface IOnChangedRoomListDTOMessageHandler
{
    void OnChangedRoomList(OnChangedRoomListDTO dto);
}

public interface IOnRoomCreateSuccessMessageHandler
{
    void OnRoomCreateSuccess(OnRoomCreateSuccessDTO dto);
}

public interface IOnUserJoinedMessageHandler
{
    void OnUserJoined(OnUserJoinedDTO dto);
}

public interface IUserLeftMessageHandler
{
    void UserLeft(UserLeftDTO dto);
}

public interface IOnTimeToBettingMessageHandler
{
    void OnTimeToBetting(OnTimeToBettingDTO dto);
}

public interface IOnTimeToActionMessageHandler
{
    void OnTimeToAction(OnTimeToActionDTO dto);
}

public interface IOnPlayerRemainChipsMessageHandler
{
    void OnPlayerRemainChips(OnPlayerRemainChipsDTO dto);
}

public interface IOnPlayerBustedMessageHandler
{
    void OnPlayerBusted(OnPlayerBustedDTO dto);
}

public interface IOnPayoutMessageHandler
{
    void OnPayout(OnPayoutDTO dto);
}

public interface IOnJoinSuccessMessageHandler
{
    void OnJoinSuccess(OnJoinSuccessDTO dto);
}

public interface IOnHandSplitMessageHandler
{
    void OnHandSplit(OnHandSplitDTO dto);
}

public interface IOnGrantRoomMasterMessageHandler
{
    void OnGrantRoomMaster(OnGrantRoomMasterDTO dto);
}

public interface IOnGameEndMessageHandler
{
    void OnGameEnd(OnGameEndDTO dto);
}

public interface IOnExistingPlayerListMessageHandler
{
    void OnExistingPlayerList(OnExistingPlayerListDTO dto);
}

public interface IOnDealerHoleCardRevealedMessageHandler
{
    void OnDealerHoleCardRevealed(OnDealerHoleCardRevealedDTO dto);
}

public interface IOnDealerHiddenCardDealtMessageHandler
{
    void OnDealerHiddenCardDealt(OnDealerHiddenCardDealtDTO dto);
}

public interface IOnDealerCardDealtCompleteMessageHandler
{
    void OnDealerCardDealtComplete(OnDealerCardDealtCompleteDTO dto);
}

public interface IOnDealerCardDealtMessageHandler
{
    void OnDealerCardDealt(OnDealerCardDealtDTO dto);
}

public interface IOnCardDealtMessageHandler
{
    void OnCardDealt(OnCardDealtDTO dto);
}

public interface IOnBetPlacedMessageHandler
{
    void OnBetPlaced(OnBetPlacedDTO dto);
}

public interface IOnAddHandToPlayerMessageHandler
{
    void OnAddHandToPlayer(OnAddHandToPlayerDTO dto);
}

public interface IOnActionDoneMessageHandler
{
    void OnActionDone(OnActionDoneDTO dto);
}
