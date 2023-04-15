using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 30;
    [SerializeField] private RectTransform _inventoryHolder;
    [SerializeField] private InventorySlot _inventorySlotPrefab;
    [SerializeField] private RectTransform _previewHolder;
    [SerializeField] private Transform _inventoryUI;
    [SerializeField] private ItemInfoUI _itemInfoUI;
    private InventorySlot[] _inventorySlotItems;
    private int _itemsInInventory;
    private EquipItem _equipItem;
    private bool _canOpenCloseInventory = true;

    public RectTransform PreviewHolder => _previewHolder;

    private void Awake()
    {
        _equipItem = GetComponent<EquipItem>();
        InitializeSlots();
    }

    private void Start()
    {
        CloseInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_inventoryUI.gameObject.activeSelf)
                OpenInventory();
            else CloseInventory();
        }
    }

    public void OpenInventory()
    {
        if (!_canOpenCloseInventory) return;

        //Play sound
        RefreshInventory();
        _canOpenCloseInventory = false;
        _inventoryUI.gameObject.SetActive(true);
        _inventoryUI.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack)
            .OnComplete(() => { _canOpenCloseInventory = true; });
    }

    void RefreshInventory()
    {
        //RefreshInventory
        foreach (var slot in _inventorySlotItems)
        {
            if (slot.ItemData)
            {
                slot.ChangeItem(slot.ItemData, slot.ItemAmount);
            }
            else
            {
                slot.EmptySlot();
            }
        }
    }

    public void CloseInventory()
    {
        if (!_canOpenCloseInventory) return;

        //Play sound
        _inventoryUI.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _canOpenCloseInventory = true;
            _inventoryUI.gameObject.SetActive(false);
        });
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
            slot.OnBeginDragItem.AddListener(OnBeginDragItem);
            slot.OnEndDragItem.AddListener(OnDropItemInSlot);
            slot.OnDeleteItem.AddListener(DeleteItem);
            slot.OnPointerEnterSlot.AddListener(OnPointerEnterSlot);
            slot.OnPointerMoveSlot.AddListener(OnPointerMoveSlot);
            slot.OnPointerExitSlot.AddListener(OnPointerExitSlot);
            if (_equipItem) slot.OnEquipItem.AddListener(_equipItem.OnEquipItem);
            _inventorySlotItems[i] = slot;
        }

        AddItemPackQA();
        RefreshInventory();
    }

    public void OnPointerMoveSlot(ItemData itemData, PointerEventData pointerEventData)
    {
        if (itemData)
            _itemInfoUI.MoveItemInfo(pointerEventData.position);
    }

    public void OnPointerExitSlot(ItemData itemData, PointerEventData pointerEventData)
    {
        if (itemData)
            _itemInfoUI.HideItemInfo();
    }

    public void OnPointerEnterSlot(ItemData itemData, PointerEventData pointerEventData)
    {
        if (itemData)
            _itemInfoUI.ShowItemInfo(itemData, pointerEventData.position);
    }

    private void DeleteItem(ItemData itemData, int slotIndex)
    {
        _inventorySlotItems[slotIndex].EmptySlot();

        ItemPack itemToRemove = default;
        foreach (var itemPack in _inventoryItems)
        {
            if (itemPack.ItemData == itemData)
            {
                itemToRemove = itemPack;
            }
        }

        _inventoryItems.Remove(itemToRemove);
        RefreshInventory();
    }

    private void AddItem(ItemData itemData, bool stack, int amount = 1)
    {
        if (_itemsInInventory >= _inventorySize)
        {
            Debug.LogWarning("Inventory is full");
            return;
        }

        if (amount <= 0) amount = 1;

        if (stack && itemData.CanStack)
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

    private void OnDropItemInSlot(InventorySlot slot, ItemData itemData, int amount)
    {
        //empty slot
        if (slot.ItemData == null)
        {
            slot.ChangeItem(itemData, amount);
        }
        else if (slot.ItemData.ItemName == itemData.ItemName && itemData.CanStack)
        {
            //stack
            slot.ChangeItem(itemData, amount + slot.ItemAmount);
        }
        else
        {
            //occupied slot
        }
    }

    private void OnBeginDragItem(GameObject itemPreview)
    {
        itemPreview.transform.SetParent(PreviewHolder);
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


    [Space(10)] public List<ItemPack> _inventoryItems;

    [ContextMenu("AddItem")]
    void AddItemPackQA()
    {
        foreach (var itemPack in _inventoryItems)
        {
            AddItemPack(itemPack);
        }
    }
}

[System.Serializable]
public struct ItemPack
{
    public int Amount;
    public ItemData ItemData;
}