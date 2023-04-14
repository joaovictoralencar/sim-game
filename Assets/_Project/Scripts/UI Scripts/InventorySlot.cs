using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Transform _itemDataHolder;
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Button _closeBtn;
    private bool _hasSubSprite;

    public UnityEvent<GameObject> OnBeginDragItem { get; } = new();
    public UnityEvent<EquipItemData> OnEquipItem { get; } = new();
    public UnityEvent<ItemData, int> OnDeleteItem { get; } = new();
    public UnityEvent<InventorySlot, ItemData, int> OnEndDragItem { get; } = new();

    private ItemData _itemData;
    private int _itemAmount = 1;
    private int _index;
    [SerializeField] Vector2 _dragOffset = new Vector2(10, 10);

    private Transform _itemDataHolderPreview;

    public int Index => _index;
    public ItemData ItemData => _itemData;

    public int ItemAmount => _itemAmount;

    public void Initialize(int index)
    {
        _index = index;
        EmptySlot();
    }

    public void SetupItem(ItemData itemData, int amount = 1)
    {
        _itemData = Instantiate(itemData);
        _itemAmount = amount;
        UpdateUI();
        _itemImage.enabled = true;
        _closeBtn.gameObject.SetActive(true);
        //handle sub sprite
        EquipItemData equipItemData = _itemData as EquipItemData;

        _hasSubSprite = equipItemData != null && equipItemData.PartsToChange.Length > 1;
        _itemImage.color = equipItemData == null ? new Color(1, 1, 1, 1) : equipItemData.Color;

        if (equipItemData == null) return;

        if (_hasSubSprite)
            _itemImage.transform.GetChild(0).GetComponent<Image>().sprite =
                equipItemData.PartsToChange[1].IdleFront;
    }

    public void ChangeItem(ItemData itemData, int amount)
    {
        _itemImage.enabled = true;
        _itemData = itemData;
        _itemAmount = amount;
        _closeBtn.gameObject.SetActive(true);
        UpdateUI();
    }

    public void EmptySlot()
    {
        _itemDataHolder.gameObject.SetActive(true);
        _itemData = null;
        _itemAmount = 0;
        _itemAmountText.text = "";
        _itemImage.sprite = null;
        _itemImage.enabled = false;
        _closeBtn.gameObject.SetActive(false);
    }

    void HideSlot()
    {
        _itemDataHolder.gameObject.SetActive(false);
        _closeBtn.gameObject.SetActive(false);
    }

    void ShowSlot()
    {
        _itemDataHolder.gameObject.SetActive(true);
        _closeBtn.gameObject.SetActive(true);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (_itemData == null)
        {
            Debug.LogError("No item data is assigned to " + name, gameObject);
            return;
        }

        _itemImage.sprite = _itemData.Icon;
        _itemAmountText.gameObject.SetActive(ItemAmount > 1);
        _itemAmountText.text = ItemAmount.ToString();
        _itemImage.transform.GetChild(0).gameObject.SetActive(_hasSubSprite);
    }

    public void DeleteItem()
    {
        EmptySlot();
        OnDeleteItem.Invoke(_itemData, _index);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemData == null) return;

        HideSlot();

        _itemDataHolderPreview = Instantiate(_itemDataHolder.gameObject, _itemDataHolder.parent).transform;
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
            EquipItemData equipItemData = _itemData as EquipItemData;
            if (equipItemData != null && equipSlot && _itemData.GetType() == typeof(EquipItemData) &&
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
            OnEndDragItem.Invoke(slot, _itemData, _itemAmount);
            EmptySlot();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_itemDataHolderPreview)
        {
            _itemDataHolderPreview.position = eventData.position - _dragOffset;
        }
    }
}