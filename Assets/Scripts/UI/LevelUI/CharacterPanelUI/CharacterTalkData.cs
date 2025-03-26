

public class CharacterTalkData
{
    public readonly DirectionType DirectionType;
    public readonly string Name;
    public readonly string TalkText;

    public CharacterTalkData(DirectionType directionType, string name, string talkText)
    {
        DirectionType = directionType;
        Name = name;
        TalkText = talkText;
    }
}