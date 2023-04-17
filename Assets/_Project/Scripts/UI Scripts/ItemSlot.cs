using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public UnityEvent<ItemData, PointerEventData> OnPointerEnterSlot { get; } = new();
    public UnityEvent<ItemData, PointerEventData> OnPointerExitSlot { get; } = new();
    public UnityEvent<ItemData, PointerEventData> OnPointerMoveSlot { get; } = new();

    public ItemData ItemData => itemData;

    public int ItemAmount => itemAmount;

    [SerializeField] protected Transform itemDataHolder;
    [SerializeField] protected Image itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Image _itemImagePlaceholder;
    [SerializeField] protected Vector2 pointerOffset = new Vector2(10, 10);
    public ItemInfoUI ItemInfoUI { get; set; }

    protected ItemData itemData;
    protected int itemAmount = 1;
    protected int index;
    protected bool hasSubSprite;
    protected bool showInfoUI = true;


    public void EmptySlot()
    {
        HideSlot();
        itemData = null;
        itemImage.sprite = null;
        if (_closeBtn)
            _closeBtn.gameObject.SetActive(false);
    }

    public virtual void SetupItem(ItemPack itemPack)
    {
        itemData = Instantiate(itemPack.ItemData);
        if (itemData.CanStack)
            itemAmount = itemPack.Amount;
        else itemAmount = 1;

        ShowSlot();

        if (_closeBtn)
            _closeBtn.gameObject.SetActive(true);

        //handle sub sprite
        EquipItemData equipItemData = itemData as EquipItemData;
        hasSubSprite = equipItemData != null && equipItemData.PartsToChange.Length > 1;
        itemImage.color = equipItemData == null ? new Color(1, 1, 1, 1) : equipItemData.Color;

        if (equipItemData == null) return;

        Image subSpriteImage = itemImage.transform.GetChild(0).GetComponent<Image>();
        if (!hasSubSprite)
        {
            subSpriteImage.gameObject.SetActive(false);
            return;
        }

        subSpriteImage.gameObject.SetActive(true);
        subSpriteImage.enabled = true;
        subSpriteImage.sprite = equipItemData.PartsToChange[1].IdleFront;
        subSpriteImage.color = equipItemData.Color;
    }

    protected void UpdateUI()
    {
        if (itemData == null)
        {
            Debug.LogError("No item data is assigned to " + name, gameObject);
            return;
        }

        if (_closeBtn)
            _closeBtn.gameObject.SetActive(itemData.IsRemoveable);
        itemImage.sprite = itemData.Icon;
        if (_itemAmountText != null)
        {
            _itemAmountText.gameObject.SetActive(ItemAmount > 1);
            _itemAmountText.text = ItemAmount.ToString();
        }

        if (itemImage.transform.childCount > 0)
            itemImage.transform.GetChild(0).gameObject.SetActive(hasSubSprite);
    }

    public void ChangeItem(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        itemAmount = amount;
        ShowSlot();
    }

    protected void HideSlot(bool hideCloseButton = false)
    {
        if (hideCloseButton && _closeBtn) _closeBtn.gameObject.SetActive(false);
        itemDataHolder.gameObject.SetActive(false);
        if (_itemImagePlaceholder == null) return;
        _itemImagePlaceholder.gameObject.SetActive(true);
        if (hasSubSprite) _itemImagePlaceholder.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ShowSlot()
    {
        itemDataHolder.gameObject.SetActive(true);
        UpdateUI();
        if (_itemImagePlaceholder == null) return;
        _itemImagePlaceholder.gameObject.SetActive(false);
        if (hasSubSprite)
            _itemImagePlaceholder.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!showInfoUI || itemData == null) return;
        ItemInfoUI.ShowItemInfo(itemData, eventData.position);
        itemImage.rectTransform.DOScale(new Vector2(1.1f, 1.1f), .15f).SetEase(Ease.OutBack);
        OnPointerEnterSlot.Invoke(itemData, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemData == null) return;
        itemImage.rectTransform.DOScale(new Vector2(1f, 1f), .1f).SetEase(Ease.InExpo);
        ItemInfoUI.HideItemInfo();
        OnPointerExitSlot.Invoke(itemData, eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemData == null) return;
        ItemInfoUI.MoveItemInfo(eventData.position);
        OnPointerMoveSlot.Invoke(itemData, eventData);
    }
}