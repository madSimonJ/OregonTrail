using OregonTrail.Entities;

namespace OregonTrail.Turns
{
    public class TurnMaker : IMakeTheNextTurn
    {
        public GameState MakeNextTurn(GameState state)
        {
            return new GameState
            {
                IsGameFinished = true,
                Text = new []
                {
                    "DO YOU NEED INSTRUCTIONS  (YES/NO)"
                },
                TurnNumber = 1
            };
        }
    }
}
