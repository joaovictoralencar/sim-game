using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingNPC : MonoBehaviour, IInteractable
{
    public Shopping _shopping;

    public void OnInteract()
    {
        if (_shopping._shoppingUI.gameObject.activeInHierarchy) return;
        _shopping.OpenShopping();
    }
}