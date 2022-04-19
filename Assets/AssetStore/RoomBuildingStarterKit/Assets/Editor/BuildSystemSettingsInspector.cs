namespace RoomBuildingStarterKit.Editor
{
    using RoomBuildingStarterKit.BuildSystem;
    using UnityEditor;

    /// <summary>
    /// The BuildSystemSettingsInspector used to define inspector layout of the BuildSystemSettings scriptableobject.
    /// </summary>
    [CustomEditor(typeof(BuildSystemSettings))]
    public class BuildSystemSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("The value can't be changed during runtime!\n" +
                "The default grid (floor) size is 2 meters. When change it to other value, you need to replace all the built-in BuildSystem models to your models with the right size compatible with the grid (floor) size.\n" +
                "Please follow the document \"How to change grid size\" to replace the models.", MessageType.Warning);
        }
    }
}