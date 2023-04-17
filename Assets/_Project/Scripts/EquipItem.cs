using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipItem : MonoBehaviour
{
    [SerializeField] private RectTransform _equipItemsUIHolder;
    [SerializeField] private BodyPartAnimation _hair, _hat, _shirt, _sleeve, _pants;

    private EquipInventorySlot[] _equipInventorySlots;
    private Inventory _inventory;

    private void Start()
    {
        _equipInventorySlots = _equipItemsUIHolder.GetComponentsInChildren<EquipInventorySlot>();
        _inventory = GetComponent<Inventory>();
        foreach (EquipInventorySlot slot in _equipInventorySlots)
        {
            slot.ItemInfoUI = _inventory.ItemInfoUI;
            slot.OnUnequip.AddListener(UnequipItem);
            slot.OnBeginDragItem.AddListener(OnBeginDragItem);
        }
    }

    private void OnBeginDragItem(GameObject itemPreview)
    {
        itemPreview.transform.SetParent(_inventory.PreviewHolder);
    }

    private void UnequipItem(InventorySlot slot, EquipItemData itemData)
    {
        switch (itemData.Part)
        {
            case BodyPart.Hair:
                _hair.UnEquip();
                break;
            case BodyPart.Hat:
                _hat.UnEquip();
                break;
            case BodyPart.Shirt:
                _shirt.UnEquip();
                _sleeve.UnEquip();
                break;
            case BodyPart.Pants:
                _pants.UnEquip();
                break;
        }
    }

    public void OnEquipItem(EquipItemData itemData)
    {
        if (itemData.PartsToChange.Length == 0) return;

        switch (itemData.Part)
        {
            case BodyPart.Hair:
                _hair.Equip(itemData.PartsToChange[0], itemData.Color);
                break;
            case BodyPart.Hat:
                _hat.Equip(itemData.PartsToChange[0], itemData.Color);
                break;
            case BodyPart.Shirt:
                _shirt.Equip(itemData.PartsToChange[0], itemData.Color);
                _sleeve.Equip(itemData.PartsToChange[1], itemData.Color);
                break;
            case BodyPart.Pants:
                _pants.Equip(itemData.PartsToChange[0], itemData.Color);
                break;
        }
    }
}