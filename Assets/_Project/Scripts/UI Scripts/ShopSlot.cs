using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemPrice;
    [SerializeField] private TextMeshProUGUI _itemAmount;
    private ItemPack _itemPack;

    void SetupItem(ItemPack itemPack)
    {
        _itemPack = itemPack;
        _itemIcon.sprite = itemPack.ItemData.Icon;
        _itemPrice.text = itemPack.ItemData.Price.ToString();
        _itemAmount.text = itemPack.Amount.ToString();
    }

    void OnBuy()
    {
        //move to basket
        //change amount
        //disable if not enough amount
    }
}