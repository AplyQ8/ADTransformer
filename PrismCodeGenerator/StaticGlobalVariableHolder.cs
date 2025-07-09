namespace PrismCodeGenerator;

public static class StaticGlobalVariableHolder
{
    public const string TopEventVariable = "top_event";
    public const string TurnVariable = "turn";

    public const string AttackerModuleName = "Attacker";
    public const string DefenderModuleName = "Defender";
    public const string GameManagerModuleName = "GameManager";

    public const string GameManagerCheckEvent = "[check_gates]";
    
    public const string AttackerPlayerName = "attacker";
    public const string DefenderPlayerName = "defender";
    public const string GameManagerPlayerName = "gameManager";

    public const string AttackerRewardsName = "attacker_cost";
    public const string DefenderRewardsName = "defender_cost";

    public const string TopEventLabelName = "top_event_reached";
    public const string SystemSecureLabelName = "system_secure";
    public const string GameEndLabelName = "end_game";

    public const string InitialAttackerBudgetName = "INIT_ATTACKER_BUDGET";
    public const string InitialDefenderBudgetName = "INIT_DEFENDER_BUDGET";

    public const string AttackerEndTurnVariable = "attacker_done";
    public const string AttackerEndTurnEvent = "[attack_end_turn]";
    
    public const string DefenderEndTurnVariable = "defender_done";
    public const string DefenderEndTurnEvent = "[defense_end_turn]";
}