using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Item Data", menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private Sprite _icon;

    [SerializeField] private string _itemName;

    [TextArea] [SerializeField] private string _description;
    [SerializeField] private float _price = 10;

    [SerializeField] private bool _canStack;

    [SerializeField, Tooltip("Does this item go into an inventory slot?")]
    private bool _hasSlot;

    [SerializeField, Tooltip("Can this item be dropped or destroyed?")]
    private bool _isRemoveable;

    public Sprite Icon
    {
        get => _icon;
        set => _icon = value;
    }

    public string ItemName
    {
        get => _itemName;
        set => _itemName = value;
    }

    public bool CanStack
    {
        get => _canStack;
        set => _canStack = value;
    }

    public bool HasSlot
    {
        get => _hasSlot;
        set => _hasSlot = value;
    }

    public bool IsRemoveable
    {
        get => _isRemoveable;
        set => _isRemoveable = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public float Price
    {
        get => _price;
        set => _price = value;
    }
}