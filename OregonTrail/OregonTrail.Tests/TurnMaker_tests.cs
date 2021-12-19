using System;
using FluentAssertions;
using OregonTrail.Entities;
using OregonTrail.Turns;
using Xunit;

namespace OregonTrail.Tests
{
    public class TurnMaker_tests
    {
        [Fact]
        public void given_it_is_the_beginning_of_the_game_then_the_user_is_prompted_to_request_instructions()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {

            }, string.Empty);

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.DoYouRequireInstruction,
                    Text = new[]
                    {
                        "DO YOU NEED INSTRUCTIONS  (YES/NO)"
                    },
                    TurnNumber = 2
                }
            );
        }

        [Theory]
        [InlineData("yes")]
        [InlineData("y")]
        [InlineData("Y")]
        [InlineData("YES")]
        [InlineData("YeS")]
        public void given_the_user_wants_instructions_then_the_instructions_are_displayed(string yesStatement)
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.DoYouRequireInstruction

            }, yesStatement);

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    TurnNumber = 2,
                    IsGameFinished = false,
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
                }
            );
        }

        [Fact]
        public void given_the_user_does_not_want_instructions_then_the_instructions_are_not_displayed()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.DoYouRequireInstruction

            }, "no");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnOxen,
                    TurnNumber = 2,
                    Text = new[]
                    {
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
        }
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_valid_oxen_price_then_the_user_is_prompted_for_a_spend_on_food()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnOxen,
                TurnNumber = 2
            }, "250");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnFood,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON FOOD"
                    },
                    Oxen = 250,
                    TurnNumber = 3
                }
            );
        }

        [Fact]
        public void given_the_user_enters_an_oxen_price_too_low_then_the_user_is_prompted_to_try_again()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnOxen,
                TurnNumber = 2
            }, "50");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        "NOT ENOUGH",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    },
                    TurnNumber = 3
                }
            );
        }

        [Fact]
        public void given_the_user_enters_an_oxen_price_too_high_then_the_user_is_prompted_to_try_again()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnOxen,
                TurnNumber = 2
            }, "500");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        "TOO MUCH",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    },
                    TurnNumber = 3
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_food_amount_that_is_less_than_zero()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnFood,
                TurnNumber = 2
            }, "-4");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnFood,
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON FOOD"
                    },
                    TurnNumber = 3
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_food_amount_that_is_above_zero_then_the_value_is_stored_and_the_user_is_prompted_for_ammo()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnFood,
                TurnNumber = 2,
                Oxen = 500
            }, "250");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnAmmunition,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON AMMUNITION"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250
                }
            );
        }

        [Fact]
        public void given_the_user_enters_an_ammunition_amount_that_is_above_zero_then_the_value_is_stored_and_the_user_is_prompted_for_clothing()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnAmmunition,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250
            }, "750");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnClothing,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON CLOTHING"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Ammunition = 750
                }
            );
        }

        [Fact]
        public void given_the_user_enters_an_ammunition_amount_that_is_below_zero_then_the_user_is_prompted_to_try_again()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnAmmunition,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250
            }, "-5");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnAmmunition,
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON AMMUNITION"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_cothing_amount_that_is_above_zero_then_the_value_is_stored_and_the_user_is_prompted_for_misc()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnClothing,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Ammunition = 750
            }, "1000");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnMisc,
                    Text = new[]
                    {
                        "HOW MUCH DO YOU WANT TO SPEND ON MISCELANEOUS SUPPLIES"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Ammunition = 750,
                    Clothing= 1000
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_clothing_amount_that_is_below_zero_then_the_user_is_prompted_to_try_again()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnClothing,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Ammunition = 750,
            }, "-5");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnClothing,
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON CLOTHING"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Ammunition = 750,
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_misc_amount_that_is_above_zero_and_the_total_spend_is_below_700_then_the_value_is_stored_and_the_user_is_shown_the_start_of_the_game()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnMisc,
                TurnNumber = 2,
                Oxen = 250,
                Food = 100,
                Clothing = 100,
                Ammunition = 100
            }, "100");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.BeginGame,
                    Text = new[]
                    {
                        "AFTER ALL YOUR PURCHASES, YOU NOW HAVE 50 DOLLARS LEFT",
                        string.Empty,
                        "MONDAY MARCH 29 1847"
                    },
                    TurnNumber = 3,
                    Oxen = 250,
                    Food = 100,
                    Ammunition = 100,
                    MiscelaneousSupplies = 100,
                    Clothing = 100,
                    Money = 50
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_misc_amount_that_is_above_zero_and_the_total_spend_is_above_700_then_the_user_is_informed_and_returned_to_the_start_of_purchasing()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnMisc,
                TurnNumber = 2,
                Oxen = 350,
                Food = 100,
                Clothing = 900,
                Ammunition = 100
            }, "100");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnOxen,
                    Text = new[]
                    {
                        "YOU OVERSPENT--YOU ONLY HAD $700 TO SPEND.  BUY AGAIN",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM"
                    },
                    TurnNumber = 3,
                    Oxen = 350,
                    Food = 100,
                    Ammunition = 100,
                    Clothing = 900

                }
            );
        }


        [Fact]
        public void given_the_user_enters_a_misc_amount_that_is_below_zero_then_the_user_is_prompted_to_try_again()
        {
            var turnMaker = new TurnMaker();

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowMuchSpendOnMisc,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Ammunition = 750,
            }, "-5");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowMuchSpendOnClothing,
                    Text = new[]
                    {
                        "IMPOSSIBLE",
                        string.Empty,
                        string.Empty,
                        "HOW MUCH DO YOU WANT TO SPEND ON MISCELANEOUS SUPPLIES"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Ammunition = 750,
                }
            );
        }
    }
}
