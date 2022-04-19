namespace RoomBuildingStarterKit.BuildSystem
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The Cache class used for cache by frame.
    /// </summary>
    public class FrameCache
    {
        /// <summary>
        /// The last frame count.
        /// </summary>
        private Dictionary<string, int> lastFrameCount = new Dictionary<string, int>();

        /// <summary>
        /// Initializes a new instace of the <see cref="Cache"/> class.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="frameCount">The frame count.</param>
        /// <param name="refresh">The refresh callback.</param>
        public void Cache(string key, int frameCount, Action refresh)
        {
            if(this.lastFrameCount.TryGetValue(key, out int value) == false)
            {
                this.lastFrameCount[key] = 0;
            }

            int currentFrameCount = Time.frameCount;
            int frameDiff = currentFrameCount - this.lastFrameCount[key];

            // Need to refresh.
            if (frameDiff >= frameCount || frameDiff < 0 || this.lastFrameCount[key] == 0)
            {
                refresh();
                this.lastFrameCount[key] = currentFrameCount;
            }
        }
    }

    /// <summary>
    /// The Cache class used for cache by update.
    /// </summary>
    /// <typeparam name="T">The cache value type.</typeparam>
    public class UpdateCache<T>
    {
        /// <summary>
        /// The old value.
        /// </summary>
        private Dictionary<string, T> oldValue = new Dictionary<string, T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="refresh">The refresh callback</param>
        public void Cache(string key, T newValue, Action refresh)
        {
            if (this.oldValue.TryGetValue(key, out T value) == false || !oldValue[key].Equals(newValue))
            {
                refresh();
                oldValue[key] = newValue;
            }
        }
    }
}
