namespace RoomBuildingStarterKit.Common
{
    using System.IO;

    /// <summary>
    /// The image helper class.
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// Gets the image bytes.
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <returns>The image bytes.</returns>
        public static byte[] GetImageByte(string imagePath)
        {
            FileStream files = new FileStream(imagePath, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            return imgByte;
        }
    }
}