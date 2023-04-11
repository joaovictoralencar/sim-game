using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Body Part Data")]
public class BodyPartData : ScriptableObject
{
    [Header("Idle")] public Sprite IdleFront;
    public Sprite IdleBack;
    public Sprite IdleSide;

    [Header("Front")] public Sprite[] FrontAnimationSprites;
    public bool DisabledOnFront;
    [Header("Back")] public Sprite[] BackAnimationSprites;
    public bool DisabledOnBack;
    [Header("Side")] public Sprite[] SideAnimationSprites;
    public bool DisabledOnSides;
}