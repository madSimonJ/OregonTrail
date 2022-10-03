using System.Security.Cryptography.X509Certificates;
using OregonTrail.Common;
using OregonTrail.Entities;
using OregonTrail.Random;
using OregonTrail.TimeService;

namespace OregonTrail.Turns
{


    public class TurnMaker : IMakeTheNextTurn
    {
        private readonly ITimeService _timeService;
        private readonly IGenerateRandomNumbers _rnd;
        private readonly IHandleEndOfTurnEvents endOfTurnEventHandler;

        public TurnMaker(ITimeService timeService, IGenerateRandomNumbers rnd, IHandleEndOfTurnEvents endOfTurnEventHandler)
        {
            _timeService = timeService;
            _rnd = rnd;
            this.endOfTurnEventHandler = endOfTurnEventHandler;
        }

        public GameState MakeNextTurn(GameState state, string userInput)
        {
            var userInputAsInt = int.TryParse(userInput, out var result) ? result : -1;

            var firstRoundOfUpdates = state.Request switch
            {
                Request.StartGame => state with
                {
                    Text = new[]
                    {
                        "DO YOU NEED INSTRUCTIONS  (YES/NO)"
                    },
                    TurnNumber = 1,
                    Request = Request.DoYouRequireInstruction
                },
                Request.DoYouRequireInstruction when IsYes(userInput) => state with
                {
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        "THIS PROGRAM SIMULATES A TRIP OVER THE OREGON TRAIL FROM",
                        "INDEPENDENCE, MISSOURI TO OREGON CITY, OREGON IN 1847.",
                        "YOUR FAMILY OF FIVE WILL COVER THE 2000 MILE OREGON TRAIL",
                        "IN 5-6 MONTHS --- IF YOU MAKE IT ALIVE.",
                        string.Empty,
                        "YOU HAD SAVED $900 TO SPEND FOR THE TRIP, AND YOU'VE JUST",
                        "   PAID $200 FOR A WAGON.",
                        "YOU WILL NEED TO SPEND THE REST OF YOUR MONEY ON THE",
                        "   FOLLOWING ITEMS:",
                        string.Empty,
                        "     OXEN - YOU CAN SPEND $200-$300 ON YOUR TEAM",
                        "            THE MORE YOU SPEND, THE FASTER YOU'LL GO",
                        "               BECAUSE YOU'LL HAVE BETTER ANIMALS",
                        string.Empty,
                        "     FOOD - THE MORE YOU HAVE, THE LESS CHANCE THERE",
                        "               IS OF GETTING SICK",
                        string.Empty,
                        "     AMMUNITION - $1 BUYS A BELT OF 50 BULLETS",
                        "            YOU WILL NEED BULLETS FOR ATTACKS BY ANIMALS",
                        "               AND BANDITS, AND FOR HUNTING FOOD",
                        string.Empty,
                        "     CLOTHING - THIS IS ESPECIALLY IMPORTANT FOR THE COLD",
                        "               WEATHER YOU WILL ENCOUNTER WHEN CROSSING",
                        "               THE MOUNTAINS",
                        string.Empty,
                        "     MISCELLANEOUS SUPPLIES - THIS INCLUDES MEDICINE AND",
                        "               OTHER THINGS YOU WILL NEED FOR SICKNESS",
                        "               AND EMERGENCY REPAIRS",
                        string.Empty,
                        string.Empty,
                        "YOU CAN SPEND ALL YOUR MONEY BEFORE YOU START YOUR TRIP -",
                        "OR YOU CAN SAVE SOME OF YOUR CASH TO SPEND AT FORTS ALONG",
                        "THE WAY WHEN YOU RUN LOW.  HOWEVER, ITEMS COST MORE AT",
                        "THE FORTS.  YOU CAN ALSO GO HUNTING ALONG THE WAY TO GET",
                        "MORE FOOD.",
                        "WHENEVER YOU HAVE TO USE YOUR TRUSTY RIFLE ALONG THE WAY,",
                        "YOU WILL SEE THE WORDS: TYPE BANG.  THE FASTER YOU TYPE",
                        "IN THE WORD 'BANG' AND HIT THE 'RETURN' KEY, THE BETTER",
                        "LUCK YOU'LL HAVE WITH YOUR GUN.",
                        string.Empty,
                        "WHEN ASKED TO ENTER MONEY AMOUNTS, DON'T USE A '$'.",
                        "GOOD LUCK!!!",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    }
                },
                Request.DoYouRequireInstruction => state with
                {
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    }
                },
                Request.HowMuchSpendOnOxen when userInputAsInt < 200 => state with
                {
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        "NOT ENOUGH",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    }
                },
                Request.HowMuchSpendOnOxen when userInputAsInt > 300 => state with
                {
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        "TOO MUCH",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    }
                },
                Request.HowMuchSpendOnOxen => state with
                {
                    Request = Request.HowMuchSpendOnFood,
                    Oxen = userInputAsInt,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON FOOD"
                    }
                },
                Request.HowMuchSpendOnFood when userInputAsInt < 0 => state with
                {
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON FOOD"
                    }
                },
                Request.HowMuchSpendOnFood => state with
                {
                    Food = userInputAsInt,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON AMMUNITION"
                    },
                    Request = Request.HowMuchSpendOnAmmunition
                },
                Request.HowMuchSpendOnAmmunition when userInputAsInt < 0 => state with
                {
                    Request = Request.HowMuchSpendOnAmmunition,
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON AMMUNITION"
                    }
                },
                Request.HowMuchSpendOnAmmunition => state with
                {
                    Ammunition = userInputAsInt,
                    Request = Request.HowMuchSpendOnClothing,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON CLOTHING"
                    }
                },
                Request.HowMuchSpendOnClothing when userInputAsInt < 0 => state with
                {
                    Request = Request.HowMuchSpendOnClothing,
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON CLOTHING"
                    }
                },
                Request.HowMuchSpendOnClothing => state with
                {
                    Clothing = userInputAsInt,
                    Request = Request.HowMuchSpendOnMisc,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON MISCELANEOUS SUPPLIES"
                    }
                },
                Request.HowMuchSpendOnMisc when userInputAsInt < 0 => state with
                {
                    Request = Request.HowMuchSpendOnClothing,
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON MISCELANEOUS SUPPLIES"
                    }
                },
                Request.HowMuchSpendOnMisc when (state.Ammunition + state.Food + userInputAsInt + state.Oxen + state.Clothing) > 700 => state with
                {
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        "YOU OVERSPENT--YOU ONLY HAD $700 TO SPEND.  BUY AGAIN",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    }
                },
                Request.HowMuchSpendOnMisc => state with
                {
                    Request = Request.BeginTurn,
                    MiscellaneousSupplies = userInputAsInt,
                    Money = 700 - (state.Ammunition + state.Food + userInputAsInt + state.Oxen + state.Clothing),
                    Text = new[]
                    {
                        $"AFTER ALL YOUR PURCHASES, YOU NOW HAVE {700 - (state.Ammunition + state.Food + userInputAsInt + state.Oxen + state.Clothing)} DOLLARS LEFT",
                        string.Empty
                    }
                },
                Request.BeginTurn when !state.Fort => state with
                {
                    Request = Request.TurnAction,
                    Text = new[]
                    {
                        new []
                        {
                            "MONDAY MARCH 29 1847",
                            string.Empty,
                        },
                        state.Food < 12 ?
                        new []
                        {
                            "YOU'D BETTER DO SOME HUNTING OR BUY FOOD AND SOON!!!!",
                            string.Empty
                        }
                        : Enumerable.Empty<string>(),
                        new []
                        {
                            "TOTAL MILEAGE IS 0",
                            string.Empty,
                            "FOOD   BULLETS CLOTHING    SUPPLIES    CASH",
                            $"{state.Food}      {state.Ammunition}      {state.Clothing}         {state.MiscellaneousSupplies}       {state.Money}",
                            string.Empty,
                            "DO YOU WANT TO (1) HUNT, OR (2) CONTINUE"
                        }
                    }.SelectMany(x => x)
                },
                Request.TurnAction when userInputAsInt == -1 => state with
                {
                    Request = Request.TurnAction,
                    Text = new[]
                    {
                        "REENTER"
                    }
                },
                Request.TurnAction when !state.Fort && userInput == "1" && state.Ammunition < 39 => state with
                {
                    Request = Request.TurnAction,
                    Text = new[]
                    {
                        "TOUGH---YOU NEED MORE BULLETS TO GO HUNTING",
                        "DO YOU WANT TO (1) HUNT, OR (2) CONTINUE"
                    }
                },
                Request.TurnAction when !state.Fort && userInput == "1" => state with
                {
                    Request = Request.HuntingResult,
                    Text = new[]
                    {
                        "TYPE BANG: "
                    }
                    ,
                    HuntingTimeBegin = this._timeService.GetCurrentTime()
                },
                Request.TurnAction when !state.Fort && userInputAsInt >= 2 => state with
                {
                    Request = Request.HowWellEat,
                    Text = new[]
                    {
                        string.Empty,
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                    }
                },
                Request.HuntingResult => ResolveHunting(state, userInput, this._timeService.GetCurrentTime(), this._rnd)
                    .Map(x =>
                        x.Food < 13
                            ? PlayerHasDied(x, x.Text.Concat( new[] { string.Empty, "YOU RAN OUT OF FOOD AND STARVED TO DEATH" }))
                        : x with
                        {
                            Text = x.Text.Concat(
                                    new[]
                                    {
                                        string.Empty,
                                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                                    }

                                ),
                            Request = Request.HowWellEat,
                            MilesTraveled = x.MilesTraveled - 45

                        }),
                Request.DeadWouldLikeMinister => state with
                {
                    Text = new[]
                    {
                        "WOULD YOU LIKE A FANCY FUNERAL?"
                    },
                    Request = Request.DeadWantFancyFuneral
                },
                Request.DeadWantFancyFuneral => state with
                {
                    Text = new[]
                    {
                        "WOULD YOU LIKE US TO INFORM YOUR NEXT OF KIN?"
                    },
                    Request = Request.DeadInformNextOfkin
                },
                Request.DeadInformNextOfkin => state with
                {
                    Text = (IsYes(userInput) ? new[] { "YOUR AUNT NELLIE IN ST. LOUIS IS ANXIOUS TO HEAR", string.Empty } : Enumerable.Empty<string>())
                    .Concat(
                            new[]
                            {
                                "WE THANK YOU FOR THIS INFORMATION AND WE ARE SORRY YOU",
                                "DIDN'T MAKE IT TO THE GREAT TERRITORY OF OREGON",
                                "BETTER LUCK NEXT TIME",
                                string.Empty,
                                string.Empty,
                                "       SINCERELY",
                                "   THE OREGON CITY CHAMBER OF COMMERCE"
                            }
                        ),
                    IsGameFinished = true,
                    Request = Request.DeadEndGame
                },
                Request.HowWellEat when userInputAsInt is <= 0 or > 3 => state with
                {
                    Text = new[]
                    {
                        "YOU CAN'T EAT THAT WELL",
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY",
                        "OR (3) WELL"
                    }
                },
                Request.HowWellEat => ResolveHowWellEat(state, userInput, userInputAsInt, this._rnd),
                Request.RidersWhatToDo when userInputAsInt is < 1 or > 3 => state with
                {
                    Text = new[]
                    {
                        "(1) RUN  (2) ATTACK  (3) CONTINUE  (4) CIRCLE WAGONS",
                        "IF YOU RUN YOU'LL GAIN TIME BUT WEAR DOWN YOUR OXEN",
                        "IF YOU CIRCLE YOU'LL LOSE TIME"
                    },
                },
                Request.RidersWhatToDo when userInputAsInt == 1 =>
                        (this._rnd.AreRidersActuallyFriendly(state.RidersAreFriendly))
                        //.Map(flipRiderHostility => flipRiderHostility ?  !state.RidersAreFriendly : state.RidersAreFriendly)
                        .Map(reallyHostileOrNot =>
                            state with
                            {
                                Text = new[]
                                {
                                    reallyHostileOrNot ? "RIDERS WERE FRIENDLY, BUT CHECK FOR POSSIBLE LOSSES" : "RIDERS WERE HOSTILE--CHECK FOR LOSSES"
                                },
                                MilesTraveled = state.MilesTraveled + (reallyHostileOrNot ? 15 : 20),
                                MiscellaneousSupplies = state.MiscellaneousSupplies - (reallyHostileOrNot ? 0 : 15),
                                Ammunition = state.Ammunition - (reallyHostileOrNot ? 10 : 150),
                                Oxen = state.Oxen - (reallyHostileOrNot ? 0 : 40),
                                Request = Request.RandomEvent,
                                RidersAreFriendly = reallyHostileOrNot
                            }
                ),
                Request.RidersWhatToDo when userInputAsInt == 2 => state with
                {
                    Request = Request.ShootRiders,
                    Text = new[]
                    {
                        "TYPE BANG: "
                    },
                    HuntingTimeBegin = this._timeService.GetCurrentTime()
                },
                Request.RidersWhatToDo when userInputAsInt == 3 => 
                    (ridersStillAttack: this._rnd.DoRidersStillAttack(), AreRidersFriendly: this._rnd.AreRidersActuallyFriendly(state.RidersAreFriendly))
                        .Map(x =>
                        state with
                        {
                            Ammunition = state.Ammunition - (x.ridersStillAttack ?  150 : 0),
                            Request = Request.RandomEvent,
                            MilesTraveled = state.MilesTraveled - (x.ridersStillAttack ?  15 : 0),
                            Text = new []
                            {
                                x.ridersStillAttack 
                                    ? x.AreRidersFriendly ? "RIDERS WERE FRIENDLY, BUT CHECK FOR POSSIBLE LOSSES" :  "RIDERS WERE HOSTILE--CHECK FOR LOSSES"
                                    : "THEY DID NOT ATTACK",
                                
                            }
                        }),
                Request.ShootRiders => ResolveShootingRiders(state, this._timeService.GetCurrentTime())
            };

            var secondRoundOfUpdates = firstRoundOfUpdates switch
            {
                { Request: Request.RidersAhead } => ResolveRidersAhead(firstRoundOfUpdates, userInput, this._rnd, this.endOfTurnEventHandler),
                { Request: Request.RandomEvent } => this.endOfTurnEventHandler.HandleEndOfTurn(firstRoundOfUpdates, userInput),
                _ => firstRoundOfUpdates
            };


            return secondRoundOfUpdates with { TurnNumber = state.TurnNumber + 1 };
        }

        private static GameState ResolveShootingRiders(GameState oldState, DateTime currentTime)
        {
            var timeTaken = (int)(currentTime - oldState.HuntingTimeBegin).TotalSeconds;
            var successMessage = timeTaken switch
            {
                0 => new[] { "NICE SHOOTING---YOU DROVE THEM OFF" },
                <= 4 => new[] { "KINDA SLOW WITH YOUR COLT .45" },
                _ => new[] { "LOUSY SHOT---YOU GOT KNIFED", "YOU HAVE TO SEE OL' DOC BLANCHARD" }
            };

            var ridersMessage = oldState.RidersAreFriendly
                ? "RIDERS WERE FRIENDLY, BUT CHECK FOR POSSIBLE LOSSES"
                : "RIDERS WERE HOSTILE--CHECK FOR LOSSES";

            var updatedAmmunition = oldState.Ammunition - timeTaken * 40 - 80;

            var returnValue = updatedAmmunition > 0
                    ? oldState with
                    {
                        Ammunition = updatedAmmunition,
                        Text = successMessage.Append(ridersMessage),
                        Injured = timeTaken >= 5,
                        Request = Request.RandomEvent
                    }
                    : PlayerHasDied(oldState, "YOU RAN OUT OF BULLETS AND GOT MASSACRED BY THE RIDERS");

            return returnValue;
        }

        private static GameState PlayerHasDied(GameState oldState, string customDeathMessage) =>
            PlayerHasDied(oldState, new[] { customDeathMessage });


        private static GameState PlayerHasDied(GameState oldState, IEnumerable<string> customDeathMessage) =>
            oldState with
            {

                Text = customDeathMessage.Concat(  new[]
                        {
                            string.Empty,
                            "DO TO YOUR UNFORTUNATE SITUATION, THERE ARE A FEW",
                            "FORMALITIES WE MUST GO THROUGH",
                            string.Empty,
                            "WOULD YOU LIKE A MINISTER?"
                        }),
                Request = Request.DeadWouldLikeMinister
            };

        private static GameState ResolveHunting(GameState oldState, string userInput, DateTime currentTime, IGenerateRandomNumbers rnd) =>
            ((int)(currentTime - oldState.HuntingTimeBegin).TotalSeconds)
            .Map(x => userInput.ToUpper() == "BANG" ? x : 7)
            .Map(x => x < 7 ? x : 7)
            .Map(x =>
                x == 1 ? oldState with
                {
                    Food = oldState.Food + 52 + rnd.BetweenZeroAnd(6),
                    Ammunition = oldState.Ammunition - 10 - rnd.BetweenZeroAnd(4),
                    Text = new[]
                        {
                            "RIGHT BETWEEN THE EYES---YOU GOT A BIG ONE!!!!",  //TODO: BELLS
                        }
                }
                :
                    x * 13 < rnd.BetweenZeroAnd(100)
                        ? oldState with
                        {
                            Text = new[]
                            {
                                "NICE SHOT--RIGHT THROUGH THE NECK--FEAST TONIGHT!!"

                            },
                            Ammunition = oldState.Ammunition - 10 - 3 * x,
                            Food = oldState.Food + 48 - 2 * x
                        }
                        : oldState with
                        {
                            Text = new[]
                            {
                                "SORRY---NO LUCK TODAY"
                            }
                        }
            );

        private static GameState ResolveHowWellEat(GameState oldState, string userInput, int userInputAsInt, IGenerateRandomNumbers rnd)
        {
            var amountOfFoodToEat = 8 + 5 * userInputAsInt;
            var updatedState = oldState.Food > amountOfFoodToEat
                ? oldState with
                {
                    Food = oldState.Food - amountOfFoodToEat,
                    MilesTraveled = oldState.MilesTraveled + 200 + (oldState.Oxen - 220) / 5 + rnd.BetweenZeroAnd(10),
                    Blizzard = false,
                    InsufficientClothing = false,
                    Request = Request.RidersAhead
                }
                : oldState with
                {
                    Text = new[]
                    {
                        "YOU CAN'T EAT THAT WELL",
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY",
                        "OR (3) WELL"
                    }
                };

            return updatedState;
        }

        private static GameState ResolveRidersAhead(GameState state, string userInput, IGenerateRandomNumbers rnd, IHandleEndOfTurnEvents eventHandler)
        {
            var areRidersAhead = rnd.AreRidersAhead(state.MilesTraveled);
            var areRidersFriendly = rnd.AreRidersFriendly();
            return areRidersAhead
                ? state with
                {
                    Text = new[]
                    {
                        new []
                        {
                            "RIDERS AHEAD.  THEY",
                        },
                        new []
                        {
                            areRidersFriendly ? "DON'T" : string.Empty
                        } ,
                        new []
                        {

                            "LOOK HOSTILE",
                            "TACTICS",
                            "(1) RUN  (2) ATTACK  (3) CONTINUE  (4) CIRCLE WAGONS",
                            "IF YOU RUN YOU'LL GAIN TIME BUT WEAR DOWN YOUR OXEN",
                            "IF YOU CIRCLE YOU'LL LOSE TIME"
                        }
                    }.SelectMany(x => x).Where(x => !string.IsNullOrWhiteSpace(x)),
                    Request = Request.RidersWhatToDo,
                    RidersAreFriendly = areRidersFriendly
                }
                : eventHandler.HandleEndOfTurn(state, userInput);
        }

        private static bool IsYes(string s) =>
            new[]
            {
                "y",
                "yes"
            }.Contains(s.ToLower());



    }
}
