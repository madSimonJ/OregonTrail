using OregonTrail2.PlayerInteraction;

namespace OregonTrail2.Inventory
{
    public class InventoryManager : IManageInventory
    {
        private enum InventoryItem
        {
            Oxen,
            Food,
            Ammunition,
            Clothing,
            Miscellaneous
        }

        private readonly IPlayerInteraction userInput;

        public InventoryManager(IPlayerInteraction userInput)
        {
            this.userInput = userInput;
        }

        public InventoryState GetInitialInventory()
        {
            var startingValue = Enumerable.Empty<(InventoryItem, int)>();

            var spends = startingValue.IterateUntil(x => UpdateInventory(x), 
                x => x.Select(x => x.Item1).Distinct().Count() == 5);

            var totalSpend = spends.Sum(x => x.Item2);
            
            if(totalSpend > 700)
            {
                this.userInput.WriteMessage("YOU OVERSPENT--YOU ONLY HAD $700 TO SPEND.  BUY AGAIN");
                return GetInitialInventory();
            }

            this.userInput.WriteMessage($"AFTER ALL YOUR PURCHASES, YOU NOW HAVE {700 - totalSpend} DOLLARS LEFT");

            var spendDictionary = spends.ToDictionary(x => x.Item1, x => x.Item2);
            var returnValue = new InventoryState
            {
                OxenTeam = spendDictionary[InventoryItem.Oxen],
                Food = spendDictionary[InventoryItem.Food],
                Ammunition = spendDictionary[InventoryItem.Ammunition] * 50,
                Clothing = spendDictionary[InventoryItem.Clothing],
                MiscellaneousSupplies = spendDictionary[InventoryItem.Miscellaneous],
                Money = 700 - totalSpend
            };

            return returnValue;
            
        }

        private IEnumerable<(InventoryItem, int)> UpdateInventory(IEnumerable<(InventoryItem, int)> prev)
        {
            if (!prev.Any())
                return new[] { (InventoryItem.Oxen, GetOxenSpend()) };

            var returnValue = prev.Max(x => x.Item1) switch
            {
                InventoryItem.Oxen => prev.Append((InventoryItem.Food,  GetNonZeroSpend("FOOD"))),
                InventoryItem.Food=> prev.Append((InventoryItem.Ammunition, GetNonZeroSpend("AMMUNITION"))),
                InventoryItem.Ammunition => prev.Append((InventoryItem.Clothing, GetNonZeroSpend("CLOTHING"))),
                InventoryItem.Clothing => prev.Append((InventoryItem.Miscellaneous, GetNonZeroSpend("MISCELANEOUS SUPPLIES")))
            };

            return returnValue;
        }

        private int GetOxenSpend()
        {
            var userInput = this.userInput.GetInput(string.Empty, string.Empty, "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM");
            var finalInput = userInput.IterateUntil(
                x => x switch
                {
                    IntegerInput ii when ii.IntegerFromUser < 200 => this.userInput.GetInput("NOT ENOUGH"),
                    IntegerInput ii when ii.IntegerFromUser > 300 => this.userInput.GetInput("TOO MUCH"),
                    IntegerInput ii => ii,
                    _ => this.userInput.GetInput("NOT AN INTEGER"),

                },
                        x => x is IntegerInput ii && ii.IntegerFromUser >= 200 && ii.IntegerFromUser <= 300);
            return (finalInput as IntegerInput).IntegerFromUser;
        }

        private int GetNonZeroSpend(string itemName)
        {
            var userInput = this.userInput.GetInput($"HOW MUCH DO YOU WANT TO SPEND ON {itemName}");
            var finalInput = userInput.IterateUntil(
                x => x switch
                {
                    IntegerInput ii when ii.IntegerFromUser < 0 => this.userInput.GetInput("IMPOSSIBLE"),
                    IntegerInput ii => ii,
                    _ => this.userInput.GetInput("NOT AN INTEGER"),

                },
                x => x is IntegerInput ii && ii.IntegerFromUser >= 0);
            return (finalInput as IntegerInput).IntegerFromUser;
        }
    }
}
