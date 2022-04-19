namespace RoomBuildingStarterKit.BuildSystem
{
    using UnityEngine;
    using System.Collections.Generic;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Entity;
    using RoomBuildingStarterKit.Common.Extensions;

    /// <summary>
    /// The FoundationManager class used to initialize the association between the floor gameobject and floor entity in a office.
    /// </summary>
    public class FoundationManager : MonoBehaviour
    {
        /// <summary>
        /// The const string for BaseWalls.
        /// </summary>
        private const string BASE_WALLS = "BaseWalls";

        /// <summary>
        /// The transform of baseFloors gameobject
        /// </summary>
        private Transform baseFloors;

        /// <summary>
        /// The office doors.
        /// </summary>
        private List<FurnitureController> officeDoors = new List<FurnitureController>();

        /// <summary>
        /// Gets or sets whether the office is come from unlock.
        /// </summary>
        public bool JustUnLock { get; set; } = false;

        /// <summary>
        /// Gets the floor gameObject to floor entity dictionary.
        /// </summary>
        public Dictionary<int?, FloorEntity> FloorGoToFloorEntityMap { get; } = new Dictionary<int?, FloorEntity>();

        /// <summary>
        /// Gets the office floor collection.
        /// </summary>
        public OfficeFloorCollection OfficeFloorCollection { get; private set; }

        /// <summary>
        /// Gets the office doors.
        /// </summary>
        public List<FurnitureController> OfficeDoors => this.officeDoors;

        /// <summary>
        /// Loads base floors when game start.
        /// </summary>
        public void LoadBaseFloors()
        {
            List<Transform> floorTransforms = new List<Transform>();
            this.baseFloors.GetChilds(ref floorTransforms);

            // Sort floors by x and z coordinates.
            floorTransforms.Sort((lhs, rhs) =>
            {
                if (lhs.localPosition.z > rhs.localPosition.z || lhs.localPosition.z == rhs.localPosition.z && lhs.localPosition.x > rhs.localPosition.x)
                {
                    return -1;
                }

                return 1;
            });

            var floorEntities = new List<FloorEntity>();
            var count = floorTransforms.Count;
            for (int i = 0; i < count; ++i)
            {
                var f = new FloorEntity(floorTransforms[i], this.gameObject);

                if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && floorTransforms[i].gameObject.name.StartsWith("EditorFloor_Bound"))
                {
                    f.IsOfficeEditorBoundFloor = true;
                }

                var origin = (f.LeftDownWorldPosition + f.RightUpWorldPosition) / 2f + Vector3.up;
                Ray leftRay = new Ray(origin, Vector3.right);
                Ray rightRay = new Ray(origin, -Vector3.right);
                Ray downRay = new Ray(origin, Vector3.forward);
                Ray upRay = new Ray(origin, -Vector3.forward);

                Ray leftUpRay = new Ray(origin, Vector3.right - Vector3.forward);
                Ray leftDownRay = new Ray(origin, Vector3.right + Vector3.forward);
                Ray rightUpRay = new Ray(origin, -Vector3.right - Vector3.forward);
                Ray rightDownRay = new Ray(origin, -Vector3.right + Vector3.forward);

                RaycastHit hit;

                // Detects Office Doors.
                if (Physics.Raycast(leftRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("OfficeDoor")) 
                    || Physics.Raycast(rightRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("OfficeDoor"))
                    || Physics.Raycast(upRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("OfficeDoor"))
                    || Physics.Raycast(downRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("OfficeDoor")))
                {
                    f.IsOfficeDoorFloor = true;
                }

                // Detects walls.
                GameObject wall;
                if (Physics.Raycast(leftRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("Wall")))
                {
                    wall = hit.collider.gameObject;
                    f.OriginalLeftWall = f.LeftWall = new WallEntity(wall, true, wall.name.Contains("Window"), f);
                }

                if (Physics.Raycast(rightRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("Wall")))
                {
                    wall = hit.collider.gameObject;
                    f.OriginalRightWall = f.RightWall = new WallEntity(wall, true, wall.name.Contains("Window"), f);
                }

                if (Physics.Raycast(upRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("Wall")))
                {
                    wall = hit.collider.gameObject;
                    f.OriginalUpWall = f.UpWall = new WallEntity(wall, true, wall.name.Contains("Window"), f);
                }

                if (Physics.Raycast(downRay, out hit, Global.inst.BuildSystemSettings.GridSize / 2 + 0.2f, LayerMask.GetMask("Wall")))
                {
                    wall = hit.collider.gameObject;
                    f.OriginalDownWall = f.DownWall = new WallEntity(wall, true, wall.name.Contains("Window"), f);
                }

                // Detects wallCorners.
                if (Physics.Raycast(leftUpRay, out hit, Global.inst.BuildSystemSettings.GridSize, LayerMask.GetMask("Wall")) && hit.collider.gameObject.name.Contains("WallCorner"))
                {
                    f.OriginalLeftUpWallCorner = f.LeftUpOutWallCorner = hit.collider.gameObject;
                }

                if (Physics.Raycast(leftDownRay, out hit, Global.inst.BuildSystemSettings.GridSize, LayerMask.GetMask("Wall")) && hit.collider.gameObject.name.Contains("WallCorner"))
                {
                    f.OriginalLeftDownWallCorner = f.LeftDownOutWallCorner = hit.collider.gameObject;
                }

                if (Physics.Raycast(rightUpRay, out hit, Global.inst.BuildSystemSettings.GridSize, LayerMask.GetMask("Wall")) && hit.collider.gameObject.name.Contains("WallCorner"))
                {
                    f.OriginalRightUpWallCorner = f.RightUpOutWallCorner = hit.collider.gameObject;
                }

                if (Physics.Raycast(rightDownRay, out hit, Global.inst.BuildSystemSettings.GridSize, LayerMask.GetMask("Wall")) && hit.collider.gameObject.name.Contains("WallCorner"))
                {
                    f.OriginalRightDownWallCorner = f.RightDownOutWallCorner = hit.collider.gameObject;
                }

                floorEntities.Add(f);
                GlobalGridManager.inst.FloorGoToFloorEntityMaps.Add(floorTransforms[i].gameObject.GetInstanceID(), f);
                this.FloorGoToFloorEntityMap.Add(floorTransforms[i].gameObject.GetInstanceID(), f);
            }

            this.OfficeFloorCollection = new OfficeFloorCollection(floorEntities, overrideFloorEntity: true);

            if (MeshCombineManager.inst != null)
            {
                List<MeshFilter> meshFilters = new List<MeshFilter>();
                this.transform.Find("RealRoom").Find("Walls").GetChildsByLayer<MeshFilter>(LayerMask.NameToLayer("MeshCombine"), ref meshFilters);
                MeshCombineManager.inst.MergeToOfficeStaticMeshes(meshFilters);
            }

            // Plays dust effect when unlock office.
            if (this.JustUnLock)
            {
                GlobalParticleEffectsManager.inst.PlayDustsEffectByFloorEntities(floorEntities, 0, DustEffectType.BuildRoom);
            }
        }

        /// <summary>
        /// Initializes foundation manager data.
        /// </summary>
        private void InitData()
        {
            this.baseFloors = this.transform.Find("RealRoom").Find("Floors");
            this.transform.Find("RealRoom")?.Find("Furnitures")?.GetChilds(ref this.officeDoors);
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.InitData();
        }

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            this.LoadBaseFloors();
        }
    }
}