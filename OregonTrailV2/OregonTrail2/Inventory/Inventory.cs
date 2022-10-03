namespace OregonTrail2.Inventory
{
    public record InventoryState
    {
        public int OxenTeam { get; set; }
        public int Food { get; set; }
        public int Ammunition { get; set; }
        public int Clothing { get; set; }
        public int MiscellaneousSupplies { get; set; }
        public int Money { get; set; }
    }
}
