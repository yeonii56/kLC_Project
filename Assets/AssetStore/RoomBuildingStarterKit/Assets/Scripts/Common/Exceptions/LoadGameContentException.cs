namespace RoomBuildingStarterKit.Common
{
    using System;

    /// <summary>
    /// The LoadGameContentException class.
    /// </summary>
    public class LoadGameContentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadGameContentException"/> class.
        /// </summary>
        public LoadGameContentException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadGameContentException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LoadGameContentException(string message) : base(message)
        {
        }
    }
}