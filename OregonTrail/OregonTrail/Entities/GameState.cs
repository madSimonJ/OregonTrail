
namespace OregonTrail.Entities
{
    public enum Request
    {
        StartGame = 0,
        DoYouRequireInstruction = 1,
        HowMuchSpendOnOxen,
        HowMuchSpendOnFood,
        HowMuchSpendOnAmmunition,
        HowMuchSpendOnClothing,
        HowMuchSpendOnMisc,
        BeginGame
    }
    public record GameState
    {
        public int TurnNumber { get; set; } = 1;
        public bool IsGameFinished { get; set; }
        public IEnumerable<string> Text { get; set; } = Enumerable.Empty<string>();
        public Request Request { get; set; }
        public int Food { get; set; }
        public int Oxen { get; set; }
        public int Ammunition { get; set; }
        public int MiscelaneousSupplies { get; set; }
        public int Clothing { get; set; }
        public int Money { get; set; }
    }
}
