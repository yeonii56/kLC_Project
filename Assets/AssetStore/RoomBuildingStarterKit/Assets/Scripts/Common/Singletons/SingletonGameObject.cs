namespace RoomBuildingStarterKit.Common
{
    /// <summary>
    /// The GameObject Singleton base class. All gameObject Singleton class should derived from this class.
    /// </summary>
    public class SingletonGameObject<T> : Singleton<T>
        where T : SingletonGameObject<T>
    {
        /// <summary>
        /// Initializes a singleton instance.
        /// </summary>
        protected override void InitSingletonInst()
        {
            if (inst != null && inst != (T)this)
            {
                Destroy(this.gameObject);
                return;
            }

            inst = (T)this;
            this.AwakeInternal();
        }
    }
}