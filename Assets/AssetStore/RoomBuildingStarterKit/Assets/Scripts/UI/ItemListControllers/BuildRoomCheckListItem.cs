namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using UnityEngine;
    
    /// <summary>
    /// The BuildRoomCheckListItem class.
    /// </summary>
    public class BuildRoomCheckListItem : MonoBehaviour
    {
        /// <summary>
        /// The text.
        /// </summary>
        public TextMeshProUGUIWrapper Text;

        /// <summary>
        /// The ok image.
        /// </summary>
        public GameObject YesImage;

        /// <summary>
        /// The no image.
        /// </summary>
        public GameObject NoImage;

        /// <summary>
        /// Sets text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="args">The string format arguments.</param>
        public void SetText(UIText text, params string[] args)
        {
            this.Text.SetGlobalText(text, args);
        }

        /// <summary>
        /// Sets the check result.
        /// </summary>
        /// <param name="result">The check result.</param>
        public void SetResult(bool result)
        {
            this.YesImage.SetActive(result);
            this.NoImage.SetActive(!result);
        }
    }
}
