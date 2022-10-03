using OregonTrail2.PlayerInteraction;

namespace OregonTrail2.StartTurn
{
    public class StartNewTurnClient : IStartTheTurn
    {
        private readonly IPlayerInteraction userInteraction;

        public StartNewTurnClient(IPlayerInteraction userInteraction)
        {
            this.userInteraction = userInteraction;
        }

        public GameState StartNewTurn(GameState gs)
        {
            var zeroedGameState = gs
                                    .Message(() => this.userInteraction.WriteMessage(DateLookup[gs.TurnNumber]))
                                    .ContinueGame(x => x with
                                    {
                                        TurnNumber = x.TurnNumber + 1
                                    })
                                    .ContinueGame(ZeroInventory)
                                    .Message(() => this.userInteraction.WriteMessageConditional(
                                        gs.Inventory.Food < 12, "YOU'D BETTER DO SOME HUNTING OR BUY FOOD AND SOON!!!!")
                                    )
                                    .ContinueGame(VisitDoctor)
                                    .ContinueGame(DisplayMilage);
            return zeroedGameState;
            
        }

        public static GameState ZeroInventory(GameState oldGameState) =>
            oldGameState with
            {
                Inventory = oldGameState.Inventory with
                {
                    OxenTeam = oldGameState.Inventory.OxenTeam < 0 ? 0 : oldGameState.Inventory.OxenTeam,
                    Food = oldGameState.Inventory.Food < 0 ? 0 : oldGameState.Inventory.Food,
                    Ammunition = oldGameState.Inventory.Ammunition < 0 ? 0 : oldGameState.Inventory.Ammunition,
                    Clothing = oldGameState.Inventory.Clothing < 0 ? 0 : oldGameState.Inventory.Clothing,
                    MiscellaneousSupplies = oldGameState.Inventory.MiscellaneousSupplies < 0 ? 0 : oldGameState.Inventory.MiscellaneousSupplies
                }
            };


        private readonly Dictionary<int, string> DateLookup = new()
        {
            {0, "MONDAY MARCH 29 1847" },
            {1, "MONDAY APRIL 12 1847" },
            {2, "MONDAY APRIL 26 1847" },
            {3, "MONDAY MAY 10 1847" },
            {4, "MONDAY MAY 24 1847" },
            {5, "MONDAY JUNE 7 1847" },
            {6, "MONDAY JUNE 21 1847" },
            {7, "MONDAY JULY 5 1847" },
            {8, "MONDAY JULY 19 1847" },
            {9, "MONDAY AUGUST 2 1847" },
            {10, "MONDAY AUGUST 16 1847" },
            {11, "MONDAY AUGUST 31 1847" },
            {12, "MONDAY SEPTEMBER 13 1847" },
            {13, "MONDAY SEPTEMBER 27 1847" },
            {14, "MONDAY OCTOBER 11 1847" },
            {15, "MONDAY OCTOBER 25 1847" },
            {16, "MONDAY NOVEMBER 8 1847" },
            {17, "MONDAY NOVEMBER 22 1847" }
        };

        private GameState VisitDoctor(GameState oldGameState) =>
            oldGameState switch
            {
                ({ InjuryFlag: true } or { IllnessFlag: true }) and { Inventory.Money:  >= 20 } => 
                        (oldGameState with
                        {
                            Inventory = oldGameState.Inventory with
                            {
                                Money = oldGameState.Inventory.Money - 20
                            },
                            InjuryFlag = false,
                            IllnessFlag = false
                        }).Message(() => this.userInteraction.WriteMessage("DOCTOR'S BILL IS $20")),
                ({ InjuryFlag: true } or { IllnessFlag: true }) and { Inventory.Money: < 20 } => (oldGameState with
                {
                    IsDead = true,
                    Inventory = oldGameState.Inventory with { Money = 0 }
                }).Message(() => this.userInteraction.WriteMessage("YOU CAN'T AFFORD A DOCTOR", 
                                                                        "YOU DIED OF " + (oldGameState.InjuryFlag ? "INJURIES" : "PNEUMONIA"))),
                _ => oldGameState
            };

        private GameState DisplayMilage(GameState oldGameState) =>
            oldGameState switch
            {
                { ClearnedSouthPassFlag: true } => (oldGameState with { ClearnedSouthPassFlag = true })
                                                        .Message(() => this.userInteraction.WriteMessage("TOTAL MILEAGE IS 950")),
                _ => oldGameState.Message(() => this.userInteraction.WriteMessage($"TOTAL MILEAGE IS {oldGameState.TotalMilage}"))
            };

    }
}
