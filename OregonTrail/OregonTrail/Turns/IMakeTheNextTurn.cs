using OregonTrail.Entities;

namespace OregonTrail.Turns
{
    public interface IMakeTheNextTurn
    {
        GameState MakeNextTurn(GameState state, string userInput);
    }
}
