using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasketSlot : ItemSlot
{
    public UnityEvent<ItemData, int> OnDeleteItem { get; } = new();
    
    public void RemoveFromBasket()
    {
        if (itemData == null) return;
        ItemData itemDataClone = itemData;
        int amountClone = itemAmount;
        EmptySlot();
        OnDeleteItem.Invoke(itemDataClone, amountClone);
    }
}