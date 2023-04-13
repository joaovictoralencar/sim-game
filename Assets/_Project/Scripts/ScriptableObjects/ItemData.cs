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

    [SerializeField] private bool _canStack;

    [SerializeField, Tooltip("Does this item go into an inventory slot?")]
    private bool _hasSlot;

    [SerializeField, Tooltip("Can this item be dropped or destroyed?")]
    private bool _isRemoveable;

    public Sprite Icon => _icon;

    public string ItemName => _itemName;

    public bool CanStack => _canStack;

    public bool HasSlot => _hasSlot;

    public bool IsRemoveable => _isRemoveable;

    public string Description => _description;

}