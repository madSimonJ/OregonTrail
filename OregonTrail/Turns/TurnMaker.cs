using OregonTrail.Entities;

namespace OregonTrail.Turns
{


    public class TurnMaker : IMakeTheNextTurn
    {
        public GameState MakeNextTurn(GameState state, string userInput)
        {
            var userInputAsInt = int.TryParse(userInput, out var result) ? result : -1;

            var returnValue = state.Request switch
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
                    }                },
                Request.DoYouRequireInstruction => state with
                {
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    }                },
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
                    }                },
                Request.HowMuchSpendOnFood when userInputAsInt < 0 => state with 
                { 
                    Text = new [] 
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
                    Text = new []
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON AMMUNITION"
                    },
                    Request = Request.HowMuchSpendOnAmmunition
                },
                Request.HowMuchSpendOnAmmunition when userInputAsInt < 0 => state with
                {
                    Request = Request.HowMuchSpendOnAmmunition,
                    Text = new []
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
                    Text = new []
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
                    Request = Request.BeginGame,
                    MiscellaneousSupplies = userInputAsInt,
                    Money = 700 - (state.Ammunition + state.Food + userInputAsInt + state.Oxen + state.Clothing),
                    Text = new[]
                    {
                        $"AFTER ALL YOUR PURCHASES, YOU NOW HAVE {700 - (state.Ammunition + state.Food + userInputAsInt + state.Oxen + state.Clothing)} DOLLARS LEFT",
                        string.Empty,
                        "MONDAY MARCH 29 1847"
                    }
                }
            };

            return returnValue with { TurnNumber = state.TurnNumber + 1};
        }

        private static bool IsYes(string s) =>
            new[]
            {
                "y",
                "yes"
            }.Contains(s.ToLower());



    }
}
