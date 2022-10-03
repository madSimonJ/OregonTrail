using OregonTrail2.Inventory;
using OregonTrail2.PlayerInteraction;

namespace OregonTrail2
{
    public record GameState
    {
        public GameState(InventoryState initialInventory )
        {
            this.Inventory = initialInventory;
        }

        public InventoryState Inventory { get; set; }
        public bool IsDead { get; set; }
        public bool GameOver { get; set; }
        public int TurnNumber { get; set; }
        public bool InjuryFlag { get; set; }
        public bool IllnessFlag { get; set; }
        public bool ClearnedSouthPassFlag { get; set; }
        public int TotalMilage { get; set; }
        public bool IsFortTurn { get; set; }
        public bool BlizzardFlag { get; set; }
        public bool InsufficientClothingInColdWeatherFlag { get; set; }
        public int LastEatingChoice { get; set; }
        public bool ClearedBlueMountains { get; internal set; }
        public bool ClearnedSouthPassForSettingMilageFlag { get; internal set; }
    }
}