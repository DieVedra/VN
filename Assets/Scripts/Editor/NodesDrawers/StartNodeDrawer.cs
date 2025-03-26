
using XNodeEditor;

[CustomNodeEditor(typeof(StartNode))]
public class StartNodeDrawer : NodeEditor
{
    public override void OnBodyGUI()
    {
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
    }
}