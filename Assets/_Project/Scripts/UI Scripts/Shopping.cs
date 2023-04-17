using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Shopping : MonoBehaviour
{
    public ShoppingItemsPack ShoppingItemsPack;
    List<ItemPack> _shoppingItems;
    [SerializeField] int _shoppingMaxItemsCount = 16;
    [SerializeField] private RectTransform _shoppingUIHolder;
    [SerializeField] private ShoppingSlot _shoppingSlotPrefab;
    [SerializeField] private PlayerGold _playerGold;
    [SerializeField] private Basket _basket;
    [SerializeField] private ShoppingBalanceUI _shoppingBalanceUI;
    public RectTransform _shoppingUI;

    public UnityEvent<ItemPack, PlayerGold> OnAddItemToBasket { get; } = new();
    public UnityEvent<PlayerGold> OnBuyItems { get; } = new();
    public UnityEvent OnOpenShopping { get; } = new();
    public UnityEvent OnCloseShopping { get; } = new();
    public UnityEvent<ItemPack> OnRemoveItemFromBasket { get; } = new();
    public ItemInfoUI ItemInfoUI;
    private ShoppingSlot[] _shoppingSlotItems;
    private bool _canOpenCloseShopping = true;

    public int ShoppingMaxItemsCount => _shoppingMaxItemsCount;

    private void Start()
    {
        _basket.OnDeleteItem.AddListener(OnDeleteItemFromBasket);
    }

    private void OnDeleteItemFromBasket(ItemData itemData, int amount)
    {
        _shoppingBalanceUI.OnRemoveItemToBasket(new ItemPack
        {
            Amount = amount,
            ItemData = itemData
        }, _playerGold);
    }

    void Initialize()
    {
        _shoppingItems = new List<ItemPack>(ShoppingItemsPack._shoppingItems);

        if (ShoppingItemsPack.randomItems)
        {
            // Shuffle the list using Fisher-Yates algorithm
            int n = _shoppingItems.Count;
            System.Random rng = new System.Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (_shoppingItems[k], _shoppingItems[n]) = (_shoppingItems[n], _shoppingItems[k]);
            }
        }


        if (ShoppingItemsPack.randomAmount)
        {
            for (int i = 0; i < _shoppingItems.Count; i++)
            {
                var pack = _shoppingItems[i];
                pack.Amount = Random.Range(1, 100);
                _shoppingItems[i] = pack;
            }
        }

        InitializeShopping();
    }

    public void ReturnItemToShopping(ItemData itemData, int amount)
    {
        foreach (var slot in _shoppingSlotItems)
        {
            if (slot.ItemData && slot.ItemData.name == itemData.name)
            {
                slot.ChangeItem(itemData, slot.ItemAmount + amount);
                return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_shoppingUI.gameObject.activeSelf)
                OpenShopping();
            else CloseShopping();
        }
    }

    public void OpenShopping()
    {
        if (!_canOpenCloseShopping) return;

        //Play sound
        Initialize();
        OnOpenShopping.Invoke();
        GetComponent<Image>().enabled = true;
        _canOpenCloseShopping = false;
        _shoppingUI.gameObject.SetActive(true);
        _shoppingUI.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack)
            .OnComplete(() => { _canOpenCloseShopping = true; });
    }

    public void CloseShopping()
    {
        if (!_canOpenCloseShopping) return;
        GetComponent<Image>().enabled = false;
        //Play sound
        _shoppingUI.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _canOpenCloseShopping = true;
            _shoppingUI.gameObject.SetActive(false);
            OnCloseShopping.Invoke();
        });
    }


    private void InitializeShopping()
    {
        _shoppingSlotItems = new ShoppingSlot[ShoppingMaxItemsCount];
        //Delete old ones
        for (int i = _shoppingUIHolder.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_shoppingUIHolder.GetChild(i).gameObject);
        }

        //Instantiate slots
        for (int i = 0; i < ShoppingMaxItemsCount; i++)
        {
            ShoppingSlot slot = Instantiate(_shoppingSlotPrefab, _shoppingUIHolder);
            slot.ItemInfoUI = ItemInfoUI;
            _shoppingSlotItems[i] = slot;
            slot.EmptySlot();
            if (i >= _shoppingItems.Count) return;
            slot.SetupItem(_shoppingItems[i]);
            slot.OnAddItemToBasket.AddListener(TryAddItemToBasket);
        }
    }

    private void TryAddItemToBasket(ItemData itemData, int amount)
    {
        if (!(_playerGold.CurrentValue >= itemData.Price)) return;
        if (_basket.AddItemToBasket(itemData, amount))
        {
            OnAddItemToBasket.Invoke(new ItemPack
            {
                Amount = amount,
                ItemData = itemData
            }, _playerGold);
        }
    }

    public void PurchaseItems()
    {
        OnBuyItems.Invoke(_playerGold);
        _basket.InitializeBasket();
        //CloseShopping();
    }
}