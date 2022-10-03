using OregonTrail.Entities;

namespace OregonTrail.Turns
{
    public interface IHandleEndOfTurnEvents
    {
        GameState HandleEndOfTurn(GameState currentState, string userInput);
    }
}
