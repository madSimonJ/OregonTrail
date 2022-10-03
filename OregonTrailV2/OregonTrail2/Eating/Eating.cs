using OregonTrail2.PlayerInteraction;

namespace OregonTrail2.Eating
{
    public class Eating : IHandleEating
    {
        private readonly IPlayerInteraction playerInteraction;


        public Eating(IPlayerInteraction playerInteraction)
        {
            this.playerInteraction = playerInteraction;
        }
        public GameState HandleEating(GameState state)
        {
            var checkPlayerStillAlive =
                    state switch
                    {
                        { Inventory.Food: < 13 } => (state with
                        {
                            IsDead = true
                        }).Message(() => this.playerInteraction.WriteMessage("YOU RAN OUT OF FOOD AND STARVED TO DEATH")),
                        _ => state
                    };

            var userInitialChoice = () => this.playerInteraction.GetInput("DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY", "OR (3) WELL");
            var userFinalChoice = userInitialChoice().IterateUntil(
                    x => userInitialChoice(),
                    x => x is IntegerInput i && i.IntegerFromUser >= 1 && i.IntegerFromUser <= 3) as IntegerInput;

            var amountOfFoodToEat = 8 - 5 * userFinalChoice.IntegerFromUser;

            var stateAfterEating = state switch
            {
                _ when state.Inventory.Food > amountOfFoodToEat => state with
                {
                    Inventory = state.Inventory with
                    {
                        Food = state.Inventory.Food - amountOfFoodToEat
                    },
                    LastEatingChoice = userFinalChoice.IntegerFromUser
                },
                _ => state.Message(() => this.playerInteraction.WriteMessage("YOU CAN'T EAT THAT WELL")).ContinueGame(HandleEating)
            };

            return stateAfterEating;
        }
    }
}
