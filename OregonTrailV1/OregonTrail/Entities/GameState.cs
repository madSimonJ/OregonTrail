
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
        BeginTurn,
        TurnAction,
        HuntingResult,
        RidersAhead,
        HowWellEat,
        DeadWouldLikeMinister,
        DeadWantFancyFuneral,
        DeadInformNextOfkin,
        DeadEndGame,
        RandomEvent,
        RidersWhatToDo,
        ShootRiders
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
        public int MiscellaneousSupplies { get; set; }
        public int Clothing { get; set; }
        public int Money { get; set; }
        public int DateCounter { get; set; }
        public bool Fort { get; set; }
        public DateTime HuntingTimeBegin { get; set; }
        public int MilesTraveled { get; set; }
        public bool Blizzard { get; set; }
        public bool InsufficientClothing { get; set; }
        public bool RidersAreFriendly { get; set; }
        public bool Injured { get; set; }
    }

    public enum Event
    {
        WagonBreaksDown,
        OxInjuresLeg,
        DaughterBrokeHerArm,
        OxWandersOff,
        SonGetsLost,
        UnsafeWater,
        HeavyRain,
        BanditsAttack,
        FireInWagon,
        LostInHeavyFog,
        KilledPoisonousSnake,
        WagonSwampedFordingRiver,
        WildAnimalsAttack,
        HailStorm,
        Illness,
        HelpfulIndians,
        RuggedMountains,

    }
}
