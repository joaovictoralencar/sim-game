using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    [SerializeField] private Vector2 _positionOffset;
    [SerializeField] private RectTransform _rt;

    private void Start()
    {
        _rt = GetComponent<RectTransform>();
    }

    public void ShowItemInfo(ItemData itemData, Vector3 position)
    {
        gameObject.SetActive(true);
        _itemNameText.text = itemData.ItemName;
        _itemDescriptionText.text = itemData.Description;
        _itemPriceText.text = "<b>Price:</b>" + itemData.Price;
        transform.position = position + new Vector3(_positionOffset.x, _positionOffset.y, 0);
        if (_rt == null) _rt = GetComponent<RectTransform>();

        foreach (RectTransform rTransform in _rt.transform)
        {
            ForceLayoutUpdate(rTransform);
        }
    }

    void ForceLayoutUpdate(RectTransform rt)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        foreach (RectTransform rTransform in rt)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rTransform);
        }
    }

    public void MoveItemInfo(Vector3 position)
    {
        transform.position = position + new Vector3(_positionOffset.x, _positionOffset.y, 0);
    }

    public void HideItemInfo()
    {
        gameObject.SetActive(false);
    }
}