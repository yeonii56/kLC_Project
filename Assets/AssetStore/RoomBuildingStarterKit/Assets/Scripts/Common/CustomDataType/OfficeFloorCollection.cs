namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// The OfficeFloor class represents the floor data in office. It's the element in a office floor collection. Gives the floor position infos to locate a floor in a collection.
    /// </summary>
    public class OfficeFloor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeFloor"/> class.
        /// </summary>
        /// <param name="index">The floor index.</param>
        /// <param name="x">The x dimension of the floor.</param>
        /// <param name="z">The z dimension of the floor.</param>
        /// <param name="floorEntity">The floor entity.</param>
        public OfficeFloor(int index, int x, int z, FloorEntity floorEntity)
        {
            this.Reset(index, x, z, floorEntity);
        }

        /// <summary>
        /// Resets the office floor.
        /// </summary>
        /// <param name="index">The floor index.</param>
        /// <param name="x">The x dimension of the floor.</param>
        /// <param name="z">The y dimension of the floor.</param>
        /// <param name="floorEntity">The floor entity.</param>
        public void Reset(int index, int x, int z, FloorEntity floorEntity)
        {
            this.Index = index;
            this.X = x;
            this.Z = z;
            this.FloorEntity = floorEntity;
        }

        /// <summary>
        /// Gets or sets the floor entity referenced by the office floor.
        /// </summary>
        public FloorEntity FloorEntity { get; set; }

        /// <summary>
        /// Gets or sets the floor index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the x dimension of the floor in collection.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y dimension of the floor in collection.
        /// </summary>
        public int Z { get; set; }
    }

    /// <summary>
    /// The OfficeFloorCollection is a floor grid. It supply an efficient way to locate floors in a given grid by convert a floor list to a floor grid.
    /// 1. New a office floor collection instance.
    /// 2. Resizes it to a custom dimension, such as the size of an office. It will generate a floor grid with the specified size and nothing inside.
    /// 3. Reassigns it with a floor entity list to fill the current floor grid generated in last step.
    /// </summary>
    public class OfficeFloorCollection
    {
        /// <summary>
        /// Whether the collection has been resized or not.
        /// </summary>
        private bool hasResized = false;

        /// <summary>
        /// The matrix used to locate the office floor in a given grid.
        /// </summary>
        private List<List<OfficeFloor>> floorMatrix = new List<List<OfficeFloor>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeFloorCollection"/> class.
        /// </summary>
        public OfficeFloorCollection() 
        {
        }

        /// <summary>
        /// Initailizes a new instance of the <see cref="OfficeFloorCollection"/> class.
        /// </summary>
        /// <param name="floorEntities">The list of floor entities.</param>
        /// <param name="overrideFloorEntity">Whether to override floor entity or not.</param>
        public OfficeFloorCollection(List<FloorEntity> floorEntities, bool overrideFloorEntity = false)
        {
            this.Reset(floorEntities, overrideFloorEntity);
        }

        /// <summary>
        /// Gets the floor entity list in the collection.
        /// </summary>
        public List<FloorEntity> FloorEntities { get; private set; } = new List<FloorEntity>();

        /// <summary>
        /// Gets the office floor list in the collection.
        /// </summary>
        public List<OfficeFloor> OfficeFloors { get; private set; } = new List<OfficeFloor>();

        /// <summary>
        /// Gets the row count of the floor grid.
        /// </summary>
        public int RowCount { get; private set; } = -1;

        /// <summary>
        /// Gets the column count of the floor grid.
        /// </summary>
        public int ColumnCount { get; private set; } = -1;

        /// <summary>
        /// The minimum x dimension of the floor in the floor grid.
        /// </summary>
        public float MinX { get; private set; }

        /// <summary>
        /// The maximum x dimension of the floor in the floor grid.
        /// </summary>
        public float MaxX { get; private set; }

        /// <summary>
        /// The minimum z dimension of the floor in the floor grid.
        /// </summary>
        public float MinZ { get; private set; }

        /// <summary>
        /// The maximum z dimension of the floor in the floor grid.
        /// </summary>
        public float MaxZ { get; private set; }

        /// <summary>
        /// Reassigns the floor grid with another size.
        /// </summary>
        /// <param name="MinX">The minimum x dimension.</param>
        /// <param name="MinZ">The minimum z dimension.</param>
        /// <param name="MaxX">The maximum x dimension.</param>
        /// <param name="MaxZ">The maximum z dimension.</param>
        public void Resize(float MinX, float MinZ, float MaxX, float MaxZ )
        {
            this.MinX = MinX;
            this.MinZ = MinZ;
            this.MaxX = MaxX;
            this.MaxZ = MaxZ;

            this.RowCount = (int)((MaxX - MinX) / Global.inst.BuildSystemSettings.GridSize) + 1;
            this.ColumnCount = (int)((MaxZ - MinZ) / Global.inst.BuildSystemSettings.GridSize) + 1;

            this.hasResized = true;
        }

        /// <summary>
        /// Reassigns the floor grid with another grid.
        /// </summary>
        /// <param name="floorCollection">The floor collection.</param>
        public void Resize(OfficeFloorCollection floorCollection)
        {
            this.Resize(
                floorCollection.MinX,
                floorCollection.MinZ,
                floorCollection.MaxX,
                floorCollection.MaxZ);
        }

        /// <summary>
        /// Reassigns the office collection with floor entities and determine whether to override these floor entities with the x/z dimension in collection.
        /// </summary>
        /// <param name="floorEntities">The floor entities.</param>
        /// <param name="overrideFloorEntity">Whether to override floor entity or not.</param>
        /// <returns></returns>
        public OfficeFloorCollection Reset(List<FloorEntity> floorEntities, bool overrideFloorEntity = false)
        {
            floorEntities = floorEntities.Where(f => f != null).ToList();
            this.FloorEntities = floorEntities;

            if (this.hasResized == false)
            {
                this.MinX = floorEntities.Min(f => f.LeftDownLocalPosition.x);
                this.MaxX = floorEntities.Max(f => f.LeftDownLocalPosition.x);
                this.MinZ = floorEntities.Min(f => f.LeftDownLocalPosition.z);
                this.MaxZ = floorEntities.Max(f => f.LeftDownLocalPosition.z);

                this.RowCount = (int)((this.MaxX - this.MinX) / Global.inst.BuildSystemSettings.GridSize) + 1;
                this.ColumnCount = (int)((this.MaxZ - this.MinZ) / Global.inst.BuildSystemSettings.GridSize) + 1;
            }

            for (int i = 0; i < this.RowCount; ++i)
            {
                if (i == this.floorMatrix.Count)
                {
                    this.floorMatrix.Add(new List<OfficeFloor>(this.ColumnCount));
                }

                for (int j = 0; j < this.ColumnCount; ++j)
                {
                    if (j == this.floorMatrix[i].Count)
                    {
                        this.floorMatrix[i].Add(new OfficeFloor(-1, -1, -1, null));
                    }
                    else
                    {
                        this.floorMatrix[i][j].Reset(-1, -1, -1, null);
                    }
                }
            }

            this.OfficeFloors.Clear();
            floorEntities.ForEach(f =>
            {
                int x = (int)((this.MaxX - f.LeftDownLocalPosition.x) / Global.inst.BuildSystemSettings.GridSize);
                int y = (int)((this.MaxZ - f.LeftDownLocalPosition.z) / Global.inst.BuildSystemSettings.GridSize);
                var floor = this.floorMatrix[x][y];
                floor.Reset(y * this.RowCount + x, x, y, f);
                this.OfficeFloors.Add(floor);

                if (overrideFloorEntity)
                {
                    f.Index = floor.Index;
                    f.X = floor.X;
                    f.Z = floor.Z;
                }
            });

            return this;
        }

        /// <summary>
        /// Gets office floor by specified coordinates.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>The office floor.</returns>
        public OfficeFloor GetOfficeFloor(int row, int column)
        {
            if (row >= this.RowCount || column >= this.ColumnCount || row < 0 || column < 0 || this.floorMatrix[row][column].Index == -1)
            {
                return null;
            }

            return this.floorMatrix[row][column];
        }

        /// <summary>
        /// Gets office floor by specified coordinates.
        /// </summary>
        /// <param name="rowAndColumn">The row and column.</param>
        /// <returns>The office floor.</returns>
        public OfficeFloor GetOfficeFloor(Vector2Int rowAndColumn)
        {
            return this.GetOfficeFloor(rowAndColumn.x, rowAndColumn.y);
        }

        /// <summary>
        /// Gives an office floor and direction, return another office floor in the direction of the given office floor. 
        /// Directions and their relative values:
        /// 7 0 4
        /// 3   1
        /// 6 2 5
        /// </summary>
        /// <param name="officeFloor">The office floor.</param>
        /// <param name="dir">The direction.</param>
        /// <returns>The office floor.</returns>
        public OfficeFloor GetOfficeFloorByDir(OfficeFloor officeFloor, short dir)
        {
            Assert.IsTrue(dir >= 0 && dir <= 7, "dir is invalid");
            switch (dir)
            {
                case 0: return this.GetUpOfficeFloor(officeFloor);
                case 1: return this.GetRightOfficeFloor(officeFloor);
                case 2: return this.GetDownOfficeFloor(officeFloor);
                case 3: return this.GetLeftOfficeFloor(officeFloor);
                case 4: return this.GetLeftUpCornerOfficeFloor(officeFloor);
                case 5: return this.GetRightUpCornerOfficeFloor(officeFloor);
                case 6: return this.GetRightDownCornerOfficeFloor(officeFloor);
                case 7: return this.GetLeftDownCornerOfficeFloor(officeFloor);
                default: return null;
            }
        }

        /// <summary>
        /// Gets the office floor up to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor up to the specified office floor.</returns>
        public OfficeFloor GetUpOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X, floor.Z + 1);
        }

        /// <summary>
        /// Gets the office floor down to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor down to the specified office floor.</returns>
        public OfficeFloor GetDownOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X, floor.Z - 1);
        }

        /// <summary>
        /// Gets the office floor left to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor left to the specified office floor.</returns>
        public OfficeFloor GetLeftOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X - 1, floor.Z);
        }

        /// <summary>
        /// Gets the office floor right to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor right to the specified office floor.</returns>
        public OfficeFloor GetRightOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X + 1, floor.Z);
        }

        /// <summary>
        /// Gets the office floor left up to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor left up to the specified office floor.</returns>
        public OfficeFloor GetLeftUpCornerOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X - 1, floor.Z + 1);
        }

        /// <summary>
        /// Gets the office floor right up to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor right up to the specified office floor.</returns>
        public OfficeFloor GetRightUpCornerOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X + 1, floor.Z + 1);
        }

        /// <summary>
        /// Gets the office floor left down to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor left down to the specified office floor.</returns>
        public OfficeFloor GetLeftDownCornerOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X - 1, floor.Z - 1);
        }

        /// <summary>
        /// Gets the office floor right down to the specified office floor.
        /// </summary>
        /// <param name="floor">The office floor.</param>
        /// <returns>The office floor right down to the specified office floor.</returns>
        public OfficeFloor GetRightDownCornerOfficeFloor(OfficeFloor floor)
        {
            return this.GetOfficeFloor(floor.X + 1, floor.Z - 1);
        }

        /// <summary>
        /// Checks whether the floors in this grid are connected.
        /// </summary>
        /// <returns>Whether the floors in this grid are connected.</returns>
        public bool CheckConnect()
        {
            List<Vector2Int> dirs = new List<Vector2Int> { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
            bool[] visit = new bool[this.RowCount * this.ColumnCount + 1];
            Queue<OfficeFloor> queue = new Queue<OfficeFloor>();
            var start = this.OfficeFloors.First();
            queue.Enqueue(start);
            visit[start.Index] = true;
            int count = 1;
            while (queue.Count > 0)
            {
                var floor = queue.Dequeue();
                foreach (var dir in dirs)
                {
                    var target = this.GetOfficeFloor(floor.X + dir.x, floor.Z + dir.y);
                    if (target != null && visit[target.Index] == false)
                    {
                        visit[target.Index] = true;
                        queue.Enqueue(target);
                        ++count;
                    }
                }
            }

            return count == this.OfficeFloors.Count;
        }

        /// <summary>
        /// Checks whether the room doors can be reached from every office doors.
        /// </summary>
        /// <param name="doorParams">The blue print door parameters.</param>
        /// <param name="bluePrintFloorEntities">The blue print floor entities.</param>
        /// <returns></returns>
        public bool CheckDoorsUnion(List<Tuple<FloorEntity, short>> doorParams = null, List<FloorEntity> bluePrintFloorEntities = null)
        {
            return this.OfficeFloors
                .Where(f => f.FloorEntity.IsOfficeDoorFloor)
                .All(f => this.CheckSingleDoorUnion(f, doorParams, bluePrintFloorEntities));
        }

        /// <summary>
        /// Checks whether every doors could be reached through a office door.
        /// </summary>
        /// <param name="officeDoorFloor">The floor under the office door.</param>
        /// <param name="doorParams">The blue print door parameters.</param>
        /// <param name="bluePrintFloorEntities">The floor entities of the blue print room.</param>
        /// <returns>The office door could reach every rooms or not.</returns>
        private bool CheckSingleDoorUnion(OfficeFloor officeDoorFloor, List<Tuple<FloorEntity, short>> doorParams, List<FloorEntity> bluePrintFloorEntities = null)
        {
            List<Vector2Int> dirs = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
            bool[] visit = new bool[this.RowCount * this.ColumnCount + 1];
            bool[] isBluePrintFloorEntity = new bool[this.RowCount * this.ColumnCount + 1];

            // Can't occupy office doors.
            if (bluePrintFloorEntities != null && bluePrintFloorEntities.Any(f => (f?.IsOfficeDoorFloor ?? false) == true))
            {
                return false;
            }

            bluePrintFloorEntities.ForEach(f =>
            {
                if (f != null)
                {
                    isBluePrintFloorEntity[f.Index] = true;
                }
            });

            Queue<OfficeFloor> queue = new Queue<OfficeFloor>();
            var start = officeDoorFloor;

            queue.Enqueue(start);
            visit[start.Index] = true;
            int count = 0;
            while (queue.Count > 0)
            {
                var floor = queue.Dequeue();

                if (floor.FloorEntity.OccupiedRoom != null || bluePrintFloorEntities != null && isBluePrintFloorEntity[floor.FloorEntity.Index])
                {
                    ++count;
                }

                for (short i = 0; i < dirs.Count; ++i)
                {
                    var target = this.GetOfficeFloor(floor.X + dirs[i].x, floor.Z + dirs[i].y);
                    if (target != null && visit[target.Index] == false && this.IsConnect(floor.FloorEntity, target.FloorEntity, i, bluePrintFloorEntities, doorParams, ref isBluePrintFloorEntity))
                    {
                        visit[target.Index] = true;
                        queue.Enqueue(target);
                    }
                }
            }

            return count == this.OfficeFloors.Count(f => f.FloorEntity.OccupiedRoom != null) + (bluePrintFloorEntities != null ? bluePrintFloorEntities.Count : 0);
        }

        private bool IsConnect(FloorEntity standFloorEntity, FloorEntity targetFloorEntity, short dir, List<FloorEntity> bluePrintFloorEntities, List<Tuple<FloorEntity, short>> doorParams, ref bool[] isBluePrintFloorEntity)
        {
            var isStandBluePrintFloor = isBluePrintFloorEntity[standFloorEntity.Index];
            var isTargetBluePrintFloor = isBluePrintFloorEntity[targetFloorEntity.Index];

            if (bluePrintFloorEntities != null && (isStandBluePrintFloor || isTargetBluePrintFloor))
            {
                if (isStandBluePrintFloor && isTargetBluePrintFloor||
                    isStandBluePrintFloor && doorParams != null && doorParams.Any(p => p.Item1 == targetFloorEntity && p.Item2 == dir) ||
                    isTargetBluePrintFloor && doorParams != null && doorParams.Any(p => p.Item1 == standFloorEntity && p.Item2 == (dir + 2) % 4))
                {
                    return true;
                }

                return standFloorEntity.OccupiedDoorEntities.Any(d => d.InRoomFloorEntity == targetFloorEntity && d.OutRoomFloorEntity == standFloorEntity) ||
                       targetFloorEntity.OccupiedDoorEntities.Any(d => d.InRoomFloorEntity == standFloorEntity && d.OutRoomFloorEntity == targetFloorEntity);
            }

            return (standFloorEntity.GetWallByDir(dir) == null ||
                standFloorEntity.OccupiedDoorEntities.Any(d => d.InRoomFloorEntity == targetFloorEntity && d.OutRoomFloorEntity == standFloorEntity) ||
                targetFloorEntity.OccupiedDoorEntities.Any(d => d.InRoomFloorEntity == standFloorEntity && d.OutRoomFloorEntity == targetFloorEntity));
        }
    }
}
