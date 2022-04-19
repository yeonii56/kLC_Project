namespace RoomBuildingStarterKit.Helpers
{
    using RoomBuildingStarterKit.BuildSystem;
    using UnityEngine;
    
    /// <summary>
    /// The furniture helper class.
    /// </summary>
    public class FurnitureHelper
    {
        /// <summary>
        /// Enables the furniture selectable state.
        /// </summary>
        /// <param name="bluePrint">The blue print component.</param>
        public static void EnableFurnituresSelectable(BluePrint bluePrint)
        {
            // Windows
            bluePrint.BluePrintWindowFurnitureEntities.ForEach(w =>
            {
                w.Furniture.layer = LayerMask.NameToLayer("Selectable");
                w.Furniture.GetComponent<BoxCollider>().enabled = true;
            });

            // Doors
            bluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
            {
                d.Furniture.layer = LayerMask.NameToLayer("Selectable");
                d.Furniture.GetComponent<BoxCollider>().enabled = true;
            });

            // Furnitures
            bluePrint.BluePrintFurnitureEntities.ForEach(f =>
            {
                FurnitureHelper.ChangeFurnitureLayer(f.Furniture, LayerMask.NameToLayer("Selectable"));
                f.Furniture.GetComponent<BoxCollider>().enabled = true;
            });

            // Wall Furnitures
            bluePrint.BluePrintWallFurnitureEntities.ForEach(f =>
            {
                FurnitureHelper.ChangeFurnitureLayer(f.Furniture, LayerMask.NameToLayer("Selectable"));
                f.Furniture.GetComponent<BoxCollider>().enabled = true;
            });
        }

        /// <summary>
        /// Disables the furniture selectable state.
        /// </summary>
        /// <param name="bluePrint">The blue print component.</param>
        public static void DisableFurnituresSelectable(BluePrint bluePrint)
        {
            // Windows
            bluePrint.BluePrintWindowFurnitureEntities.ForEach(w =>
            {
                w.Furniture.GetComponent<BoxCollider>().enabled = false;
            });

            // Doors
            bluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
            {
                d.Furniture.GetComponent<BoxCollider>().enabled = false;
            });

            // Furnitures
            bluePrint.BluePrintFurnitureEntities.ForEach(f =>
            {
                f.Furniture.GetComponent<BoxCollider>().enabled = false;
            });

            // Wall Furnitures
            bluePrint.BluePrintWallFurnitureEntities.ForEach(f =>
            {
                f.Furniture.GetComponent<BoxCollider>().enabled = false;
            });
        }

        /// <summary>
        /// Change the furniture layer.
        /// </summary>
        /// <param name="furniture">The furniture.</param>
        /// <param name="layer">The layer.</param>
        public static void ChangeFurnitureLayer(GameObject furniture, int layer)
        {
            furniture.layer = layer;
            for (int i = 0; i < furniture.transform.childCount; ++i)
            {
                var child = furniture.transform.GetChild(i).gameObject;
                if (child.name != "BuildableHintPlane" && child.layer != LayerMask.NameToLayer("Furniture"))
                {
                    child.layer = layer;
                }
            }
        }
    }
}
