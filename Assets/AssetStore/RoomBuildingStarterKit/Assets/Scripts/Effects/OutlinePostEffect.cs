namespace RoomBuildingStarterKit
{
    using UnityEngine;
    using RoomBuildingStarterKit.Common.Extensions;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.Common;

    /// <summary>
    /// The outline post effect class used to outline gameObject when mouse hovered on it.
    /// </summary>
    public class OutlinePostEffect : MonoBehaviour
    {
        /// <summary>
        /// The outline post effect camera.
        /// </summary>
        public Camera OutlineCamera;
        
        /// <summary>
        /// The outline shader.
        /// </summary>
        public Shader OutlineShader;

        /// <summary>
        /// The render image shader.
        /// </summary>
        public Shader RenderImageShader;

        /// <summary>
        /// The outline material.
        /// </summary>
        public Material OutlineMaterial;

        /// <summary>
        /// The outline color.
        /// </summary>
        public Color OutlineColor;

        /// <summary>
        /// The outline alpha.
        /// </summary>
        [HideInInspector]
        public float OutlineAlpha = 0f;

        /// <summary>
        /// The outline width.
        /// </summary>
        [Range(1f, 16f)]
        public float OutlineWidth = 1f;

        /// <summary>
        /// The alpha change smooth time.
        /// </summary>
        public float AlphaSmoothTime = 0.05f;

        /// <summary>
        /// The mask render texture.
        /// </summary>
        private RenderTexture MaskRT;

        /// <summary>
        /// The blur render texture.
        /// </summary>
        private RenderTexture BlurRT;

        /// <summary>
        /// The alpha smooth change velocity.
        /// </summary>
        private float alphaSmoothChangeVelocity;

        /// <summary>
        /// The selectable object.
        /// </summary>
        private GameObject selectableObject;

        /// <summary>
        /// Changes gameObject hierarchy's layer by name.
        /// </summary>
        /// <param name="obj">The gameObject.</param>
        /// <param name="srcLayerName">The source layer name.</param>
        /// <param name="dstLayerName">The target layer name.</param>
        public void ChangeContinuousHierarchyLayerByName(GameObject obj, string srcLayerName, string dstLayerName)
        {
            if (obj.layer == LayerMask.NameToLayer(srcLayerName))
            {
                obj.layer = LayerMask.NameToLayer(dstLayerName);
                var curTransform = obj.transform;
                int count = curTransform.childCount;
                for (int i = 0; i < count; ++i)
                {
                    var child = curTransform.GetChild(i).gameObject;
                    this.ChangeContinuousHierarchyLayerByName(child, srcLayerName, dstLayerName);
                }
            }
        }

        /// <summary>
        /// Shows the outline.
        /// </summary>
        /// <param name="obj">The gameObject.</param>
        public void ShowOutline(GameObject obj)
        {
            if (InputWrapper.IsBlocking == false)
            {
                this.ChangeContinuousHierarchyLayerByName(obj, "Selectable", "Outline");
                this.OutlineAlpha = 0f;
            }
        }

        /// <summary>
        /// Hides the outline.
        /// </summary>
        /// <param name="obj">The gameObject.</param>
        public void HideOutline(GameObject obj)
        {
            this.ChangeContinuousHierarchyLayerByName(obj, "Outline", "Selectable");
            this.OutlineAlpha = 0f;
        }

        /// <summary>
        /// Renders image.
        /// </summary>
        /// <param name="source">The source texture.</param>
        /// <param name="destination">The destination texture.</param>
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (this.OutlineCamera.enabled == false)
            {
                this.OutlineCamera.enabled = true;
            }

            var activeRT = RenderTexture.active;
            RenderTexture.active = this.MaskRT;

            GL.Clear(true, true, Color.clear);

            RenderTexture.active = activeRT;
            this.OutlineCamera.targetTexture = this.MaskRT;
            this.OutlineCamera.RenderWithShader(this.RenderImageShader, "");

            this.OutlineMaterial.SetFloat("_AlphaFactor", this.OutlineAlpha);
            this.OutlineMaterial.SetVector("_BlurDirection", new Vector2(0, 1) * this.OutlineWidth);
            Graphics.Blit(this.MaskRT, this.BlurRT, this.OutlineMaterial, 0);

            this.OutlineMaterial.SetVector("_BlurDirection", new Vector2(1, 0) * this.OutlineWidth);
            Graphics.Blit(this.BlurRT, this.MaskRT, this.OutlineMaterial, 0);

            Shader.SetGlobalColor("_OutlineColor", this.OutlineColor);
            Graphics.Blit(source, destination);
            Graphics.Blit(this.MaskRT, destination, this.OutlineMaterial, 1);

            this.MaskRT.DiscardContents();
            this.BlurRT.DiscardContents();
        }

        /// <summary>
        /// Updates outline post effect.
        /// </summary>
        private void UpdateOutlineEffect()
        {
            var obj = OfficeMouseEventListener.GetMouseHoveredObjectByLayer(LayerMask.GetMask("Selectable") | LayerMask.GetMask("Outline"));
            if (obj != null && UIMouseEventDetector.CheckMouseEventOnUI() == false)
            {
                var rootObject = obj.GetSameLayerRootObject();
                if (this.selectableObject == null)
                {
                    this.ShowOutline(rootObject);
                }
                else if (this.selectableObject != rootObject)
                {
                    this.HideOutline(this.selectableObject);
                    this.ShowOutline(rootObject);
                }

                this.selectableObject = rootObject;
            }
            else if (this.selectableObject != null)
            {
                this.HideOutline(this.selectableObject);
                this.selectableObject = null;
            }
        }

        /// <summary>
        /// Executes when gameObject Instantiates.
        /// </summary>
        private void Awake()
        {
            this.BlurRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
            this.MaskRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            this.UpdateOutlineEffect();
            this.OutlineAlpha = Mathf.SmoothDamp(this.OutlineAlpha, 1.0f, ref this.alphaSmoothChangeVelocity, this.AlphaSmoothTime, float.MaxValue, Time.unscaledDeltaTime);
        }
    }
}