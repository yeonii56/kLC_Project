#if UNITY_EDITOR
namespace RoomBuildingStarterKit.OfficeEditor
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common.Extensions;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The MapGenerator class used to generate the base ground for building office in Office Editor.
    /// </summary>
    [ExecuteInEditMode]
    public class MapGenerator : MonoBehaviour
    {
        /// <summary>
        /// The build system settings.
        /// </summary>
        public BuildSystemSettings BuildSystemSettings;

        [HideInInspector]
        public int MapSize = 16;

        /// <summary>
        /// The floor prefab put on odd tile.
        /// </summary>
        public GameObject FloorOdd;

        /// <summary>
        /// The floor prefab put on even tile.
        /// </summary>
        public GameObject FloorEven;

        /// <summary>
        /// The floor prefab put on boundary.
        /// </summary>
        public GameObject BoundFloor;

        /// <summary>
        /// The floor container's transform.
        /// </summary>
        private Transform baseFloors;

        /// <summary>
        /// The floors' container.
        /// </summary>
        private List<GameObject> floors = new List<GameObject>();

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.baseFloors = this.transform.Find("RealRoom").Find("Floors");
        }

        /// <summary>
        /// Refreshes the base map.
        /// </summary>
        public void RefreshMap()
        {
            this.baseFloors.GetChilds(ref this.floors);
            this.floors.ForEach(f => DestroyImmediate(f));

            for (int i = 0; i <= this.MapSize + 1; ++i)
            {
                for (int j = 0; j <= this.MapSize + 1; ++j)
                {
                    GameObject floor = null;
                    if (i == 0 || i == this.MapSize + 1 || j == 0 || j == this.MapSize + 1)
                    {
                        floor = Instantiate(this.BoundFloor, this.baseFloors);
                    }
                    else
                    {
                        floor = Instantiate(((i + j) & 1) == 1 ? this.FloorOdd : this.FloorEven, this.baseFloors);
                    }

                    floor.transform.position = new Vector3(-i * this.BuildSystemSettings.GridSize, -0.2f, -j * this.BuildSystemSettings.GridSize);
                    floor.transform.localScale = new Vector3(this.BuildSystemSettings.GridSize / 2, 1, this.BuildSystemSettings.GridSize / 2);
                }
            }
        }
    }
}
#endif