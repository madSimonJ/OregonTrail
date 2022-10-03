using System;
using System.Linq;
using FluentAssertions;
using Moq;
using OregonTrail.Entities;
using OregonTrail.Random;
using OregonTrail.TimeService;
using OregonTrail.Turns;
using Xunit;

namespace OregonTrail.Tests
{
    public class TurnMaker_tests
    {
        [Fact]
        public void given_it_is_the_beginning_of_the_game_then_the_user_is_prompted_to_request_instructions()
        {
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
                    Request = Request.BeginTurn,
                    Text = new[]
                    {
                        "AFTER ALL YOUR PURCHASES, YOU NOW HAVE 50 DOLLARS LEFT",
                        string.Empty
                    },
                    TurnNumber = 3,
                    Oxen = 250,
                    Food = 100,
                    Ammunition = 100,
                    MiscellaneousSupplies = 100,
                    Clothing = 100,
                    Money = 50
                }
            );
        }

        [Fact]
        public void given_the_user_enters_a_misc_amount_that_is_above_zero_and_the_total_spend_is_above_700_then_the_user_is_informed_and_returned_to_the_start_of_purchasing()
        {
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

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

        [Fact]
        public void given_the_user_is_on_turn_one_then_they_are_shown_the_available_actions_and_prompted_for_a_choice_of_action()
        {
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.BeginTurn,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50
            }, "-5");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.TurnAction,
                    Text = new[]
                    {
                        "MONDAY MARCH 29 1847",
                        string.Empty,
                        "TOTAL MILEAGE IS 0",
                        string.Empty,
                        "FOOD   BULLETS CLOTHING    SUPPLIES    CASH",
                        "250      750      666         616       50",
                        string.Empty,
                        "DO YOU WANT TO (1) HUNT, OR (2) CONTINUE"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50
                }
            );
        }

        [Fact]
        public void given_the_user_is_on_turn_one_and_they_have_less_than_12_food_then_they_are_warned_to_go_hunting_soon()
        {
            var turnMaker = new TurnMaker(Mock.Of<ITimeService>(), Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.BeginTurn,
                TurnNumber = 2,
                Oxen = 500,
                Food = 5,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50
            }, "-5");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.TurnAction,
                    Text = new[]
                    {
                        "MONDAY MARCH 29 1847",
                        string.Empty,
                        "YOU'D BETTER DO SOME HUNTING OR BUY FOOD AND SOON!!!!",
                        string.Empty,
                        "TOTAL MILEAGE IS 0",
                        string.Empty,
                        "FOOD   BULLETS CLOTHING    SUPPLIES    CASH",
                        "5      750      666         616       50",
                        string.Empty,
                        "DO YOU WANT TO (1) HUNT, OR (2) CONTINUE"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 5,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50
                }
            );
        }

        [Fact]
        public void given_the_user_is_on_a_non_fort_turn_and_they_choose_option_1_then_they_are_prompted_to_type_bang_to_try_hunting()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.TurnAction,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HuntingResult,
                    Text = new[]
                    {
                        "TYPE BANG: "
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0)
                }
            );
        }

        [Fact]
        public void given_the_user_is_on_a_non_fort_turn_and_they_choose_option_2_then_they_are_prompted_to_choose_eating()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.TurnAction,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "2");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowWellEat,
                    Text = new[]
                    {
                        string.Empty,
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false
                }
            );
        }

        [Fact]
        public void given_the_user_is_on_a_non_fort_turn_and_they_choose_a_non_integer_option_then_they_are_prompted_to_reenter()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.TurnAction,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "z");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.TurnAction,
                    Text = new[]
                    {
                        "REENTER"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false
                }
            );
        }

        [Fact]
        public void given_the_user_is_on_a_non_fort_turn_and_they_choose_1_but_have_fewer_than_39_bullets_then_they_are_prompted_to_choose_again()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.TurnAction,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 38,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.TurnAction,
                    Text = new[]
                    {
                        "TOUGH---YOU NEED MORE BULLETS TO GO HUNTING",
                        "DO YOU WANT TO (1) HUNT, OR (2) CONTINUE"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 38,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_enter_bang_in_less_than_7_seconds_and_a_large_random_number_is_generated_then_they_lose_bullets_and_gain_food_and_do_fewer_miles_of_travel()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 5));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(100)).Returns(99);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BANG");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowWellEat,
                    Text = new[]
                    {
                        "NICE SHOT--RIGHT THROUGH THE NECK--FEAST TONIGHT!!",
                        string.Empty,
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 288,
                    Clothing = 666,
                    Ammunition = 725,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = -45
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_enter_bang_in_less_than_7_seconds_and_a_small_random_number_is_generated_then_gain_no_food_and_do_fewer_miles_of_travel()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 5));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(100)).Returns(2);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BANG");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowWellEat,
                    Text = new[]
                    {
                        "SORRY---NO LUCK TODAY",
                        string.Empty,
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = -45
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_enter_bang_in_less_than_1_seconds_then_they_gain_extra_food_and_lose_fewer_bullets_and_do_fewer_miles_of_travel()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(6)).Returns(5);
            mockRnd.Setup(x => x.BetweenZeroAnd(4)).Returns(3);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BANG");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowWellEat,
                    Text = new[]
                    {
                        "RIGHT BETWEEN THE EYES---YOU GOT A BIG ONE!!!!",  //TODO: BELLS
                        string.Empty,
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 307,
                    Clothing = 666,
                    Ammunition = 737,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = -45
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_misspell_bang_in_less_than_7_seconds_then_it_is_treated_as_7_seconds()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 5));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(100)).Returns(91);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BAGN");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowWellEat,
                    Text = new[]
                    {
                        "SORRY---NO LUCK TODAY",
                        string.Empty,
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = -45
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_type_bang_in_more_than_7_seconds_then_it_is_treated_as_7_seconds()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 59));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(100)).Returns(92);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BANG");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.HowWellEat,
                    Text = new[]
                    {
                        "NICE SHOT--RIGHT THROUGH THE NECK--FEAST TONIGHT!!",
                        string.Empty,
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY OR (3) WELL"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 284,
                    Clothing = 666,
                    Ammunition = 719,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = -45
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_enter_bang_in_less_than_1_seconds_and_they_end_up_with_less_than_13_food_then_they_starve_to_death()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(6)).Returns(5);
            mockRnd.Setup(x => x.BetweenZeroAnd(4)).Returns(3);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = -100,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BANG");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.DeadWouldLikeMinister,
                    Text = new[]
                    {
                        "RIGHT BETWEEN THE EYES---YOU GOT A BIG ONE!!!!",  //TODO: BELLS
                        string.Empty,
                        "YOU RAN OUT OF FOOD AND STARVED TO DEATH",
                        string.Empty,
                        "DO TO YOUR UNFORTUNATE SITUATION, THERE ARE A FEW",
                        "FORMALITIES WE MUST GO THROUGH",
                        string.Empty,
                        "WOULD YOU LIKE A MINISTER?"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = -43,
                    Clothing = 666,
                    Ammunition = 737,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = 0
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_enter_bang_in_less_than_7_seconds_and_a_large_random_number_is_generated_and_the_player_ends_up_with_is_less_than_13_then_they_starve_to_death()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 5));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(100)).Returns(99);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = -100,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BANG");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.DeadWouldLikeMinister,
                    Text = new[]
                    {
                        "NICE SHOT--RIGHT THROUGH THE NECK--FEAST TONIGHT!!",
                        string.Empty,
                        "YOU RAN OUT OF FOOD AND STARVED TO DEATH",
                        string.Empty,
                        "DO TO YOUR UNFORTUNATE SITUATION, THERE ARE A FEW",
                        "FORMALITIES WE MUST GO THROUGH",
                        string.Empty,
                        "WOULD YOU LIKE A MINISTER?"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = -62,
                    Clothing = 666,
                    Ammunition = 725,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = 0
                }
            );
        }

        [Fact]
        public void given_the_user_is_hunting_and_they_enter_bang_in_less_than_7_seconds_and_a_small_random_number_is_generated_and_the_player_has_less_than_13_food_then_they_starve_to_death()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 5));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(100)).Returns(2);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HuntingResult,
                TurnNumber = 2,
                Oxen = 500,
                Food = 2,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "BANG");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.DeadWouldLikeMinister,
                    Text = new[]
                    {
                        "SORRY---NO LUCK TODAY",
                        string.Empty,
                        "YOU RAN OUT OF FOOD AND STARVED TO DEATH",
                        string.Empty,
                        "DO TO YOUR UNFORTUNATE SITUATION, THERE ARE A FEW",
                        "FORMALITIES WE MUST GO THROUGH",
                        string.Empty,
                        "WOULD YOU LIKE A MINISTER?"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 2,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = 0
                }
            );
        }

        [Fact]
        public void given_the_user_is_dead_and_has_been_prompted_for_a_minister_then_whatever_they_type_results_in_being_asked_about_funeral()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.DeadWouldLikeMinister,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "z");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.DeadWantFancyFuneral,
                    Text = new[]
                    {
                        "WOULD YOU LIKE A FANCY FUNERAL?"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false
                }
            );
        }

        [Fact]
        public void given_the_user_is_dead_and_has_been_prompted_for_a_fancy_funeral_then_whatever_they_type_results_in_being_asked_about_next_of_kin()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.DeadWantFancyFuneral,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "z");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = false,
                    Request = Request.DeadInformNextOfkin,
                    Text = new[]
                    {
                        "WOULD YOU LIKE US TO INFORM YOUR NEXT OF KIN?"
                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false
                }
            );
        }

        [Fact]
        public void given_the_user_is_dead_and_has_been_prompted_for_next_of_kin_and_they_answer_yes_then_aunt_nellie_is_informed_and_the_game_ends()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.DeadInformNextOfkin,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "yes");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = true,
                    Request = Request.DeadEndGame,
                    Text = new[]
                    {
                        "YOUR AUNT NELLIE IN ST. LOUIS IS ANXIOUS TO HEAR",
                        string.Empty,
                        "WE THANK YOU FOR THIS INFORMATION AND WE ARE SORRY YOU",
                        "DIDN'T MAKE IT TO THE GREAT TERRITORY OF OREGON",
                        "BETTER LUCK NEXT TIME",
                        string.Empty,
                        string.Empty,
                        "       SINCERELY",
                        "   THE OREGON CITY CHAMBER OF COMMERCE"

                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false
                }
            );
        }

        [Fact]
        public void given_the_user_is_dead_and_has_been_prompted_for_next_of_kin_and_they_answer_no_then_aunt_nellie_is_not_informed_and_the_game_ends()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 0));

            var turnMaker = new TurnMaker(mockTimeService.Object, Mock.Of<IGenerateRandomNumbers>(), Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.DeadInformNextOfkin,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                Fort = false
            }, "no");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    IsGameFinished = true,
                    Request = Request.DeadEndGame,
                    Text = new[]
                    {
                        "WE THANK YOU FOR THIS INFORMATION AND WE ARE SORRY YOU",
                        "DIDN'T MAKE IT TO THE GREAT TERRITORY OF OREGON",
                        "BETTER LUCK NEXT TIME",
                        string.Empty,
                        string.Empty,
                        "       SINCERELY",
                        "   THE OREGON CITY CHAMBER OF COMMERCE"

                    },
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false
                }
            );
        }

        [Theory]
        [InlineData("0")]
        [InlineData("z")]
        [InlineData("one")]
        [InlineData("")]
        public void given_the_user_is_prompted_for_food_and_they_choose_an_invalid_option_then_they_are_reprompted(string input)
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(6)).Returns(5);
            mockRnd.Setup(x => x.BetweenZeroAnd(4)).Returns(3);

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowWellEat,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false,
                MilesTraveled = 100
            }, input);

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.HowWellEat,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    MilesTraveled = 100,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    Fort = false,
                    Text = new []
                    {
                        "YOU CAN'T EAT THAT WELL",
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY",
                        "OR (3) WELL"
                    }
                }
            );
        }

        [Fact]
        public void given_the_user_is_prompted_for_food_and_they_choose_an_option_that_would_mean_eating_more_food_than_they_have_then_they_are_prompted_to_choose_again()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(2);
            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, Mock.Of<IHandleEndOfTurnEvents>());

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowWellEat,
                TurnNumber = 2,
                Oxen = 500,
                Food = 10,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                MilesTraveled = 100,
                Money = 50,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.HowWellEat,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 10,
                    Clothing = 666,
                    MilesTraveled = 100,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    Fort = false,
                    Text = new[]
                    {
                        "YOU CAN'T EAT THAT WELL",
                        "DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY",
                        "OR (3) WELL"
                }
                }
            );
        }

        [Fact]
        public void given_the_user_is_prompted_for_food_and_they_choose_option_one_and_there_are_no_riders_then_they_eat_poorly_and_milage_is_updated_and_end_of_turn_events_are_triggered()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(2);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) =>
                {
                    return gs with
                    {
                        Text = new[]
                        {
                            "random event"
                        },
                        Request = Request.RandomEvent
                    };
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowWellEat,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = true,
                InsufficientClothing = true,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250 - 13,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    MilesTraveled = 100 + 258,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    Fort = false,
                    Text = new []
                    {
                        "random event"
                    }
                }
            );
        }

        [Fact]
        public void given_the_user_is_prompted_for_food_and_they_choose_option_two_and_there_are_no_riders__then_they_eat_moderately_and_milage_is_updated_and_end_of_turn_events_are_triggered()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(2);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) =>
                {
                    return gs with
                    {
                        Text = new[]
                        {
                            "random event"
                        },
                        Request = Request.RandomEvent
                    };
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowWellEat,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                MilesTraveled = 100,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "2");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250 - 18,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    MilesTraveled = 100 + 258,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    Fort = false,
                    Text = new[]
                    {
                        "random event"
                    }
                }
            );
        }

        [Fact]
        public void given_the_user_is_prompted_for_food_and_they_choose_option_three_and_there_are_no_riders__then_they_eat_well_and_milage_is_updated_and_end_of_turn_events_are_triggered()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.SetupSequence(x => x.BetweenZeroAnd(10))
                .Returns(2)
                .Returns(10);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) =>
                {
                    return gs with
                    {
                        Text = new[]
                        {
                            "random event"
                        },
                        Request = Request.RandomEvent
                    };
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);

            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowWellEat,
                TurnNumber = 2,
                Oxen = 500,
                MilesTraveled = 100,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                Fort = false
            }, "3");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250 - 23,
                    Clothing = 666,
                    Ammunition = 750,
                    MilesTraveled = 100 + 258,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    Fort = false,
                    Text = new[]
                    {
                        "random event"
                    }
                }
            );
        }

        [Fact]
        public void given_the_user_is_prompted_for_food_and_they_choose_option_one_and_there_are_unfriendly_riders_then_they_eat_poorly_and_milage_is_updated_and_they_are_prompted_to_deal_with_the_riders()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.AreRidersActuallyFriendly(It.IsAny<bool>())).Returns(false);
            mockRnd.Setup(x => x.AreRidersAhead(It.IsAny<int>())).Returns(true);

            mockRnd.SetupSequence(x => x.BetweenZeroAnd(10))
                .Returns(2)
                .Returns(1)
                .Returns(10);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) =>
                {
                    return gs with
                    {
                        Text = new[]
                        {
                            "random event"
                        },
                        Request = Request.RandomEvent
                    };
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowWellEat,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = true,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = true,
                InsufficientClothing = true,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RidersWhatToDo,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250 - 13,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    MilesTraveled = 100 + 258,
                    RidersAreFriendly = false,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    Fort = false,
                    Text = new[]
                    {
                        "RIDERS AHEAD.  THEY",
                        "LOOK HOSTILE",
                        "TACTICS",
                        "(1) RUN  (2) ATTACK  (3) CONTINUE  (4) CIRCLE WAGONS",
                        "IF YOU RUN YOU'LL GAIN TIME BUT WEAR DOWN YOUR OXEN",
                        "IF YOU CIRCLE YOU'LL LOSE TIME"
                    }
                }
            );
        }

        [Fact]
        public void given_the_user_is_prompted_for_food_and_they_choose_option_one_and_there_are_friendly_riders_then_they_eat_poorly_and_milage_is_updated_and_they_are_prompted_to_deal_with_the_riders()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1982, 8, 18, 11, 24, 1));

            var mockRnd = new Mock<IGenerateRandomNumbers>();

            mockRnd.Setup(x => x.AreRidersFriendly()).Returns(true);
            mockRnd.Setup(x => x.AreRidersAhead(It.IsAny<int>())).Returns(true);

            mockRnd.SetupSequence(x => x.BetweenZeroAnd(10))
                .Returns(2)
                .Returns(1);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) =>
                {
                    return gs with
                    {
                        Text = new[]
                        {
                            "random event"
                        },
                        Request = Request.RandomEvent
                    };
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.HowWellEat,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = true,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = true,
                InsufficientClothing = true,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RidersWhatToDo,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250 - 13,
                    Clothing = 666,
                    Ammunition = 750,
                    MiscellaneousSupplies = 616,
                    MilesTraveled = 100 + 258,
                    RidersAreFriendly = true,
                    DateCounter = 1,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    Fort = false,
                    Text = new[]
                    {
                        "RIDERS AHEAD.  THEY",
                        "DON'T",
                        "LOOK HOSTILE",
                        "TACTICS",
                        "(1) RUN  (2) ATTACK  (3) CONTINUE  (4) CIRCLE WAGONS",
                        "IF YOU RUN YOU'LL GAIN TIME BUT WEAR DOWN YOUR OXEN",
                        "IF YOU CIRCLE YOU'LL LOSE TIME"
                    }
                }
            );
        }


        [Theory]
        [InlineData("0")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("z")]
        public void given_the_player_has_encountered_riders_and_they_enter_an_invalid_value_then_they_are_reprompted_to_chooose_again(string userInput)
        {
            var mockTimeService = new Mock<ITimeService>();

            var mockRnd = new Mock<IGenerateRandomNumbers>();

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.RidersWhatToDo,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = true,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, userInput);

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RidersWhatToDo,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    RidersAreFriendly = true,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = 100,
                    Fort = false,
                    Text = new[]
                    {
                        "(1) RUN  (2) ATTACK  (3) CONTINUE  (4) CIRCLE WAGONS",
                        "IF YOU RUN YOU'LL GAIN TIME BUT WEAR DOWN YOUR OXEN",
                        "IF YOU CIRCLE YOU'LL LOSE TIME"
                    }
                }
            );
        }


        [Fact]
        public void given_the_player_has_encountered_unfriendly_riders_and_they_choose_to_run_then_they_escape_but_the_oxen_are_worn_down_and_a_random_event_occurs()
        {
            var mockTimeService = new Mock<ITimeService>();

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(9);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text =gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.RidersWhatToDo,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = 460,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 600,
                    RidersAreFriendly = false,
                    MiscellaneousSupplies = 601,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = 120,
                    Fort = false,
                    Text = new[]
                    {
                        "RIDERS WERE HOSTILE--CHECK FOR LOSSES",
                        "random event"
                    }
                }
            );
        }

        //[Theory]
        //[InlineData(0, true)]
        //[InlineData(1, true)]
        //[InlineData(2, false)]
        //[InlineData(3, false)]
        //[InlineData(4, false)]
        //[InlineData(5, false)]
        //[InlineData(6, false)]
        //[InlineData(7, false)]
        //[InlineData(8, false)]
        //[InlineData(9, false)]
        public void given_the_player_has_encountered_unfriendly_riders_and_they_choose_to_run_then_there_is_a_chance_they_might_actually_be_friendly(int rnd, bool ridersAreFriendly)
        {
            var mockTimeService = new Mock<ITimeService>();

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(rnd);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.RidersWhatToDo,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = ridersAreFriendly ? 500 : 460,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = ridersAreFriendly ? 740 : 600,
                    RidersAreFriendly = ridersAreFriendly,
                    MiscellaneousSupplies = ridersAreFriendly ? 616 : 601,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = ridersAreFriendly ? 115 : 120,
                    Fort = false,
                    Text = new[]
                    {
                        ridersAreFriendly ? "RIDERS WERE FRIENDLY, BUT CHECK FOR POSSIBLE LOSSES" : "RIDERS WERE HOSTILE--CHECK FOR LOSSES",
                        "random event"
                    }
                }
            );
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void given_the_player_has_encountered_friendly_riders_and_they_choose_to_run_then_there_is_a_chance_they_might_actually_be_unfriendly(bool ridersAreFriendly)
        {
            var mockTimeService = new Mock<ITimeService>();

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.AreRidersActuallyFriendly(true)).Returns(ridersAreFriendly);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.RidersWhatToDo,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = true,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "1");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = ridersAreFriendly ? 500 : 460,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = ridersAreFriendly ? 740 : 600,
                    RidersAreFriendly = ridersAreFriendly,
                    MiscellaneousSupplies = ridersAreFriendly ? 616 : 601,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = ridersAreFriendly ? 115 : 120,
                    Fort = false,
                    Text = new[]
                    {
                        ridersAreFriendly ? "RIDERS WERE FRIENDLY, BUT CHECK FOR POSSIBLE LOSSES" : "RIDERS WERE HOSTILE--CHECK FOR LOSSES",
                        "random event"
                    }
                }
            );
        }



        [Fact]
        public void given_the_player_has_encountered_unfriendly_riders_and_they_choose_to_fight_they_are_prompted_to_type_bang()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1984, 4, 17, 12, 0, 0, 0));

            var mockRnd = new Mock<IGenerateRandomNumbers>();

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.RidersWhatToDo,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = true,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = DateTime.MinValue,
                MilesTraveled = 100,
                Fort = false
            }, "2");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.ShootRiders,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750,
                    RidersAreFriendly = true,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                    MilesTraveled = 100,
                    Fort = false,
                    Text = new[]
                    {
                        "TYPE BANG: "
                    }
                }
            );
        }

        [Fact]
        public void given_the_player_has_shot_the_encountered_unfriendly_riders_in_less_than_a_second_then_they_are_congratulated()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1984, 4, 17, 12, 0, 0, 0));

            var mockRnd = new Mock<IGenerateRandomNumbers>();

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.ShootRiders,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                MilesTraveled = 100,
                Fort = false
            }, "2");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 670,
                    RidersAreFriendly = false,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                    MilesTraveled = 100,
                    Fort = false,
                    Text = new[]
                    {
                        "NICE SHOOTING---YOU DROVE THEM OFF",
                        "RIDERS WERE HOSTILE--CHECK FOR LOSSES",
                        "random event"
                    }
                }
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void given_the_player_has_shot_the_encountered_unfriendly_riders_in_less_than_5_seconds_then_they_are_congratulated(int numSeconds)
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1984, 4, 17, 12, 0, numSeconds, 0));

            var mockRnd = new Mock<IGenerateRandomNumbers>();

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.ShootRiders,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                MilesTraveled = 100,
                Fort = false
            }, "2");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750 - numSeconds * 40 - 80,
                    RidersAreFriendly = false,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                    MilesTraveled = 100,
                    Fort = false,
                    Text = new[]
                    {
                        "KINDA SLOW WITH YOUR COLT .45",
                        "RIDERS WERE HOSTILE--CHECK FOR LOSSES",
                        "random event"
                    }
                }
            );
        }


        [Fact]
        public void given_the_player_has_shot_the_encountered_unfriendly_riders_in_5_seconds_or_more_then_they_are_wounded()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1984, 4, 17, 12, 0, 5, 0));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(9);
            
            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.ShootRiders,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                MilesTraveled = 100,
                Fort = false
            }, "2");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 750 - 5 * 40 - 80,
                    RidersAreFriendly = false,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                    MilesTraveled = 100,
                    Fort = false,
                    Injured = true,
                    Text = new[]
                    {
                        "LOUSY SHOT---YOU GOT KNIFED",
                        "YOU HAVE TO SEE OL' DOC BLANCHARD",
                        "RIDERS WERE HOSTILE--CHECK FOR LOSSES",
                        "random event"
                    }
                }
            );
        }


        //[Theory]
        //[InlineData(0, true)]
        //[InlineData(1, true)]
        //[InlineData(2, true)]
        //[InlineData(3, true)]
        //[InlineData(4, true)]
        //[InlineData(5, true)]
        //[InlineData(6, true)]
        //[InlineData(7, true)]
        //[InlineData(8, true)]
        //[InlineData(9, false)]
        public void given_the_player_has_encountered_unfriendly_riders_and_they_choose_to_continue_then_there_is_small_chance_the_riders_do_not_attack(int rnd, bool ridersStillAttack)
        {
            var mockTimeService = new Mock<ITimeService>();

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(rnd);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.RidersWhatToDo,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "3");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = ridersStillAttack ? 500 : 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = ridersStillAttack ? 750 - 150: 750,
                    RidersAreFriendly = false,
                    MiscellaneousSupplies = ridersStillAttack ? 616 : 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = ridersStillAttack ? 100 - 15 : 100,
                    Fort = false,
                    Text = new[]
                    {
                        ridersStillAttack ?  "RIDERS WERE HOSTILE--CHECK FOR LOSSES" : "THEY DID NOT ATTACK",
                        "random event"
                    }
                }
            );
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void given_the_player_has_encountered_friendly_riders_and_they_choose_to_continue_then_there_is_small_chance_the_riders_do_not_attack(bool ridersStillAttack)
        {
            var mockTimeService = new Mock<ITimeService>();

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.DoRidersStillAttack()).Returns(ridersStillAttack);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.RidersWhatToDo,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 750,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                MilesTraveled = 100,
                Fort = false
            }, "3");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.RandomEvent,
                    TurnNumber = 3,
                    Oxen = ridersStillAttack ? 500 : 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = ridersStillAttack ? 750 - 150 : 750,
                    RidersAreFriendly = false,
                    MiscellaneousSupplies = ridersStillAttack ? 616 : 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1982, 8, 18, 11, 24, 0),
                    MilesTraveled = ridersStillAttack ? 100 - 15 : 100,
                    Fort = false,
                    Text = new[]
                    {
                        ridersStillAttack ?  "RIDERS WERE HOSTILE--CHECK FOR LOSSES" : "THEY DID NOT ATTACK",
                        "random event"
                    }
                }
            );
        }

        [Fact]
        public void given_the_player_has_shot_the_encountered_unfriendly_riders_and_they_run_out_of_bullets_then_they_are_killed()
        {
            var mockTimeService = new Mock<ITimeService>();
            mockTimeService.Setup(x => x.GetCurrentTime())
                .Returns(new DateTime(1984, 4, 17, 12, 0, 5, 0));

            var mockRnd = new Mock<IGenerateRandomNumbers>();
            mockRnd.Setup(x => x.BetweenZeroAnd(10)).Returns(9);

            var mockEventHandler = new Mock<IHandleEndOfTurnEvents>();
            mockEventHandler.Setup(x => x.HandleEndOfTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns((GameState gs, string s) => gs with
                {
                    Text = gs.Text.Append("random event"),
                    Request = Request.RandomEvent
                });

            var turnMaker = new TurnMaker(mockTimeService.Object, mockRnd.Object, mockEventHandler.Object);


            var newTurn = turnMaker.MakeNextTurn(new GameState
            {
                Request = Request.ShootRiders,
                TurnNumber = 2,
                Oxen = 500,
                Food = 250,
                Clothing = 666,
                Ammunition = 10,
                RidersAreFriendly = false,
                MiscellaneousSupplies = 616,
                DateCounter = 1,
                Blizzard = false,
                InsufficientClothing = false,
                Money = 50,
                HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                MilesTraveled = 100,
                Fort = false
            }, "2");

            newTurn.Should().BeEquivalentTo(
                new GameState
                {
                    Request = Request.DeadWouldLikeMinister,
                    TurnNumber = 3,
                    Oxen = 500,
                    Food = 250,
                    Clothing = 666,
                    Ammunition = 10,
                    RidersAreFriendly = false,
                    MiscellaneousSupplies = 616,
                    DateCounter = 1,
                    Blizzard = false,
                    InsufficientClothing = false,
                    Money = 50,
                    HuntingTimeBegin = new DateTime(1984, 4, 17, 12, 0, 0, 0),
                    MilesTraveled = 100,
                    Fort = false,
                    Injured = false,
                    Text = new[]
                    {
                        "YOU RAN OUT OF BULLETS AND GOT MASSACRED BY THE RIDERS",
                        string.Empty,
                        "DO TO YOUR UNFORTUNATE SITUATION, THERE ARE A FEW",
                        "FORMALITIES WE MUST GO THROUGH",
                        string.Empty,
                        "WOULD YOU LIKE A MINISTER?"
                    }
                }
            );
        }

    }
}
