namespace RoomBuildingStarterKit.Common
{
    using System;

    /// <summary>
    /// The LoadGameContentException class.
    /// </summary>
    public class SaveGameContentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadGameContentException"/> class.
        /// </summary>
        public SaveGameContentException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadGameContentException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SaveGameContentException(string message) : base(message)
        {
        }
    }
}