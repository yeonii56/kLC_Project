namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The MeshCombineManager class is used to merge gameObject meshes inside office in run time for decrease draw call.
    /// </summary>
    public class MeshCombineManager : Singleton<MeshCombineManager>
    {
        /// <summary>
        /// The enable mesh combine or not.
        /// </summary>
        public bool EnableMeshCombine = false;

        /// <summary>
        /// The office static mesh container.
        /// </summary>
        public Transform OfficeStaticMeshContainer;

        /// <summary>
        /// The office static mesh filter component.
        /// </summary>
        private MeshFilter officeStaticMeshFilter;

        /// <summary>
        /// The office static mesh renderer.
        /// </summary>
        private MeshRenderer officeStaticMeshRenderer;

        /// <summary>
        /// Merges target gameObject's mesh to office static mesh.
        /// </summary>
        /// <param name="meshFilters">The mesh filters.</param>
        public void MergeToOfficeStaticMeshes(List<MeshFilter> meshFilters)
        {
            if (!this.EnableMeshCombine || meshFilters.Count == 0)
            {
                return;
            }

            var combineInstances = new List<CombineInstance>();
            var combineInstance = new CombineInstance();
            combineInstance.mesh = this.officeStaticMeshFilter.sharedMesh;
            combineInstance.transform = this.OfficeStaticMeshContainer.worldToLocalMatrix * this.officeStaticMeshFilter.transform.localToWorldMatrix;
            combineInstances.Add(combineInstance);

            foreach (var meshFilter in meshFilters)
            {
                combineInstance = new CombineInstance();
                combineInstance.mesh = meshFilter.sharedMesh;
                combineInstance.transform = this.OfficeStaticMeshContainer.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combineInstance);
                meshFilter.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }

            DestroyImmediate(this.officeStaticMeshFilter.mesh);
            var mesh = new Mesh();
            mesh.name = "TechParkStaticMesh";
            this.officeStaticMeshFilter.sharedMesh = mesh;
            this.officeStaticMeshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
            this.officeStaticMeshRenderer.sharedMaterial = meshFilters[0].gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        }

        /// <summary>
        /// Merges target gameObject's mesh to office static mesh.
        /// </summary>
        /// <param name="target">The target gameObject.</param>
        public void MergeToOfficeStaticMeshes(Transform target)
        {
            if (!this.EnableMeshCombine)
            {
                return;
            }

            var meshFilters = new List<MeshFilter>();
            target.GetComponentsInChildren<MeshFilter>(meshFilters);

            this.MergeToOfficeStaticMeshes(meshFilters);
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            this.officeStaticMeshFilter = this.OfficeStaticMeshContainer.GetComponent<MeshFilter>();
            this.officeStaticMeshRenderer = this.OfficeStaticMeshContainer.GetComponent<MeshRenderer>();
        }
    }
}