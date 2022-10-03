using OregonTrail2.PlayerInteraction;
using OregonTrail2.Shooting;

namespace OregonTrail2.Riders
{
    public class RidersOnTheTrailClient : IRiders
    {
        private readonly IPlayerInteraction playerInteraction;
        private readonly IRandomNumberGenerator rnd;
        private readonly IShootTheGun gun;

        public RidersOnTheTrailClient(IPlayerInteraction playerInteraction, IRandomNumberGenerator rnd, IShootTheGun gun)
        {
            this.playerInteraction = playerInteraction;
            this.rnd = rnd;
            this.gun = gun;
        }

        public GameState RidersOnTheTrail(GameState oldState)
        {
            var areThereRiders = this.rnd.BetweenZeroAnd(10) < ((oldState.TotalMilage / 100 - 4) ^ 2 + 72) / ((oldState.TotalMilage / 100 - 4) ^ 2 + 12) - 1;
            var updatedState = areThereRiders
                ? EncounterRiders(oldState)
                : oldState;
            return updatedState;
        }

        private GameState EncounterRiders(GameState oldState)
        {
            var doRidersLookFriendly = this.rnd.BetweenZeroAnd(9) > 7;
            var ridersAreNotWhatTheySeem = this.rnd.BetweenZeroAnd(9) > 1;
            var ridersActuallyFriendly = ridersAreNotWhatTheySeem
                                                ? !doRidersLookFriendly
                                                : doRidersLookFriendly;

            var message = new[]
            {
                $"RIDERS AHEAD.  THEY { (doRidersLookFriendly ? "DON'T" : string.Empty ) } LOOK HOSTILE",
                "TACTICS",
                "(1) RUN  (2) ATTACK  (3) CONTINUE  (4) CIRCLE WAGONS",
                "IF YOU RUN YOU'LL GAIN TIME BUT WEAR DOWN YOUR OXEN",
                "IF YOU CIRCLE YOU'LL LOSE TIME"
            };
            var playerTacticInput = this.playerInteraction.GetInput(message);
            var finalPlayerTacticInput = (playerTacticInput.IterateUntil(
                    x => this.playerInteraction.GetInput("TRY AGAIN"),
                    x => x is IntegerInput i && i.IntegerFromUser > 0 && i.IntegerFromUser <= 4) as IntegerInput).IntegerFromUser;

            var updatedState = (friendly: ridersActuallyFriendly, playerChoice: finalPlayerTacticInput) switch
            {
                {  friendly: true, playerChoice: 1 } => oldState with { TotalMilage = oldState.TotalMilage + 15, Inventory = oldState.Inventory with { Ammunition = oldState.Inventory.Ammunition - 10 } },
                {  friendly: true, playerChoice: 2 } => oldState with { TotalMilage = oldState.TotalMilage - 5, Inventory = oldState.Inventory with { Ammunition = oldState.Inventory.Ammunition - 100 } },
                {  friendly: true, playerChoice: 3 } => oldState,
                {  friendly: true, playerChoice: 4 } => oldState with { TotalMilage = oldState.TotalMilage - 20 },
                {  friendly: false, playerChoice: 1 } => oldState with { TotalMilage = oldState.TotalMilage + 20, Inventory = oldState.Inventory with { MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - 15, Ammunition = oldState.Inventory.Ammunition - 150, OxenTeam = oldState.Inventory.OxenTeam - 40 } },
                {  friendly: false, playerChoice: 2 } => ShootItOutWithRiders(oldState),
                {  friendly: false, playerChoice: 3 } => ContinueFromRiders(oldState),
                {  friendly: false, playerChoice: 4 } => CircleWagons(oldState),
                _ => oldState
            };

            this.playerInteraction.WriteMessageConditional(ridersActuallyFriendly, "RIDERS WERE FRIENDLY, BUT CHECK FOR POSSIBLE LOSSES");
            this.playerInteraction.WriteMessageConditional(!ridersActuallyFriendly, "RIDERS WERE HOSTILE--CHECK FOR LOSSES");
            this.playerInteraction.WriteMessageConditional(updatedState.Inventory.Ammunition < 0, "YOU RAN OUT OF BULLETS AND GOT MASSACRED BY THE RIDERS");

            var finalState = updatedState with
            {
                IsDead = updatedState.Inventory.Ammunition < 0
            };

            return finalState;
        }

        private GameState ShootItOutWithRiders(GameState oldGameState)
        {
            var shootTime = this.gun.Shoot();

            var newState = (oldGameState with
            {
                InjuryFlag = shootTime > 4,
                Inventory = oldGameState.Inventory with
                {
                    Ammunition = oldGameState.Inventory.Ammunition - shootTime * 40 - 80
                }
            }).Message(() => this.playerInteraction.WriteMessage(ShootingMessage(shootTime).ToArray()));

            return newState;
        }

        private static IEnumerable<string> ShootingMessage(int shootTime) => shootTime switch
        {
            _ when shootTime < 1 => new[] { "NICE SHOOTING---YOU DROVE THEM OFF" },
            _ when shootTime <= 4 => new[] { "KINDA SLOW WITH YOUR COLT .45" },
            _ => new[] { "LOUSY SHOT---YOU GOT KNIFED", "YOU HAVE TO SEE OL' DOC BLANCHARD" }
        };

        private GameState ContinueFromRiders(GameState oldState)
        {
            var ridersDontAttack = this.rnd.BetweenZeroAnd(10) > 8;
            var updatedState = ridersDontAttack
                        ? oldState.Message(() => this.playerInteraction.WriteMessage("THEY DID NOT ATTACK"))
                        : oldState with
                        {
                            Inventory = oldState.Inventory with
                            {
                                Ammunition = oldState.Inventory.Ammunition - 150,
                                MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - 15
                            }
                        };
            return updatedState;
        }

        private GameState CircleWagons(GameState oldState)
        {
            var shootTime = this.gun.Shoot();
            var updatedState = (oldState with
            {
                TotalMilage = oldState.TotalMilage - 25,
                Inventory = oldState.Inventory with
                {
                    Ammunition = oldState.Inventory.Ammunition - shootTime * 30 - 80
                }
            }).Message(() => this.playerInteraction.WriteMessage(ShootingMessage(shootTime).ToArray()));

            return updatedState;
        }
    }
}
