namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IBattleInitializer
    {
        IBattle StartBattle(BattleInitializationRequest request);
    }
}