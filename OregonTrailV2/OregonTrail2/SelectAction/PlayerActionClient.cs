using OregonTrail2.Inventory;
using OregonTrail2.PlayerInteraction;
using OregonTrail2.Shooting;

namespace OregonTrail2.SelectAction
{
    public class PlayerActionClient : ISelectPlayerAction
    {
        private readonly IPlayerInteraction userInteraction;
        private readonly IShootTheGun gun;
        private readonly IRandomNumberGenerator randomNumberGenerator;

        public PlayerActionClient(IPlayerInteraction userInteraction, IShootTheGun gun, IRandomNumberGenerator randomNumberGenerator)
        {
            this.userInteraction = userInteraction;
            this.gun = gun;
            this.randomNumberGenerator = randomNumberGenerator;
        }

        private enum UserChoice
        {
            StopAtNextFort,
            Hunt,
            Continue
        }

        public GameState SelectAction(GameState oldGameState)
        {
            var message = oldGameState.IsFortTurn
                ? new[] { "DO YOU WANT TO (1) STOP AT THE NEXT FORT, (2) HUNT, ", "OR (3) CONTINUE" }
                : new[] { "DO YOU WANT TO (1) HUNT, OR (2) CONTINUE" };

            var notEnoughBulletsMessage = new[] { "TOUGH---YOU NEED MORE BULLETS TO GO HUNTING" }.Concat(message).ToArray();

            var validChoices = oldGameState.IsFortTurn
                ? new[] { 1, 2, 3 }
                : new[] { 1, 2 }; 

            var firstPlayerInput = this.userInteraction.GetInput(message);
            var finalPlayerInput = firstPlayerInput.IterateUntil(x => x switch
            {
                IntegerInput i when oldGameState.IsFortTurn && i.IntegerFromUser == 2 && oldGameState.Inventory.Ammunition <= 39
                                        || !oldGameState.IsFortTurn && i.IntegerFromUser == 1 =>
                                            this.userInteraction.GetInput(notEnoughBulletsMessage),
                _ => this.userInteraction.GetInput(message)

            },
                x => x is IntegerInput ii && validChoices.Contains(ii.IntegerFromUser));


            var playerChoice = finalPlayerInput switch
            {
                IntegerInput i when i.IntegerFromUser == 1 && oldGameState.IsFortTurn => UserChoice.StopAtNextFort,
                IntegerInput i when (i.IntegerFromUser == 1 && !oldGameState.IsFortTurn) ||
                                        (i.IntegerFromUser == 2 && oldGameState.IsFortTurn) => UserChoice.Hunt,
                IntegerInput i when (i.IntegerFromUser == 2 && !oldGameState.IsFortTurn) ||
                                        (i.IntegerFromUser == 3 && oldGameState.IsFortTurn) => UserChoice.Continue
            };

            var updatedGameState = playerChoice switch
            {
                UserChoice.StopAtNextFort => StopAtFort(oldGameState),
                UserChoice.Hunt => GoHunting(oldGameState),
                _ => oldGameState
            };

            return updatedGameState with
            {
                IsFortTurn = !updatedGameState.IsFortTurn
            };
        }

        private GameState StopAtFort(GameState oldGameState)
        {
            this.userInteraction.WriteMessage("ENTER WHAT YOU WISH TO SPEND ON THE FOLLOWING");

            var updatedState = oldGameState.ContinueGame(x => MakePurchase(x, "FOOD", (x, y) => y with { Food = y.Food - x }))
                        .ContinueGame(x => MakePurchase(x, "AMMUNITION", (x, y) => y with { Ammunition = y.Ammunition + (int)Math.Round(2M/3M * x * 50M) }))
                        .ContinueGame(x => MakePurchase(x, "CLOTHING", (x, y) => y with { Clothing = y.Clothing + (int)Math.Round(2m/3m * x) }))
                        .ContinueGame(x => MakePurchase(x, "MISCELLANEOUS SUPPLIES", (x, y) => y with { MiscellaneousSupplies = y.MiscellaneousSupplies + (int)Math.Round(2m/3m * x)  }));
            return updatedState;

        }

        private GameState MakePurchase(GameState oldGameState, string itemName, Func<int, InventoryState, InventoryState> update)
        {
            var spend = this.userInteraction.GetInput(itemName).IterateUntil(
                x => this.userInteraction.GetInput("TRY AGAIN"),
                x => x is IntegerInput i && i.IntegerFromUser > 0);

            var updateWithPurchase = spend switch
            {
                IntegerInput i when i.IntegerFromUser > oldGameState.Inventory.Money =>
                    oldGameState.Message(() => this.userInteraction.WriteMessage("YOU DON'T HAVE THAT MUCH--KEEP YOUR SPENDING DOWN")),
                IntegerInput i => oldGameState with
                {
                    Inventory = update(i.IntegerFromUser, oldGameState.Inventory with
                    {
                        Money = oldGameState.Inventory.Money - i.IntegerFromUser
                    })
                }
            };

            return updateWithPurchase;
        }

        private GameState GoHunting(GameState oldGameState)
        {
            var stateWithLostMilage = oldGameState with { TotalMilage = oldGameState.TotalMilage - 45 };
            var timeTakenToShoot = this.gun.Shoot();

            var stateAfterHunting = timeTakenToShoot switch
            {
                _ when timeTakenToShoot <= 1 => (stateWithLostMilage with
                    {
                        Inventory = stateWithLostMilage.Inventory with
                        {
                            Food = stateWithLostMilage.Inventory.Food + 52 + this.randomNumberGenerator.BetweenZeroAnd(6),
                            Ammunition = stateWithLostMilage.Inventory.Ammunition - 10 - this.randomNumberGenerator.BetweenZeroAnd(4)
                        }
                    }).Message(() => this.userInteraction.WriteMessage("RIGHT BETWEEN THE EYES---YOU GOT A BIG ONE!!!!")),

                _ when timeTakenToShoot * 13 < this.randomNumberGenerator.BetweenZeroAnd(100) => (
                    stateWithLostMilage with
                    {
                        Inventory = stateWithLostMilage.Inventory with
                        {
                            Food = stateWithLostMilage.Inventory.Food + 48 + this.randomNumberGenerator.BetweenZeroAnd(6),
                            Ammunition = stateWithLostMilage.Inventory.Ammunition - 10 - this.randomNumberGenerator.BetweenZeroAnd(4)
                        }
                    }
                ).Message(() => this.userInteraction.WriteMessage("NICE SHOT--RIGHT THROUGH THE NECK--FEAST TONIGHT!!")),

                _ => stateWithLostMilage.Message(() => this.userInteraction.WriteMessage("SORRY---NO LUCK TODAY"))

            };

            return stateAfterHunting;

        }
    }
}
