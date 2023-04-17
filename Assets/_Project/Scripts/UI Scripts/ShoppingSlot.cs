using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShoppingSlot : ItemSlot
{
    [SerializeField] private TextMeshProUGUI _priceText;
    private ItemPack _itemPack;
    private Button _btn;
    public UnityEvent<ItemData, int> OnAddItemToBasket { get; } = new();

    
    private void Awake()
    {
        _btn = GetComponentInChildren<Button>();
        //Add on buy listener
        _btn.onClick.AddListener(() => { OnAddItemToBasket.Invoke(itemData, 1); });
    }

    public override void SetupItem(ItemPack itemPack)
    {
        base.SetupItem(itemPack);
        _priceText.text = itemData.Price.ToString();
    }

}