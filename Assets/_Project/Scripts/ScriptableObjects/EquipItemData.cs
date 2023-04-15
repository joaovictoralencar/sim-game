using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Items/Equip Item Data")]
public class EquipItemData : ItemData
{
    [SerializeField] private BodyPart _part;
    [SerializeField] private BodyPartData[] _partsToChange;
    [SerializeField] private Color _color = new Color(1, 1, 1, 1);

    public BodyPart Part => _part;
    public BodyPartData[] PartsToChange => _partsToChange;
    public Color Color => _color;
}

public enum BodyPart
{
    Hair,
    Hat,
    Shirt,
    Pants
}