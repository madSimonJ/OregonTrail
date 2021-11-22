using OregonTrail;
using OregonTrail.Display;
using OregonTrail.Turns;

var game = new Game(
        new TurnMaker(),
        new ConsoleTextDisplay()
    );

game.StartGame();