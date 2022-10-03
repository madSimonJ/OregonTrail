namespace OregonTrail2.SelectAction
{
    public interface ISelectPlayerAction
    {
        GameState SelectAction(GameState oldGameState);
    }
}
