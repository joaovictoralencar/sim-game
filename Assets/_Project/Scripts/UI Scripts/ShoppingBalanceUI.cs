using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShoppingBalanceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _totalText;
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private Shopping _shopping;

    private float totalPrice, balancePrice;

    private void Start()
    {
        _shopping.OnAddItemToBasket.AddListener(OnAddItemToBasket);
        _shopping.OnOpenShopping.AddListener(OnOpenShopping);
        _shopping.OnBuyItems.AddListener(OnPurchaseItems);
    }

    private void OnPurchaseItems(PlayerGold playerGold)
    {
        ResetBalanceUI();
    }

    private void OnOpenShopping()
    {
        ResetBalanceUI();
    }

    void ResetBalanceUI()
    {
        totalPrice = 0;
        balancePrice = 0;
        UpdateUI();
    }
    void UpdateUI()
    {
        _totalText.text = totalPrice.ToString();
        _balanceText.text = balancePrice.ToString();
    }

    private void OnAddItemToBasket(ItemPack itemPack, PlayerGold playerGold)
    {
        totalPrice += itemPack.ItemData.Price;
        balancePrice = playerGold.CurrentValue - totalPrice;
        UpdateUI();
    }
    
    public void OnRemoveItemToBasket(ItemPack itemPack, PlayerGold playerGold)
    {
        totalPrice -= itemPack.ItemData.Price;
        balancePrice = playerGold.CurrentValue - totalPrice;
        UpdateUI();
    }
    
}