using System;
using System.Collections.Generic;
using FakeItEasy;
using MPC_HC.Domain.Interfaces;
using MPC_HC.Domain.Services;
using Xunit;

namespace MPC_HC.Test
{
    public class CommandServiceTest
    {
        private readonly Uri _uri;

        public CommandServiceTest()
        {
            _uri = new Uri("http://localhost:13579/controls.html");
        }

        [Fact]
        public void SetSoundThrowsArgumentOutOfRangeException()
        {
            var fejkRequest = A.Fake<IRequestService>();
            A.CallTo(() => fejkRequest.ExcutePostRequest("",new []{new KeyValuePair<string, string>()})).Returns("");
            
            var soundService = new CommandService(fejkRequest);
            Assert.Throws<ArgumentOutOfRangeException>(() => soundService.SetSoundLevel(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => soundService.SetSoundLevel(-10));
            Assert.Throws<ArgumentOutOfRangeException>(() => soundService.SetSoundLevel(-54));
            Assert.Throws<ArgumentOutOfRangeException>(() => soundService.SetSoundLevel(1000));
            Assert.Throws<ArgumentOutOfRangeException>(() => soundService.SetSoundLevel(120));
            Assert.Throws<ArgumentOutOfRangeException>(() => soundService.SetSoundLevel(101));
        }
    }
}