using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopping : MonoBehaviour
{
    public ShoppingItemsPack ShoppingItemsPack;
    List<ItemPack> _shoppingItems;
    [SerializeField] int _shoppingMaxItemsCount = 16;
    [SerializeField] private RectTransform _shoppingUIHolder;
    [SerializeField] private ShoppingSlot _shoppingSlotPrefab;
    [SerializeField] private PlayerGold _playerGold;
    [SerializeField] private Basket _basket;
    public ItemInfoUI ItemInfoUI;

    private ShoppingSlot[] _shoppingSlotItems;

    public int ShoppingMaxItemsCount => _shoppingMaxItemsCount;

    // Start is called before the first frame update
    void Start()
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
            slot.OnBuyItem.AddListener(TryBuyItem);
        }
    }

    private void TryBuyItem(ItemData itemData, int amount)
    {
        if (!(_playerGold.CurrentValue >= itemData.Price)) return;
        if (_basket.AddItemToBasket(itemData, amount))
        {
            _playerGold.ChangeValue(-itemData.Price);
        }
    }
}