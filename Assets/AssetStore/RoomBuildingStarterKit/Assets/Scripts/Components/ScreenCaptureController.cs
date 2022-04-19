namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// The ScreenCaptureController class used to capture screen.
    /// </summary>
    public class ScreenCaptureController : MonoBehaviour
    {
        /// <summary>
        /// The screen capture camera.
        /// </summary>
        public Camera ScreenCaptureCam;

        [Tooltip("The screen shot texture width and height.")]
        public Vector2Int ScreenShotTextureSize = new Vector2Int(1024, 1024);

        /// <summary>
        /// Captures screen.
        /// </summary>
        /// <param name="fileName">The file to save screen capture.</param>
        public void CaptureScreen(string fileName)
        {
            this.ScreenCaptureCam.gameObject.SetActive(true);
            RenderTexture rt = new RenderTexture(this.ScreenShotTextureSize.x, this.ScreenShotTextureSize.y, 24);
            this.ScreenCaptureCam.targetTexture = rt;
            Texture2D screenShot = new Texture2D(this.ScreenShotTextureSize.x, this.ScreenShotTextureSize.y, TextureFormat.RGB24, false);
            this.ScreenCaptureCam.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, this.ScreenShotTextureSize.x, this.ScreenShotTextureSize.y), 0, 0);
            this.ScreenCaptureCam.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string directoryPath = $@"{Application.dataPath}/RoomBuildingStarterKit/Save/ScreenShot";
            string filePath = $@"{directoryPath}/{fileName}.png";

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllBytes(filePath, bytes);
            this.ScreenCaptureCam.gameObject.SetActive(false);
        }

        /// <summary>
        /// Executes when enable gameObject.
        /// </summary>
        private void OnEnable()
        {
            EventManager.RegisterEvent(EventManager.Event.Save, this, nameof(this.CaptureScreen));
        }

        /// <summary>
        /// Executes when disable gameObject.
        /// </summary>
        private void OnDisable()
        {
            EventManager.UnRegisterEvent(EventManager.Event.Save, this, nameof(this.CaptureScreen));
        }
    }
}