using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 30;
    [SerializeField] private RectTransform _inventoryHolder;
    [SerializeField] private InventorySlot _inventorySlotPrefab;
    private ItemData[] _inventoryItems;
    private InventorySlot[] _inventorySlotItems;
    private int _itemsInInventory;

    private void Awake()
    {
        //Initialize inventory array
        _inventoryItems = new ItemData[_inventorySize];

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        _inventorySlotItems = new InventorySlot[_inventorySize];
        //Delete old ones
        for (int i = _inventoryHolder.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_inventoryHolder.GetChild(i).gameObject);
        }

        //Instantiate slots
        for (int i = 0; i < _inventorySize; i++)
        {
            InventorySlot slot = Instantiate(_inventorySlotPrefab, _inventoryHolder);
            slot.Initialize(i);
            _inventorySlotItems[i] = slot;
        }
    }

    private void AddItem(ItemData itemData, bool stack, int amount = 1)
    {
        if (_itemsInInventory >= _inventorySize)
        {
            Debug.LogWarning("Inventory is full");
            return;
        }

        if (amount <= 0) amount = 1;

        if (stack)
        {
            //look for item
            //add at first empty slot
            InventorySlot slot = GetSlot(itemData);
            if (slot)
            {
                slot.ChangeItem(itemData, slot.ItemAmount + amount);
            }
        }
        else
        {
            //add at first empty slot
            InventorySlot slot = GetFirstEmptySlot();
            if (slot)
            {
                slot.SetupItem(itemData, amount);
            }
        }

        _itemsInInventory++;
    }

    InventorySlot GetFirstEmptySlot()
    {
        foreach (var inventorySlot in _inventorySlotItems)
        {
            if (inventorySlot.ItemData == null)
                return inventorySlot;
        }

        return null;
    }

    InventorySlot GetSlot(ItemData itemData)
    {
        foreach (var inventorySlot in _inventorySlotItems)
        {
            if (inventorySlot.ItemData == itemData)
                return inventorySlot;
        }

        return null;
    }


    public void AddItemPack(ItemPack itemPack)
    {
        AddItem(itemPack.ItemData, false, itemPack.Amount);
    }

    [Space(10), Header("QA")]
    public ItemPack itemToAdd;

    [ContextMenu("AddItem")] 
    void AddItemPackQA()
    {
        AddItemPack(itemToAdd);
    }
}

[System.Serializable]
public struct ItemPack
{
    public int Amount;
    public ItemData ItemData;
}