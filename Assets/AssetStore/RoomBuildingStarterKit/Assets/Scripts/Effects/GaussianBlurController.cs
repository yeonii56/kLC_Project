namespace RoomBuildingStarterKit.Effect
{
    using RoomBuildingStarterKit.Common;
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;

    /// <summary>
    /// 
    /// </summary>
    public class GaussianBlurController : Singleton<GaussianBlurController>
    {
        /// <summary>
        /// The is bluring or not.
        /// </summary>
        [HideInInspector]
        public bool IsBluring = false;

        /// <summary>
        /// The blur value.
        /// </summary>
        [HideInInspector]
        public float Blur = 3.8f;

        /// <summary>
        /// The post processing effect volume
        /// </summary>
        private PostProcessVolume volume;

        /// <summary>
        /// The depth of field.
        /// </summary>
        private DepthOfField depthOfField;

        /// <summary>
        /// Gets or sets the animator.
        /// </summary>
        public Animator Animator { get; set; }

        /// <summary>
        /// Executes when instantiates gameObject.
        /// </summary>
        protected override void AwakeInternal()
        {
            this.Animator = this.GetComponent<Animator>();

            this.volume = this.GetComponent<PostProcessVolume>();
            this.depthOfField = this.volume.profile.GetSetting<DepthOfField>();
        }

        /// <summary>
        /// Executes each frame.
        /// </summary>
        private void Update()
        {
            if (this.IsBluring)
            {
                this.depthOfField.focusDistance.value = this.Blur;
            }
        }
    }
}