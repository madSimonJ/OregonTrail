using OregonTrail2.Illness;
using OregonTrail2.PlayerInteraction;
using OregonTrail2.Shooting;

namespace OregonTrail2.RandomEvents
{
    public class RandomEventClient : IRandomEvents
    {
        private readonly IRandomNumberGenerator rnd;
        private readonly IPlayerInteraction playerInteration;
        private readonly IDetermineIllness determineIllness;
        private readonly IShootTheGun gun;

        public RandomEventClient(IRandomNumberGenerator rnd, IPlayerInteraction playerInteration, IDetermineIllness determineIllness, IShootTheGun gun)
        {
            this.rnd = rnd;
            this.playerInteration = playerInteration;
            this.determineIllness = determineIllness;
            this.gun = gun;
        }

        public GameState HandleRandomEvents(GameState oldState)
        {
            var randomNumber = this.rnd.BetweenZeroAnd(100);

            var selectedEvent = new (int, Func<GameState, GameState>)[]
            {
                (6, x => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 15 - this.rnd.BetweenZeroAnd(5),
                    Inventory = oldState.Inventory with
                    {
                        MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - 8}
                } ).Message(() => this.playerInteration.WriteMessage("WAGON BREAKS DOWN--LOSE TIME AND SUPPLIES FIXING IT"))),

