using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.Constants;

namespace FishNetTypes.Transporting
{
    public static class ChannelExtensions
    {
        /// <summary>
        /// Returns a Channel enum value while containing the enum name (eg: Channel.Reliable).
        /// </summary>
        public static string GetEnumName(this byte channelValue)
        {
            Channel channel = (Channel)channelValue;
            return channel.GetEnumName();
        }
        
        /// <summary>
        /// Returns a Channel enum value while containing the enum name (eg: Channel.Reliable).
        /// </summary>
        public static string GetEnumName(this Channel channel) => $"{FishNetConstants.Channel_FullName}.{channel.ToString()}";
    }

    
    /// <summary>
    /// Channel which data is sent or received.
    /// </summary>
    public enum Channel : byte
    {
        /// <summary>
        /// Data will be sent ordered reliable.
        /// </summary>
        Reliable = 0,
        /// <summary>
        /// Data will be sent unreliable.
        /// </summary>
        Unreliable = 1
    }


}