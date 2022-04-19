namespace RoomBuildingStarterKit.Common
{
    using System.Collections.Generic;
    
    /// <summary>
    /// The UnionSet class used to check the door connection.
    /// </summary>
    public class UnionSet
    {
        /// <summary>
        /// The father index array.
        /// </summary>
        private Dictionary<int, int> fa;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnionSet"/> class.
        /// </summary>
        /// <param name="size">The count of the </param>
        public UnionSet(List<int> indexes)
        {
            this.fa = new Dictionary<int, int>();
            indexes.ForEach(i => this.fa[i] = i);
        }

        /// <summary>
        /// Finds the set.
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The father index.</returns>
        public int FindSet(int x)
        {
            if (this.fa[x] == x)
            {
                return x;
            }

            return this.fa[x] = this.FindSet(this.fa[x]);
        }

        /// <summary>
        /// Unions two sets.
        /// </summary>
        /// <param name="x">The set x.</param>
        /// <param name="y">The set y.</param>
        public void Union(int x, int y)
        {
            int faX = this.FindSet(x);
            int faY = this.FindSet(y);
            this.fa[faX] = faY;
        }
    }
}