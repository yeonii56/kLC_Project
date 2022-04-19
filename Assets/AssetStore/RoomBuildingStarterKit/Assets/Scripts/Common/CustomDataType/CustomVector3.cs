namespace RoomBuildingStarterKit.Common
{
    using UnityEngine;
    
    /// <summary>
    /// The custom vector3 for serialize.
    /// </summary>
    public struct CustomVector3
    {
        /// <summary>
        /// The x dimension.
        /// </summary>
        public float x;

        /// <summary>
        /// The y dimension.
        /// </summary>
        public float y;

        /// <summary>
        /// The z dimension.
        /// </summary>
        public float z;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomVector3"/> struct with dimensions.
        /// </summary>
        /// <param name="x">The x dimension.</param>
        /// <param name="y">The y dimension.</param>
        /// <param name="z">The z dimension.</param>
        public CustomVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomVector3"/> struct with a unity built-in vector3.
        /// </summary>
        /// <param name="value">The vector3.</param>
        public CustomVector3(Vector3 value)
        {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
        }

        /// <summary>
        /// Converts to unity built-in vector3.
        /// </summary>
        /// <returns>The unity built-in vector3.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(this.x, this.y, this.z);
        }
    }
}
