using Microsoft.Extensions.Logging;
using NSubstitute;
using ReactiveUI.Testing;

namespace BinaryBox.Connection.Test
{
    internal class ConnectionMockFixture : IBuilder
    {
        private IClientStub _client;
        private IConnectionSettings _settings;
        private ILogger _logger;

        public ConnectionMockFixture()
        {
            _client = Substitute.For<IClientStub>();
            _logger = Substitute.For<ILogger>();
            _settings = Substitute.For<IConnectionSettings>();
        }

        public ConnectionMockFixture WithClient(IClientStub clientStub) => this.With(ref _client, clientStub);

        public ConnectionMockFixture WithSettings(IConnectionSettings settings) => this.With(ref _settings, settings);

        public ConnectionMockFixture WithLogger(ILogger logger) => this.With(ref _logger, logger);

        public static implicit operator ConnectionMock(ConnectionMockFixture fixture) => fixture.Build();

        private ConnectionMock Build() => new ConnectionMock(_client, _logger, _settings);
    }
}