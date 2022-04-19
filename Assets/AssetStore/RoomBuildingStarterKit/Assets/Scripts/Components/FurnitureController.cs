namespace RoomBuildingStarterKit.BuildSystem
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// The FurnitureController class used to control furniture behaviour.
    /// </summary>
    public class FurnitureController : MonoBehaviour
    {
        /// <summary>
        /// The furniture occupied floor size.
        /// </summary>
        public Vector2Int Dimension;

        /// <summary>
        /// The furniture type.
        /// </summary>
        public FurnitureType FurnitureType;

        /// <summary>
        /// The furniture is a wall furniture or not.
        /// </summary>
        public bool IsWallFurniture;

        /// <summary>
        /// The furniture type.
        /// </summary>
        public GameObject Furniture;

        /// <summary>
        /// The hint panel.
        /// </summary>
        public GameObject HintPanel;

        /// <summary>
        /// The green panel sprite.
        /// </summary>
        public Sprite GreenPanelSprite;

        /// <summary>
        /// The red panel sprite.
        /// </summary>
        public Sprite RedPanelSprite;

        /// <summary>
        /// The hint panel sprite render.
        /// </summary>
        private SpriteRenderer hintPanelSpriteRender;

        /// <summary>
        /// The wait for end of frame.
        /// </summary>
        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// The smooth time.
        /// </summary>
        private float smoothTime = 0.1f;

        /// <summary>
        /// The mesh renderer.
        /// </summary>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// The color change smooth damp velocity.
        /// </summary>
        private float colorSmoothChangeVelocity;

        /// <summary>
        /// Shows the buildable panel.
        /// </summary>
        public void ShowBuildablePanel()
        {
            var renderer = this.HintPanel.GetComponent<SpriteRenderer>();
            renderer.sprite = this.GreenPanelSprite;

            if (this.HintPanel.activeSelf == false)
            {
                this.EnableBuildablePanel();
                StartCoroutine(this.ShowBuildablePanelSmooth(renderer));
            }

            if (this.Furniture != null)
            {
                this.meshRenderer.sharedMaterial = GlobalFurnitureManager.inst.FurnitureOpaqueMaterial;
            }
        }

        /// <summary>
        /// Sets the buildable state.
        /// </summary>
        /// <param name="isBuildable">Whether can build the furniture or not.</param>
        /// <param name="isPanelTransparent">Is a transparent panel or not.</param>
        public void SetBuildableState(bool isBuildable, bool isPanelTransparent = false)
        {
            if (isBuildable)
            {
                this.DisableBuildablePanel();
            }
            else
            {
                this.ShowNonBuildablePanel(isPanelTransparent);
            }
        }

        /// <summary>
        /// Shows the non buildable panel.
        /// </summary>
        /// <param name="shouldTransparent">Should the buildable panel be transparent or not.</param>
        public void ShowNonBuildablePanel(bool shouldTransparent = true)
        {
            var renderer = this.HintPanel.GetComponent<SpriteRenderer>();
            renderer.sprite = this.RedPanelSprite;

            if (this.HintPanel.activeSelf == false)
            {
                this.EnableBuildablePanel();
                StartCoroutine(this.ShowBuildablePanelSmooth(renderer));
            }

            if (this.Furniture != null && shouldTransparent)
            {
                this.meshRenderer.sharedMaterial = GlobalFurnitureManager.inst.FurnitureTransparentMaterial;
            }
        }

        /// <summary>
        /// Disables the buildable panel.
        /// </summary>
        public void DisableBuildablePanel()
        {
            this.HintPanel.SetActive(false);
            if (this.Furniture != null)
            {
                this.meshRenderer.sharedMaterial = GlobalFurnitureManager.inst.FurnitureOpaqueMaterial;
            }
        }

        /// <summary>
        /// Enables the buildable panel.
        /// </summary>
        public void EnableBuildablePanel()
        {
            this.HintPanel.SetActive(true);
        }

        /// <summary>
        /// Shows buildable panel smoothly.
        /// </summary>
        /// <param name="renderer">The sprite renderer.</param>
        /// <returns>The coroutine.</returns>
        private IEnumerator ShowBuildablePanelSmooth(SpriteRenderer renderer)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0);
            while (renderer.color.a <= 0.999f)
            {
                yield return this.waitForEndOfFrame;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Mathf.SmoothDamp(renderer.color.a, 1f, ref this.colorSmoothChangeVelocity, this.smoothTime, float.MaxValue, Time.unscaledDeltaTime));
            }

            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
            StopAllCoroutines();
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.hintPanelSpriteRender = this.HintPanel.GetComponent<SpriteRenderer>();
            if (this.Furniture != null)
            {
                this.meshRenderer = this.Furniture.GetComponent<MeshRenderer>();
            }
        }
    }
}