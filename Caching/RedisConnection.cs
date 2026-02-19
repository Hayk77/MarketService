using StackExchange.Redis;

namespace MyProject.Caching
{
    public static class RedisConnection
    {
        private static readonly Lazy<ConnectionMultiplexer> _connection =
            new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect("localhost:6379"));

        public static ConnectionMultiplexer Instance => _connection.Value;
    }
}
