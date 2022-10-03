using OregonTrail2.Illness;
using OregonTrail2.PlayerInteraction;
using OregonTrail2.Shooting;

namespace OregonTrail2.Mountains
{
    internal class CrossMountainsClient : IMountains
    {
        private readonly IRandomNumberGenerator rnd;
        private readonly IPlayerInteraction playerInteration;
        private readonly IDetermineIllness determineIllness;
        private readonly IShootTheGun gun;

        public CrossMountainsClient(IRandomNumberGenerator rnd, IPlayerInteraction playerInteration, IDetermineIllness determineIllness, IShootTheGun gun)
        {
            this.rnd = rnd;
            this.playerInteration = playerInteration;
            this.determineIllness = determineIllness;
            this.gun = gun;
        }

        public GameState CrossMountains(GameState oldState)
        {

            var crossedMountains = oldState switch
            {
                { TotalMilage: < 950 } => oldState,
                _ => HandleCrossMountains(oldState)
            };

            return crossedMountains;
        }

        private GameState HandleCrossMountains(GameState oldGameState)
        {
            var chanceOfReachingPass = this.rnd.BetweenZeroAnd(10) > 9 - ((oldGameState.TotalMilage / 100 - 15) ^ 2 + 72) / ((oldGameState.TotalMilage / 100 - 15) ^ 2 + 12);

            var crossMountains = (chanceOfReachingPass, oldGameState) switch
            {
                { oldGameState.TotalMilage: <= 950 } => oldGameState,
                { chanceOfReachingPass: false } => HandleRuggedMountains(oldGameState),
                { chanceOfReachingPass: true, oldGameState.ClearnedSouthPassFlag: false } => HandleClearSouthPass(oldGameState),
                { chanceOfReachingPass: true, oldGameState.ClearedBlueMountains: false, oldGameState.TotalMilage: >= 1700  } => HandleClearBlueMountains(oldGameState),
                _ => oldGameState
            };

            var updatedState = crossMountains switch
            {
                { TotalMilage: <= 950 } => crossMountains with { ClearnedSouthPassForSettingMilageFlag = true },
                _ => crossMountains
            };

            return updatedState;
        }

        private GameState HandleRuggedMountains(GameState oldGameState)
        {
            var randomFactor = this.rnd.BetweenZeroAnd(100);

            var updatedState = (oldGameState, randomFactor) switch
            {
                { randomFactor: <= 1 } => (oldGameState with { TotalMilage = oldGameState.TotalMilage - 60})
                    .Message(() => this.playerInteration.WriteMessage("YOU GOT LOST---LOSE VALUABLE TIME TRYING TO FIND TRAIL!")),
                { randomFactor: <= 11} => (oldGameState with
                {
                    TotalMilage = oldGameState.TotalMilage - 20 - this.rnd.BetweenZeroAnd(30),
                    Inventory = oldGameState.Inventory with
                    {
                        MiscellaneousSupplies = oldGameState.Inventory.MiscellaneousSupplies - 5,
                        Ammunition = oldGameState.Inventory.Ammunition - 200
                    }
                }).Message(() => this.playerInteration.WriteMessage("WAGON DAMAGED!---LOSE TIME AND SUPPLIES")),
                _ => (
                    oldGameState with
                    {
                        TotalMilage = oldGameState.TotalMilage - 45 - this.rnd.BetweenZeroAnd(50)
                    }
                )
            };

            return updatedState;
        }

        private GameState HandleClearSouthPass(GameState oldGameState)
        {
            var isBlizzard = this.rnd.BetweenZeroAnd(100) < 80;

            var passedSouthPass = oldGameState with { ClearnedSouthPassFlag = true };

            var updatedState = isBlizzard
                ? HandleBlizzardInMountainPass(passedSouthPass)
                : passedSouthPass.Message(() => this.playerInteration.WriteMessage("YOU MADE IT SAFELY THROUGH SOUTH PASS--NO SNOW"));

            return updatedState;
        }

        private GameState HandleClearBlueMountains(GameState oldGameState)
        {
            var passedBlueMountains = oldGameState with { ClearedBlueMountains = true };
            var isBlizzard = this.rnd.BetweenZeroAnd(100) < 70;

            var updatedState = isBlizzard
                ? HandleBlizzardInMountainPass(passedBlueMountains)
                : passedBlueMountains;

            return updatedState;

        }

        private GameState HandleBlizzardInMountainPass(GameState oldGameState)
        {
            this.playerInteration.WriteMessage("BLIZZARD IN MOUNTAIN PASS--TIME AND SUPPLIES LOST");

            var blizzardState = oldGameState with
            {
                BlizzardFlag = true,
                TotalMilage = oldGameState.TotalMilage - 30 - this.rnd.BetweenZeroAnd(40),
                Inventory = oldGameState.Inventory with
                {
                    Food = oldGameState.Inventory.Food - 25,
                    MiscellaneousSupplies = oldGameState.Inventory.MiscellaneousSupplies - 10,
                    Ammunition = oldGameState.Inventory.Ammunition - 300,
                }
            };

            var illnessState = blizzardState.Inventory.Clothing < 18 + this.rnd.BetweenZeroAnd(2)
                ? this.determineIllness.DetermineIllness(blizzardState)
                : blizzardState;
            
            return illnessState;
        }
    }
}
