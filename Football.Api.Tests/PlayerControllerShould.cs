using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Api.Controllers;
using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Football.Players.Interfaces;
using Football.Players.Models;
using Football.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Football.Api.Tests
{
    public class PlayerControllerShould
    {
        private readonly AutoMocker _mock;
        private readonly PlayerController _sut;
        private readonly Mock<IPlayersService> _mockPlayersService;


        public PlayerControllerShould()
        {
            _mock = new AutoMocker();
            _mockPlayersService = _mock.GetMock<IPlayersService>();
            _sut = _mock.CreateInstance<PlayerController>();
        }

        [Fact]
        public async Task CreateRookie_InvalidPosition_Returns400BadRequest()
        {
            //Arrange
            var rookieModel = new RookiePlayerModel { Name = "Test Player", Position = "BAD", Active = 1, TeamDrafted = "KC", RookieSeason = 2020, DraftPosition = 1, DeclareAge = 21 };

            //Act
            var actual = await _sut.CreateRookie(rookieModel);
        
            //Assert
            Assert.IsType<BadRequestResult>(actual);        
        }

    }
}
