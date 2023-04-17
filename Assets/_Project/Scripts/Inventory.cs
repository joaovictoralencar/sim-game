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
    [SerializeField] private Shopping _shopping;
    private InventorySlot[] _inventorySlotItems;
    private int _itemsInInventory;
    private EquipItem _equipItem;
    private bool _canOpenCloseInventory = true;

    public ItemInfoUI ItemInfoUI;
    public RectTransform PreviewHolder => _previewHolder;

    private void Awake()
    {
        _equipItem = GetComponent<EquipItem>();
        InitializeSlots();
        _shopping.OnOpenShopping.AddListener(OnOpenShopping);
        _shopping.OnCloseShopping.AddListener(OnCloseShopping);
    }

    private void OnOpenShopping()
    {
        _canOpenCloseInventory = false;
    }

    private void OnCloseShopping()
    {
        _canOpenCloseInventory = true;
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
        _inventoryItems = new List<ItemPack>(_inventorySize);
        _itemsInInventory = 0;
        foreach (var slot in _inventorySlotItems)
        {
            if (slot.ItemData)
            {
                slot.ChangeItem(slot.ItemData, slot.ItemAmount);
                _inventoryItems.Add(new ItemPack
                {
                    Amount = slot.ItemAmount,
                    ItemData = slot.ItemData
                });
                _itemsInInventory++;
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
            slot.ItemInfoUI = ItemInfoUI;
            slot.Initialize(i);
            slot.OnBeginDragItem.AddListener(OnBeginDragItem);
            slot.OnEndDragItem.AddListener(OnDropItemInSlot);
            slot.OnDeleteItem.AddListener(DeleteItem);
            if (_equipItem) slot.OnEquipItem.AddListener(_equipItem.OnEquipItem);
            _inventorySlotItems[i] = slot;
        }

        AddItemPack();
        RefreshInventory();
    }

    private void DeleteItem(InventorySlot slot)
    {
        slot.EmptySlot();
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
                slot.SetupItem(new ItemPack
                {
                    Amount = amount,
                    ItemData = itemData
                });
            }
        }

        RefreshInventory();
    }

    private void OnDropItemInSlot(InventorySlot originSlot, InventorySlot droppedSlot, ItemData itemData, int amount)
    {
        //empty slot
        if (droppedSlot.ItemData == null)
        {
            droppedSlot.SetupItem(new ItemPack
            {
                Amount = amount,
                ItemData = itemData
            });
            DeleteItem(originSlot);
        }
        else if (droppedSlot.ItemData.ItemName == itemData.ItemName && itemData.CanStack)
        {
            //stack
            droppedSlot.SetupItem(new ItemPack
            {
                Amount = amount + droppedSlot.ItemAmount,
                ItemData = itemData
            });
            DeleteItem(originSlot);
        }
        else
        {
            //occupied slot
            originSlot.ShowSlot();
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
    void AddItemPack()
    {
        foreach (var itemPack in _inventoryItems)
        {
            AddItemPack(itemPack);
        }
    }

    public int GetInventorySpace()
    {
        return _inventorySize - _itemsInInventory;
    }
}

[System.Serializable]
public class ItemPack
{
    public int Amount;
    public ItemData ItemData;
}