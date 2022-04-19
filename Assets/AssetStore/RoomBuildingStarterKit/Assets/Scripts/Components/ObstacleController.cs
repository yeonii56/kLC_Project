namespace RoomBuildingStarterKit.BuildSystem
{
    using UnityEngine;
    using System.Collections;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// The ObstacleController class. When camera collides with this gameObject, the gameObject will become transparent for avoid blocking camera view.
    /// </summary>
    public class ObstacleController : MonoBehaviour
    {
        /// <summary>
        /// The smooth time.
        /// </summary>
        public float SmoothTime = 0.1f;

        /// <summary>
        /// The transparent material.
        /// </summary>
        public Material TransparentMaterial;

        /// <summary>
        /// The materials.
        /// </summary>
        private List<Material> originalMaterials;

        /// <summary>
        /// The wait for end of frame.
        /// </summary>
        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// The color smooth change velocity.
        /// </summary>
        private float colorSmoothChangeVelocity;

        /// <summary>
        /// The mesh renderer.
        /// </summary>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// The replace materials.
        /// </summary>
        private List<Material> replaceMaterials = new List<Material>();

        /// <summary>
        /// The collider trigger enter count.
        /// </summary>
        private int triggerEnterCount = 0;

        /// <summary>
        /// The start to transparent coroutine.
        /// </summary>
        private Coroutine transparentCoroutine;

        /// <summary>
        /// Starts to become transparent.
        /// </summary>
        /// <returns>The coroutine.</returns>
        private IEnumerator StartToTransparent()
        {
            this.replaceMaterials.Clear();
            for (int i = 0; i < this.meshRenderer.materials.Length; ++i)
            {
                this.replaceMaterials.Add(new Material(this.TransparentMaterial));
                this.replaceMaterials[i].SetTexture("_MainTex", this.originalMaterials[i].GetTexture("_MainTex"));
                this.replaceMaterials[i].color = this.originalMaterials[i].color;
            }

            this.meshRenderer.materials = this.replaceMaterials.ToArray();
            while (true)
            {
                int reachCount = 0;
                for (int i = 0; i < this.meshRenderer.materials.Length; ++i)
                {
                    var material = this.meshRenderer.materials[i];
                    material.color = new Color(material.color.r, material.color.g, material.color.b, Mathf.SmoothDamp(material.color.a, 0, ref this.colorSmoothChangeVelocity, this.SmoothTime, float.MaxValue, Time.unscaledDeltaTime));
                    if (material.color.a <= 0.001f)
                    {
                        ++reachCount;
                    }
                }

                if (reachCount >= this.meshRenderer.materials.Length)
                {
                    break;
                }

                yield return this.waitForEndOfFrame;
            }
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
            this.originalMaterials = this.meshRenderer.materials.ToList();
        }

        /// <summary>
        /// Triggers when other collider enter furniture collider.
        /// </summary>
        /// <param name="collider">Other collider.</param>
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.GetComponent<Camera>())
            {
                if (this.triggerEnterCount == 0)
                {
                    if (this.transparentCoroutine != null)
                    {
                        StopCoroutine(this.transparentCoroutine);
                        this.transparentCoroutine = null;
                    }

                    this.transparentCoroutine = StartCoroutine(this.StartToTransparent());
                }

                ++this.triggerEnterCount;
            }
        }

        /// <summary>
        /// Triggers when other collider exit furniture collider.
        /// </summary>
        /// <param name="collider">Other collider.</param>
        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.GetComponent<Camera>())
            {
                --this.triggerEnterCount;

                if (triggerEnterCount <= 0)
                {
                    this.triggerEnterCount = 0;

                    if (this.transparentCoroutine != null)
                    {
                        StopCoroutine(this.transparentCoroutine);
                        this.transparentCoroutine = null;
                    }

                    this.meshRenderer.materials = this.originalMaterials.ToArray();
                }
            }
        }
    }
}