                (11, x => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 25,
                    Inventory = oldState.Inventory with
                    {
                        OxenTeam = oldState.Inventory.OxenTeam- 20}
                } ).Message(() => this.playerInteration.WriteMessage("OX INJURES LEG---SLOWS YOU DOWN REST OF TRIP"))),


                (13, x => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 5 - this.rnd.BetweenZeroAnd(4),
                    Inventory = oldState.Inventory with
                    {
                        MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies- 2 - this.rnd.BetweenZeroAnd(3)}
                } ).Message(() => this.playerInteration.WriteMessage("BAD LUCK---YOUR DAUGHTER BROKE HER ARM", "YOU HAD TO STOP AND USE SUPPLIES TO MAKE A SLING"))),

                (15, x => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 17
                } ).Message(() => this.playerInteration.WriteMessage("OX WANDERS OFF---SPEND TIME LOOKING FOR IT"))),

               (17, x => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 10
                } ).Message(() => this.playerInteration.WriteMessage("YOUR SON GETS LOST---SPEND HALF THE DAY LOOKING FOR HIM"))),

               (22, x => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - this.rnd.BetweenZeroAnd(10) - 2
                } ).Message(() => this.playerInteration.WriteMessage("UNSAFE WATER--LOSE TIME LOOKING FOR CLEAN SPRING"))),

               (32, HandleBadWeather),

               (35, HandleBanditsAttack),

                (37, x => (oldState with
                {
                    Inventory = oldState.Inventory with
                    {
                        Food = oldState.Inventory.MiscellaneousSupplies- 40,
                        Ammunition = oldState.Inventory.Ammunition - 400,
                    MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - this.rnd.BetweenZeroAnd(8) - 3},
                    TotalMilage = oldState.TotalMilage - 15
                } ).Message(() => this.playerInteration.WriteMessage("THERE WAS A FIRE IN YOUR WAGON--FOOD AND SUPPLIES DAMAGED"))),

               (42, x => (oldState with
                {
                    TotalMilage = oldState.TotalMilage - 10 - this.rnd.BetweenZeroAnd(5)
                } ).Message(() => this.playerInteration.WriteMessage("LOSE YOUR WAY IN HEAVY FOG---TIME IS LOST"))),

                (44, HandleSnakeBite),

                (54, x => (oldState with
                {
                    Inventory = oldState.Inventory with
                    {
                        Food = oldState.Inventory.MiscellaneousSupplies- 30,
                        Clothing = oldState.Inventory.Clothing - 30},
                    TotalMilage = oldState.TotalMilage - 20 - this.rnd.BetweenZeroAnd(20)
                } ).Message(() => this.playerInteration.WriteMessage("WAGON GETS SWAMPED FORDING RIVER--LOSE FOOD AND CLOTHES"))),

                (64, x => (oldState with
                {
                    Inventory = oldState.Inventory with
                    {
                        Ammunition = oldState.Inventory.Ammunition- 200,
                        MiscellaneousSupplies = oldState.Inventory.MiscellaneousSupplies - 4 - this.rnd.BetweenZeroAnd(3)},
                    TotalMilage = oldState.TotalMilage - 5 - this.rnd.BetweenZeroAnd(10)
                } ).Message(() => this.playerInteration.WriteMessage("HAIL STORM---SUPPLIES DAMAGED"))),

                (69, HandleIllness),

                (100, x => (oldState with
                {
                    Inventory = oldState.Inventory with
                    {
                        Food = oldState.Inventory.MiscellaneousSupplies + 14}
                } ).Message(() => this.playerInteration.WriteMessage("HELPFUL INDIANS SHOW YOU WHERE TO FIND MORE FOOD"))
                
                )

            }.First(x => x.Item1 > randomNumber).Item2;

            var updatedState = selectedEvent(oldState);
            return updatedState;
        }

        private GameState HandleBadWeather(GameState oldGameState) =>
            oldGameState switch
            {
                { TotalMilage: > 950 } => (oldGameState.Inventory.Clothing > 22 +  this.rnd.BetweenZeroAnd(4)).Map(x =>  (oldGameState with
                {
                    InsufficientClothingInColdWeatherFlag = !x,
                }).Message(() => this.playerInteration.WriteMessage($"COLD WEATHER---BRRRRRRR!---YOU {(x ? string.Empty : "HAVE ENOUGH CLOTHING TO KEEP YOU WARM")} ")))
                        .Map(x => x.InsufficientClothingInColdWeatherFlag ? this.determineIllness.DetermineIllness(x) : x),
                _ => (oldGameState with
                {   
                    TotalMilage = oldGameState.TotalMilage - this.rnd.BetweenZeroAnd(10) - 5,
                    Inventory = oldGameState.Inventory with
                    {
                        Food = oldGameState.Inventory.Food - 10,
                        Ammunition = oldGameState.Inventory.Ammunition - 500,
                        MiscellaneousSupplies = oldGameState.Inventory.MiscellaneousSupplies - 15
                    }

                }).Message(() => this.playerInteration.WriteMessage("HEAVY RAINS---TIME AND SUPPLIES LOST"))
            };

        private GameState HandleBanditsAttack(GameState oldGameState)
        {
            this.playerInteration.WriteMessage("BANDITS ATTACK");
            var timeTakenToShoot = this.gun.Shoot();
            var bulletsLost = 20 * timeTakenToShoot;
            var stateWithoutBullets = oldGameState with
            {
                Inventory = oldGameState.Inventory with
                {
                    Ammunition = oldGameState.Inventory.Ammunition - bulletsLost
                }
            };

            var runOutOfBullets = stateWithoutBullets switch
            {
                { Inventory.Ammunition: < 0 } => (
                    stateWithoutBullets with
                    {
                        Inventory = stateWithoutBullets.Inventory with
                        {
                            Money = stateWithoutBullets.Inventory.Money / 3
                        }
                    }
                ).Message(() => this.playerInteration.WriteMessage("YOU RAN OUT OF BULLETS---THEY GET LOTS OF CASH")),
                _ => stateWithoutBullets
            };

            var outdrawBandits = (timeTakenToShoot, runOutOfBullets) switch
            {
                { timeTakenToShoot: <= 1 } => runOutOfBullets.Message(() => this.playerInteration.WriteMessage("QUICKEST DRAW OUTSIDE OF DODGE CITY!!!", "YOU GOT 'EM!")),
                _ => (runOutOfBullets with
                {
                    InjuryFlag = true,
                    Inventory = runOutOfBullets.Inventory with
                    {
                        MiscellaneousSupplies = runOutOfBullets.Inventory.MiscellaneousSupplies - 5,
                        OxenTeam = runOutOfBullets.Inventory.OxenTeam - 20
                    }

                }).Message(() => this.playerInteration.WriteMessage(""))
            };

            return outdrawBandits;

        }

        private GameState HandleSnakeBite(GameState oldGameState)
        {
            var killedSnakeAfterBite = (oldGameState with
            {
                Inventory = oldGameState.Inventory with
                {
                    Ammunition = oldGameState.Inventory.Ammunition - 10,
                    MiscellaneousSupplies = oldGameState.Inventory.MiscellaneousSupplies - 5,
                }
            }).Message(() => this.playerInteration.WriteMessage("YOU KILLED A POISONOUS SNAKE AFTER IT BIT YOU"));

            var didPlayerDie = killedSnakeAfterBite switch
            {
                { Inventory.MiscellaneousSupplies: < 0 } => (
                    killedSnakeAfterBite with
                    {
                        IsDead = true
                    }
                ).Message(() => this.playerInteration.WriteMessage("YOU DIE OF SNAKEBITE SINCE YOU HAVE NO MEDICINE")),
                _ => killedSnakeAfterBite
            };

            return didPlayerDie;
        }

        private GameState HandleIllness(GameState oldGameState)
        {
            var randomFactor = this.rnd.BetweenZeroAnd(99);

            var updateWithIllness = (oldGameState, randomFactor) switch
            {
                { oldGameState.LastEatingChoice: 1 } 
                    or { oldGameState.LastEatingChoice: 2, randomFactor: > 25  }
                    or { oldGameState.LastEatingChoice: 3, randomFactor: > 50 }
                => this.determineIllness.DetermineIllness(oldGameState),
                _ => oldGameState
            };

            return updateWithIllness;
        }
    }
}
