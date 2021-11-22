using OregonTrail.Common;
using OregonTrail.Display;
using OregonTrail.Entities;
using OregonTrail.Turns;

namespace OregonTrail
{
    public class Game
    {
        private readonly IMakeTheNextTurn _turnMaker;
        private readonly IDisplayText _textDisplay;

        public Game(IMakeTheNextTurn turnMaker, IDisplayText textDisplay)
        {
            _turnMaker = turnMaker;
            _textDisplay = textDisplay;
        }

        public void StartGame()
        {
            new GameState
            {
                IsGameFinished = false
            }.IterateUntil(
                    x =>
                    {
                        var nextTurn = this._turnMaker.MakeNextTurn(x);
                        this._textDisplay.DisplayToUser(nextTurn.Text);
                        return nextTurn;
                    },
                    x => x.IsGameFinished
                );
        }
    }
}
