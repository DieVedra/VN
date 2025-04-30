using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "BodySpriteData", menuName = "Character/BodySpriteData", order = 51)]
public class BodySpriteData : ScriptableObject
{
    [SerializeField] private MySprite _body;
    
    [SerializeField, Expandable] private SpriteData _emotionsData;
    
    public MySprite Body => _body;
    public SpriteData EmotionsData => _emotionsData;
}