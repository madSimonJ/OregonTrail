namespace OregonTrail2.StartTurn
{
    public interface IStartTheTurn
    {
        GameState StartNewTurn(GameState oldGameState);
    }
}
