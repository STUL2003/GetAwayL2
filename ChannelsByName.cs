using System.Collections.Concurrent;
using System.Threading.Channels;
namespace GetAwayL2
{


    public static class ChannelsByName
    {
        private static readonly ConcurrentDictionary<string, object> _channels = new();

        public static Channel<T> GetOrCreate<T>(string name)
        {
            return (Channel<T>)_channels.GetOrAdd(
                name,
                _ => Channel.CreateUnbounded<T>()
            );
        }
    }

}
