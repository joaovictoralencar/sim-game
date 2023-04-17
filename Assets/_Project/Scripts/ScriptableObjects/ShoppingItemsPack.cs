using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShoppingItemsPack : ScriptableObject
{
    public bool randomAmount = true;
    public bool randomItems = true;
    public List<ItemPack> _shoppingItems;
}