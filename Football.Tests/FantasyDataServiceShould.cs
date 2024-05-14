using Moq;
using Moq.AutoMock;
using AutoMapper;
using Xunit;
using Football.Models;
using Football.Fantasy;
using Football.Fantasy.Services;

namespace Football.Tests
{
    public class FantasyDataServiceShould
    {
        private readonly AutoMocker _mock;

        private readonly FantasyDataService _sut;

        public FantasyDataServiceShould()
        {
            _mock = new AutoMocker();
        }
    }
}
