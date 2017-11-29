using System;
namespace Arepa.Parser
{
    /// <summary>
    /// Represents a message to be printed
    /// </summary>
    interface IMessage
    {
        /// <summary>
        /// Get/Set the description of the message
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Get/Set Message Type
        /// </summary>
        MessageType TypeMessage { get; set; }
    }
}
