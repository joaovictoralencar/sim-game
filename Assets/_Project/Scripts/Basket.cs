using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Basket : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Shopping _shopping;
    [SerializeField] private int _maxBasketItemsCount = 16;
    [SerializeField] private RectTransform _basketItemsHolder;
    [SerializeField] private BasketSlot _basketSlotPrefab;
    [SerializeField] private ItemInfoUI _itemInfoUI;
    List<ItemPack> _basketItems;
    private BasketSlot[] _basketSlotItems;
    public ItemInfoUI ItemInfoUI;
    
    public UnityEvent<ItemData, int> OnDeleteItem { get; } = new();


    private void Start()
    {
        _shopping.OnBuyItems.AddListener(OnPurchaseItems);
        _shopping.OnCloseShopping.AddListener(RemoveAllFromBasket);
        _shopping.OnOpenShopping.AddListener(ResetBasket);
    }

    private void ResetBasket()
    {
        InitializeBasket();
    }

    public void RemoveAllFromBasket()
    {
        for (int i = _basketSlotItems.Length - 1; i >= 0; i--)
        {
            _basketSlotItems[i].RemoveFromBasket();
        }
    }

    public void InitializeBasket()
    {
        _basketSlotItems = new BasketSlot[_shopping.ShoppingMaxItemsCount];
        _basketItems = new List<ItemPack>(_maxBasketItemsCount);
        //Delete old ones
        for (int i = _basketItemsHolder.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_basketItemsHolder.GetChild(i).gameObject);
        }

        //Instantiate slots
        for (int i = 0; i < _maxBasketItemsCount; i++)
        {
            BasketSlot slot = Instantiate(_basketSlotPrefab, _basketItemsHolder);
            slot.ItemInfoUI = _itemInfoUI;
            _basketSlotItems[i] = slot;
            slot.EmptySlot();
            slot.OnDeleteItem.AddListener(DeleteItem);
        }

        RefreshShoppingUI();
    }

    private void DeleteItem(ItemData itemData, int amount)
    {
        _shopping.ReturnItemToShopping(itemData, amount);
        OnDeleteItem.Invoke(itemData, amount);

        ItemPack itemToRemove = default;
        foreach (var itemPack in _basketItems)
        {
            if (itemPack.ItemData.ItemName == itemData.ItemName && itemPack.Amount == amount)
            {
                itemToRemove = itemPack;
            }
        }

        _basketItems.Remove(itemToRemove);
        RefreshShoppingUI();
    }


    public bool AddItemToBasket(ItemData itemData, int amount = 1)
    {
        foreach (var basketSlot in _basketSlotItems)
        {
            if (!basketSlot.ItemData || basketSlot.ItemData.ItemName != itemData.ItemName ||
                !itemData.CanStack) continue;
            
            //stack
            basketSlot.ChangeItem(basketSlot.ItemData, basketSlot.ItemAmount + 1);
            RefreshShoppingUI();
            return true;
        }

        if (!CanAddItem()) return false;

        _basketItems.Add(new ItemPack
        {
            Amount = amount,
            ItemData = itemData
        });

        int lastIndex = _basketItems.Count - 1;
        if (lastIndex < 0) return false;
        _basketSlotItems[lastIndex].SetupItem(_basketItems[lastIndex]);

        RefreshShoppingUI();
        return true;
    }

    private bool CanAddItem()
    {
        int inventorySpace = _inventory.GetInventorySpace();
        int basketSpace = _maxBasketItemsCount - _basketItems.Count;
        if (inventorySpace <= 16) basketSpace -= inventorySpace;
        return basketSpace > 0;
    }

    private void RefreshShoppingUI()
    {
        //RefreshSlots
        foreach (var slot in _basketSlotItems)
        {
            if (slot.ItemData != null)
            {
                slot.ChangeItem(slot.ItemData, slot.ItemAmount);
            }
            else
            {
                slot.EmptySlot();
            }
        }
    }

    void OnPurchaseItems(PlayerGold playerGold)
    {
        foreach (var slot in _basketSlotItems)
        {
            if (slot.ItemData != null)
            {
                _inventory.AddItemPack(new ItemPack
                {
                    Amount = slot.ItemAmount,
                    ItemData = slot.ItemData
                });
                playerGold.ChangeValue(-slot.ItemData.Price);
            }
        }
    }
}