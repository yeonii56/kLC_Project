namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Entity;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The DustEffectType definitions.
    /// </summary>
    public enum DustEffectType
    {
        BuildRoom,
        DeleteRoom,
        PutFurniture,
    }

    /// <summary>
    /// The edge particle emitter.
    /// </summary>
    public struct Edge
    {
        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// The rotation.
        /// </summary>
        public Vector3 Rotation;
        
        /// <summary>
        /// The radius.
        /// </summary>
        public float Radius;
    }

    /// <summary>
    /// The GlobalParticleEffectsManager class.
    /// </summary>
    public class GlobalParticleEffectsManager : Singleton<GlobalParticleEffectsManager>
    {
        /// <summary>
        /// The dust effect materials.
        /// </summary>
        public List<Material> DustEffectMaterials;

        /// <summary>
        /// The build room dust effect prefab.
        /// </summary>
        public GameObject BuildDustEffectPrefab;

        /// <summary>
        /// The build duest effects queue.
        /// </summary>
        private Queue<GameObject> buildDustEffects = new Queue<GameObject>();

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            StartCoroutine(this.RecycleEffects());
        }

        /// <summary>
        /// Recycles effects.
        /// </summary>
        /// <returns>The coroutine.</returns>
        private IEnumerator RecycleEffects()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                while (this.buildDustEffects.Count > 0 && this.buildDustEffects.Peek().GetComponent<ParticleSystem>().isStopped)
                {
                    GameObjectRecycler.inst.Destroy(this.buildDustEffects.Dequeue());
                }
            }
        }

        /// <summary>
        /// Plays dusts effect.
        /// </summary>
        /// <param name="floorEntities">The floor entities.</param>
        /// <param name="particleSize">The particle size.</param>
        /// <param name="effectType">The effect type.</param>
        public void PlayDustsEffectByFloorEntities(List<FloorEntity> floorEntities, float particleSize, DustEffectType effectType)
        {
            var officeFloorCollection = new OfficeFloorCollection(floorEntities);
            officeFloorCollection.OfficeFloors.ForEach(f =>
            {
                var upFloor = officeFloorCollection.GetUpOfficeFloor(f);
                var downFloor = officeFloorCollection.GetDownOfficeFloor(f);
                var leftFloor = officeFloorCollection.GetLeftOfficeFloor(f);
                var rightFloor = officeFloorCollection.GetRightOfficeFloor(f);

                var leftUpFloor = officeFloorCollection.GetLeftUpCornerOfficeFloor(f);
                var rightUpFloor = officeFloorCollection.GetRightUpCornerOfficeFloor(f);
                var leftDownFloor = officeFloorCollection.GetLeftDownCornerOfficeFloor(f);
                var rightDownFloor = officeFloorCollection.GetRightDownCornerOfficeFloor(f);

                if (downFloor == null && leftDownFloor == null && rightDownFloor == null || downFloor == null && (rightFloor == null || leftFloor == null))
                {
                    var effect = GameObjectRecycler.inst.Instantiate(this.BuildDustEffectPrefab, this.transform).GetComponent<ParticleSystem>();
                    var shape = effect.shape;
                    shape.radius = (f.FloorEntity.LeftDownWorldPosition - f.FloorEntity.RightDownWorldPosition).magnitude / 2;
                    effect.gameObject.transform.position = (f.FloorEntity.LeftDownWorldPosition + f.FloorEntity.RightDownWorldPosition) / 2f + Vector3.up;
                    shape.rotation = new Vector3(0, 0, 180);
                    effect.GetComponent<Renderer>().material = this.DustEffectMaterials[(int)effectType];
                    effect.Play();
                    this.buildDustEffects.Enqueue(effect.gameObject);
                }

                if (leftFloor == null && leftUpFloor == null && leftDownFloor == null)
                {
                    var effect = GameObjectRecycler.inst.Instantiate(this.BuildDustEffectPrefab, this.transform).GetComponent<ParticleSystem>();
                    var shape = effect.shape;
                    shape.radius = (f.FloorEntity.LeftUpWorldPosition - f.FloorEntity.LeftDownWorldPosition).magnitude / 2;
                    effect.gameObject.transform.position = (f.FloorEntity.LeftUpWorldPosition + f.FloorEntity.LeftDownWorldPosition) / 2f + Vector3.up;
                    shape.rotation = new Vector3(0, 0, 270);
                    effect.GetComponent<Renderer>().material = this.DustEffectMaterials[(int)effectType];
                    effect.Play();
                    this.buildDustEffects.Enqueue(effect.gameObject);
                }

                if (upFloor == null && leftUpFloor == null && rightUpFloor == null || upFloor == null && (rightFloor == null || leftFloor == null))
                {
                    var effect = GameObjectRecycler.inst.Instantiate(this.BuildDustEffectPrefab, this.transform).GetComponent<ParticleSystem>();
                    var shape = effect.shape;
                    shape.radius = (f.FloorEntity.LeftUpWorldPosition - f.FloorEntity.RightUpWorldPosition).magnitude / 2;
                    effect.gameObject.transform.position = (f.FloorEntity.LeftUpWorldPosition + f.FloorEntity.RightUpWorldPosition) / 2f + Vector3.up;
                    shape.rotation = new Vector3(0, 0, 0);
                    effect.GetComponent<Renderer>().material = this.DustEffectMaterials[(int)effectType];
                    effect.Play();
                    this.buildDustEffects.Enqueue(effect.gameObject);
                }

                if (rightFloor == null && rightUpFloor == null && rightDownFloor == null)
                {
                    var effect = GameObjectRecycler.inst.Instantiate(this.BuildDustEffectPrefab, this.transform).GetComponent<ParticleSystem>();
                    var shape = effect.shape;
                    shape.radius = (f.FloorEntity.RightDownWorldPosition - f.FloorEntity.RightUpWorldPosition).magnitude / 2;
                    effect.gameObject.transform.position = (f.FloorEntity.RightDownWorldPosition + f.FloorEntity.RightUpWorldPosition) / 2f + Vector3.up;
                    shape.rotation = new Vector3(0, 0, 90);
                    effect.GetComponent<Renderer>().material = this.DustEffectMaterials[(int)effectType];
                    effect.Play();
                    this.buildDustEffects.Enqueue(effect.gameObject);
                }
            });
        }
    }
}
