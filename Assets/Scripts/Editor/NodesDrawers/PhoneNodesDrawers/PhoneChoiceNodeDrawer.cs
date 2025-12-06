
[CustomNodeEditor(typeof(ChoicePhoneNode))]

public class PhoneChoiceNodeDrawer : ChoiceNodeDrawer
{
    private ChoicePhoneNode _choicePhoneNode;

    public override void OnBodyGUI()
    {
        if (_choicePhoneNode == null)
        {
            _choicePhoneNode = target as ChoicePhoneNode;
            NodeForCallMethods = _choicePhoneNode;
        }
        else
        {
            base.OnBodyGUI();
        }
    }
}