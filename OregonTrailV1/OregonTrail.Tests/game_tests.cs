using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using OregonTrail.Display;
using OregonTrail.Entities;
using OregonTrail.Turns;
using OregonTrail.UserInput;
using Xunit;

namespace OregonTrail.Tests
{
    public class game_tests
    {
        [Fact]
        public void given_a_number_of_turns_that_end_with_the_game_finishing_then_the_program_ends()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockTurnMaker = fixture.Freeze<Mock<IMakeTheNextTurn>>();
            mockTurnMaker.SetupSequence(x => x.MakeNextTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns(new GameState
                {
                    IsGameFinished = false
                })
                .Returns(new GameState
                {
                    IsGameFinished = false
                })
                .Returns(new GameState
                {
                    IsGameFinished = true
                });

            var game = fixture.Create<Game>();

            game.StartGame();

            mockTurnMaker.Verify(x => x.MakeNextTurn(It.IsAny<GameState>(), It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void given_a_game_starts_then_the_first_turn_is_1()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockTurnMaker = fixture.Freeze<Mock<IMakeTheNextTurn>>();
            GameState returnedState = null;

            mockTurnMaker.Setup(x => x.MakeNextTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Callback((GameState gs, string _) =>
                {
                    returnedState = gs;
                })
                .Returns((GameState _, string _) => new GameState
                {
                    IsGameFinished = true
                });

            var game = fixture.Create<Game>();

            game.StartGame();

            returnedState.TurnNumber.Should().Be(1);
        }

        [Fact]
        public void given_game_text_in_game_state_then_the_text_is_written()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockTurnMaker = fixture.Freeze<Mock<IMakeTheNextTurn>>();
            GameState returnedState = null;

            var textToDisplay = new[]
            {
                "Line One",
                "Line Two"
            };

            mockTurnMaker.Setup(x => x.MakeNextTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Callback((GameState gs, string _) =>
                {
                    returnedState = gs;
                })
                .Returns((GameState _, string _) => new GameState
                {
                    Text = textToDisplay,
                    IsGameFinished = true
                });

            var mockDisplayText = fixture.Freeze<Mock<IDisplayText>>();

            var game = fixture.Create<Game>();

            game.StartGame();

            mockDisplayText.Verify(x => x.DisplayToUser(textToDisplay));
        }

        [Fact]
        public void given_a_new_user_state_then_the_user_input_is_captured()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockTurnMaker = fixture.Freeze<Mock<IMakeTheNextTurn>>();
            GameState returnedState = null;

            var textToDisplay = new[]
            {
                "Line One",
                "Line Two"
            };

            mockTurnMaker.SetupSequence(x => x.MakeNextTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns(new GameState
                {
                    Text = textToDisplay,
                    IsGameFinished = false
                })
                .Returns(new GameState
                {
                    Text = textToDisplay,
                    IsGameFinished = true
                });

            var mockCaptureInput = fixture.Freeze<Mock<IGetUserInput>>();

            var game = fixture.Create<Game>();

            game.StartGame();

            mockCaptureInput.Verify(x => x.GetInput());
        }

        [Fact] public void given_a_new_user_state_then_the_user_input_is_included_in_the_new_state_call()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var userInput = fixture.Create<string>();

            var mockUserInput = fixture.Freeze<Mock<IGetUserInput>>();
            mockUserInput.Setup(x => x.GetInput()).Returns(userInput);

            var mockTurnMaker = fixture.Freeze<Mock<IMakeTheNextTurn>>();
            GameState returnedState = null;

            var textToDisplay = new[]
            {
                "Line One",
                "Line Two"
            };

            mockTurnMaker.SetupSequence(x => x.MakeNextTurn(It.IsAny<GameState>(), It.IsAny<string>()))
                .Returns(new GameState
                {
                    Text = textToDisplay,
                    IsGameFinished = false
                })
                .Returns(new GameState
                {
                    Text = textToDisplay,
                    IsGameFinished = true
                });

            var game = fixture.Create<Game>();

            game.StartGame();

            mockTurnMaker.Verify(x => x.MakeNextTurn(It.IsAny<GameState>(), userInput));
        }
    }

}