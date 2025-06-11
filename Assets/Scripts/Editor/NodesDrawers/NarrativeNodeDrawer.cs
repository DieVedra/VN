using System.Reflection;
using XNodeEditor;

[CustomNodeEditor(typeof(NarrativeNode))]
public class NarrativeNodeDrawer : NodeEditor
{
    private const int _maxCountSymbols = 200;
    
    private NarrativeNode _narrativeNode;
    private MethodInfo _privateMethod;
    private TextNodeDrawer _textNodeDrawer;

    public override void OnBodyGUI()
    {
        if (_textNodeDrawer == null)
        {
            _narrativeNode = target as NarrativeNode;
            _privateMethod = _narrativeNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
            _textNodeDrawer = new TextNodeDrawer(
                serializedObject.FindProperty("_localizationText"),
                serializedObject,
                ()=> { _privateMethod.Invoke(_narrativeNode, null); },
                "Narrative text: ",
                _maxCountSymbols);
        }
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
        _textNodeDrawer.DrawGUI();
    }
}