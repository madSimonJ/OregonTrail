using OregonTrail.Common;
using OregonTrail.Display;
using OregonTrail.Entities;
using OregonTrail.Turns;
using OregonTrail.UserInput;

namespace OregonTrail
{
    public class Game
    {
        private readonly IMakeTheNextTurn _turnMaker;
        private readonly IDisplayText _textDisplay;
        private readonly IGetUserInput _userInputCapture;

        public Game(IMakeTheNextTurn turnMaker, IDisplayText textDisplay, IGetUserInput userInputCapture)
        {
            _turnMaker = turnMaker;
            _textDisplay = textDisplay;
            _userInputCapture = userInputCapture;
        }

        public void StartGame()
        {
            var firstState = new GameState();
            var firstTurn = this._turnMaker.MakeNextTurn(firstState, string.Empty);
            this._textDisplay.DisplayToUser(firstTurn.Text);

            firstTurn.IterateUntil(
                    x =>
                    {
                        var input = this._userInputCapture.GetInput();
                        var nextTurn = this._turnMaker.MakeNextTurn(x, input);
                        this._textDisplay.DisplayToUser(nextTurn.Text);
                        
                        return nextTurn;
                    },
                    x => x.IsGameFinished
                );
        }
    }
}
