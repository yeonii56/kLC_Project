namespace RoomBuildingStarterKit.Common
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// The MinMax data type used to limit value range.
    /// </summary>
    [Serializable]
    public class MinMax
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        public float Min;

        /// <summary>
        /// The maximum value.
        /// </summary>
        public float Max;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMax"/> class.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public MinMax(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Limits value range from minimum to maxmum.
        /// </summary>
        /// <param name="val">The value need to clamp.</param>
        /// <returns>The clamped value.</returns>
        public float Clamp(float val)
        {
            return Mathf.Clamp(val, this.Min, this.Max);
        }

        /// <summary>
        /// Interpolates from minimum to maximum by val.
        /// </summary>
        /// <param name="val">The factor.</param>
        /// <returns>The lerp value.</returns>
        public float Lerp(float val)
        {
            return Mathf.Lerp(this.Min, this.Max, val);
        }
    }
}