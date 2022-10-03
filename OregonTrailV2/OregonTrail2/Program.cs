
using OregonTrail2;
using OregonTrail2.Instruction;
using OregonTrail2.Inventory;
using OregonTrail2.StartTurn;
using OregonTrail2.PlayerInteraction;
using OregonTrail2.Eating;
using OregonTrail2.SelectAction;
using OregonTrail2.Shooting;
using OregonTrail2.Time;
using OregonTrail2.Riders;
using OregonTrail2.RandomEvents;
using OregonTrail2.Illness;
using OregonTrail2.Mountains;
using OregonTrail2.EndGame;

var consoleShim = new ConsoleShim();
var playerInteraction = new PlayerInteractionClient(consoleShim);
var timeService = new DateTimeShim();
var gun = new GunShotClient(playerInteraction, timeService);
var rnd = new RandomNumberGenerator();
var determineIllness = new IllnessDetermineClient(playerInteraction, rnd);

var game = new Game(
        new DisplayInstructions(playerInteraction),
        new InventoryManager(playerInteraction),
        new StartNewTurnClient(playerInteraction),
        new PlayerActionClient(playerInteraction, gun, rnd),
        new RidersOnTheTrailClient(playerInteraction, rnd, gun),
        new RandomEventClient(rnd, playerInteraction, determineIllness, gun),
        new CrossMountainsClient(rnd, playerInteraction, determineIllness, gun),
        new EndGameClient(),
        playerInteraction,
        new Eating(playerInteraction),
        rnd
    );

game.Play();

