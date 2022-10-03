using OregonTrail;
using OregonTrail.Display;
using OregonTrail.Random;
using OregonTrail.TimeService;
using OregonTrail.Turns;
using OregonTrail.UserInput;

var game = new Game(
        new TurnMaker(new TimeService(), new RandomNumberGenerator(), new EndOfTurnEventHandler()),
        new ConsoleTextDisplay(),
        new GetInputFromConsole()
);

game.StartGame();