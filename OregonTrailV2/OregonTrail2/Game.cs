using OregonTrail2.EndGame;
using OregonTrail2.Instruction;
using OregonTrail2.Inventory;
using OregonTrail2.Mountains;
using OregonTrail2.RandomEvents;
using OregonTrail2.Riders;
using OregonTrail2.SelectAction;
using OregonTrail2.StartTurn;
using OregonTrail2.PlayerInteraction;
using OregonTrail2.Eating;

namespace OregonTrail2
{
    public  class Game
    {
        private readonly IManageInventory inventoryManager;
        private readonly IDisplayInstructions displayInstructions;
        private readonly ISelectPlayerAction selectUserAction;
        private readonly IStartTheTurn startTurn;
        private readonly IRiders riders;
        private readonly IRandomEvents randomEvents;
        private readonly IMountains mountains;
        private readonly IEndGame endGame;
        private readonly IPlayerInteraction userInteraction;
        private readonly IHandleEating eating;
        private readonly IRandomNumberGenerator rnd;

        public Game(IDisplayInstructions displayInstructions, IManageInventory inventoryManager, IStartTheTurn startTurn, ISelectPlayerAction selectUserAction, IRiders riders, IRandomEvents randomEvents, IMountains mountains, IEndGame endGame, IPlayerInteraction userInteraction, IHandleEating eating, IRandomNumberGenerator rnd)
        {
            this.inventoryManager = inventoryManager;
            this.displayInstructions = displayInstructions; 
            this.startTurn = startTurn; 
            this.selectUserAction = selectUserAction;
            this.riders = riders;
            this.randomEvents = randomEvents;
            this.mountains = mountains;
            this.endGame = endGame;
            this.userInteraction = userInteraction;
            this.eating = eating;
            this.rnd = rnd;
        }

        public void Play()
        {
            this.displayInstructions.DisplayInstructions();

            var initialState = new GameState(this.inventoryManager.GetInitialInventory());


            var finalState = initialState.IterateUntil(x =>
                x.ContinueGame(this.startTurn.StartNewTurn)
                .ContinueGame(this.selectUserAction.SelectAction)
                .ContinueGame(this.eating.HandleEating)
                .ContinueGame(x => x with
                {
                    TotalMilage = x.TotalMilage + 200 + (x.Inventory.OxenTeam - 200) / 5 + 10 + this.rnd.BetweenZeroAnd(10),
                    BlizzardFlag = false,
                    InsufficientClothingInColdWeatherFlag = false
                })
                .ContinueGame(this.riders.RidersOnTheTrail)
                .ContinueGame(this.randomEvents.HandleRandomEvents)
                .ContinueGame(this.mountains.CrossMountains)
                ,
                x => x.GameOver
            ) ;

            this.endGame.EndGame(finalState);

        }
    }
}
