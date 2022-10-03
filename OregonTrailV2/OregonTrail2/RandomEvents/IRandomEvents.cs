namespace OregonTrail2.RandomEvents
{
    public interface IRandomEvents
    {
        GameState HandleRandomEvents(GameState oldState);
    }
}
