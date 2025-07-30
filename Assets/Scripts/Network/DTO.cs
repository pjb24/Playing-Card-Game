public class DTO_Welcome
{
    public string message { get; set; }
}

public class DTO_UserConnected
{
    public string message { get; set; }
}

public class DTO_UserDisconnected
{
    public string message { get; set; }
}

public class DTO_OnError
{
    public string message { get; set; }
}

public class DTO_OnJoinSuccess
{
    public string userName { get; set; }
    public string playerGuid { get; set; }
}

public class DTO_OnUserJoined
{
    public string userName { get; set; }
}

public class DTO_OnPlayerRemainChips
{
    public string chips { get; set; }
}

public class DTO_OnGameStateChanged
{
    public string state { get; set; }
}

public class DTO_OnBetPlaced
{
    public string playerName { get; set; }
    public int betAmount { get; set; }
    public string handId { get; set; }
}

public class DTO_UserLeft
{
    public string connectionId { get; set; }
}

public class DTO_OnTimeToBetting
{
    public string handId { get; set; }
}

public class DTO_OnPayout
{
    public string handId { get; set; }
    public string evaluationResult { get; set; }
}

public class DTO_OnCardDealt
{
    public string playerGuid { get; set; }
    public string playerName { get; set; }
    public string cardString { get; set; }
    public string handId { get; set; }
}

public class DTO_OnPlayerBusted
{
    public string playerGuid { get; set; }
    public string playerName { get; set; }
    public string handId { get; set; }
}

public class DTO_OnActionDone
{
    public string playerGuid { get; set; }
    public string playerName { get; set; }
    public string handId { get; set; }
}

public class DTO_OnHandSplit
{
    public string playerName { get; set; }
    public string handId { get; set; }
    public string newHandId { get; set; }
}

public class DTO_OnDealerHoleCardRevealed
{
    public string cardString { get; set; }
}

public class DTO_OnDealerCardDealt
{
    public string cardString { get; set; }
}

public class DTO_OnDealerHiddenCardDealt
{
}

public class DTO_OnTimeToAction
{
    public string handId { get; set; }
    public string playerGuid { get; set; }
    public string playerName { get; set; }
}

public class DTO_OnHandEvaluation
{
    public string playerGuid { get; set; }
    public string playerName { get; set; }
    public string handId { get; set; }
    public string evaluationResult { get; set; }
}