using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class EquipInventorySlot : ItemSlot, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private BodyPart _equipBodyPart;

    public UnityEvent<GameObject> OnBeginDragItem { get; } = new();
    public UnityEvent<InventorySlot, EquipItemData> OnUnequip { get; } = new();

    private EquipItemData _equipItemData;

    private Transform _itemDataHolderPreview;

    public BodyPart EquipBodyPart => _equipBodyPart;


    public void SetupItem(EquipItemData equipItemData)
    {
        itemData = Instantiate(equipItemData);
        _equipItemData = itemData as EquipItemData;
        SetupItemEquipableItem();
    }

    private void SetupItemEquipableItem()
    {
        itemAmount = 1;

        ShowSlot();

        //handle sub sprite
        hasSubSprite = _equipItemData.PartsToChange.Length > 1;
        itemImage.color = _equipItemData.Color;
        itemImage.gameObject.SetActive(true);


        if (itemImage.transform.childCount == 0) return;
        Image subSpriteImage = itemImage.transform.GetChild(0).GetComponent<Image>();
        if (!hasSubSprite)
        {
            subSpriteImage.gameObject.SetActive(false);
            return;
        }

        subSpriteImage.gameObject.SetActive(true);
        subSpriteImage.sprite = _equipItemData.PartsToChange[1].IdleFront;
        subSpriteImage.color = _equipItemData.Color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_equipItemData == null) return;
        showInfoUI = false;

        HideSlot();

        _itemDataHolderPreview = Instantiate(itemImage.gameObject, itemDataHolder.parent).transform;
        _itemDataHolderPreview.gameObject.SetActive(true);
        _itemDataHolderPreview.GetComponentInChildren<Image>().raycastTarget = false;

        RectTransform rt = _itemDataHolderPreview.GetComponentInChildren<RectTransform>();

        Vector2 parentSize = rt.parent.GetComponent<RectTransform>().sizeDelta / 2;

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
        if (_equipItemData == null) return;
        if (_itemDataHolderPreview)
            Destroy(_itemDataHolderPreview.gameObject);

        showInfoUI = true;
        //end drag off UI
        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            ShowSlot();
            return;
        }

        //end drag on UI

        //UnEquip
        InventorySlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>();
        if (slot)
        {
            if (slot.ItemData == null)
            {
                slot.SetupItem(new ItemPack
                {
                    Amount = 1,
                    ItemData = _equipItemData
                });
                OnUnequip.Invoke(slot, _equipItemData);
                EmptySlot();
            }
            else
            {
                ShowSlot();
            }
        }
        else
        {
            ShowSlot();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_itemDataHolderPreview)
        {
            _itemDataHolderPreview.position = eventData.position - pointerOffset;
        }
    }
}