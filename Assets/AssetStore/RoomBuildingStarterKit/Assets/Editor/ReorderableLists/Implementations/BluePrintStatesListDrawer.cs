namespace RoomBuildingStarterKit.CustomReorderableList.Implementations
{
    using UnityEditor;
    using RoomBuildingStarterKit.Common;

    /// <summary>
    /// The blue print state list class used for the state machine of blue print.
    /// </summary>
    [CustomPropertyDrawer(typeof(StateListAttribute))]
    public class BluePrintStatesList : StatesListBase
    {
        /// <summary>
        /// Initializes the custom reorderable list.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void InitList(SerializedProperty property)
        {
            this.statesFolder = "BluePrintStates";
            base.InitList(property);
        }
    }
}