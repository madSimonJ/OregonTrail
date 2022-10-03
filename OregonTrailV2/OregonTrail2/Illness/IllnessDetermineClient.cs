using OregonTrail2.PlayerInteraction;

namespace OregonTrail2.Illness
{
    public class IllnessDetermineClient : IDetermineIllness
    {
        private readonly IPlayerInteraction playerInteraction;
        private readonly IRandomNumberGenerator rnd;

        public IllnessDetermineClient(IPlayerInteraction playerInteraction, IRandomNumberGenerator rnd)
        {
            this.playerInteraction = playerInteraction;
            this.rnd = rnd;
        }

        public GameState DetermineIllness(GameState oldState)
        {
            var mildIllnessFactor = this.rnd.BetweenZeroAnd(100);
            var hasMildIllness = mildIllnessFactor < 10 + 35 * (oldState.LastEatingChoice - 1);
            var badIllnessFactor = this.rnd.BetweenZeroAnd(100);
            var hasBadIllness = badIllnessFactor < 100 - (40 / 4 ^ (oldState.LastEatingChoice - 1));

            var stateWithIllness = (hasMildIllness, hasBadIllness) switch
            {
                { hasMildIllness : true} => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 5,
                    Inventory = oldState.Inventory with
                    {
                        MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - 2
                    }
                }).Message(() => this.playerInteraction.WriteMessage("MILD ILLNESS---MEDICINE USED")),

                { hasBadIllness: true } => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 5,
                    Inventory = oldState.Inventory with
                    {
                        MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - 5
                    }
                }).Message(() => this.playerInteraction.WriteMessage("BAD ILLNESS---MEDICINE USED")),

                _ => (oldState with
                {
                    Inventory = oldState.Inventory with
                    {
                        MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - 10
                    },
                    IllnessFlag = true
                }).Message(() => this.playerInteraction.WriteMessage("BAD ILLNESS---MEDICINE USED")),

            };

            var stateWithOtherEffects = stateWithIllness switch
            {
                { Inventory.MiscellaneousSupplies : < 0  } => (stateWithIllness with
                {
                    IsDead = true
                }).Message(() => this.playerInteraction.WriteMessage("YOU RAN OUT MEDICAL SUPPLIES", $"YOU DIED OF {(stateWithIllness.InjuryFlag ? "INJURIES" : "PNEUMONIA") }")),

                { BlizzardFlag: true, TotalMilage: <= 950 } => (stateWithIllness with
                {
                    ClearnedSouthPassFlag = true
                })
            };


            return stateWithOtherEffects;
        }
    }
}
