using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class InventorySlot : ItemSlot, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public UnityEvent<GameObject> OnBeginDragItem { get; } = new();
    public UnityEvent<EquipItemData> OnEquipItem { get; } = new();
    public UnityEvent<InventorySlot> OnDeleteItem { get; } = new();
    public UnityEvent<InventorySlot, InventorySlot, ItemData, int> OnEndDragItem { get; } = new();

    private Transform _itemDataHolderPreview;

    public void Initialize(int slotIndex)
    {
        index = slotIndex;
        EmptySlot();
    }

    public void DeleteItem()
    {
        if (!itemData.IsRemoveable) return;
        OnDeleteItem.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemData == null) return;
        showInfoUI = false;

        HideSlot(true);

        _itemDataHolderPreview = Instantiate(itemDataHolder.gameObject, itemDataHolder.parent).transform;
        _itemDataHolderPreview.gameObject.SetActive(true);
        _itemDataHolderPreview.GetComponentInChildren<Image>().raycastTarget = false;

        RectTransform rt = _itemDataHolderPreview.GetComponentInChildren<RectTransform>();

        Vector2 parentSize = rt.parent.GetComponent<RectTransform>().sizeDelta;

        //Adjust pivots and remove stretch
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = parentSize;

        OnBeginDragItem.Invoke(_itemDataHolderPreview.gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemData == null) return;
        showInfoUI = true;
        if (_itemDataHolderPreview)
            Destroy(_itemDataHolderPreview.gameObject);

        //end drag off UI
        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            ShowSlot();
            return;
        }

        //end drag on UI
        InventorySlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>();
        if (slot == null || slot == this)
        {
            EquipInventorySlot equipSlot =
                eventData.pointerCurrentRaycast.gameObject.GetComponent<EquipInventorySlot>();

            //equip item
            EquipItemData equipItemData = itemData as EquipItemData;
            if (equipItemData != null && equipSlot && itemData.GetType() == typeof(EquipItemData) &&
                equipSlot.EquipBodyPart == equipItemData.Part)
            {
                OnEquipItem.Invoke(equipItemData);
                equipSlot.SetupItem(equipItemData);
                EmptySlot();
            }
            else //invalid drop
            {
                ShowSlot();
            }
        }
        else
        {
            HideSlot();
            OnEndDragItem.Invoke(this, slot, itemData, itemAmount);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemData == null) return;

        if (_itemDataHolderPreview)
        {
            _itemDataHolderPreview.position = eventData.position - pointerOffset;
        }
    }
}