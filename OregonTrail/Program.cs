using OregonTrail;
using OregonTrail.Display;
using OregonTrail.Turns;
using OregonTrail.UserInput;

var game = new Game(
        new TurnMaker(),
        new ConsoleTextDisplay(),
        new GetInputFromConsole()
);

game.StartGame();