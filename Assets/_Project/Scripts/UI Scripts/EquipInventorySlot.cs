using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class EquipInventorySlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private RectTransform _itemDataHolder;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _itemImagePlaceholder;

    public UnityEvent<GameObject> OnBeginDragItem { get; } = new();
    public UnityEvent<InventorySlot, EquipItemData> OnUnequip { get; } = new();

    private EquipItemData _itemData;
    [SerializeField] Vector2 _dragOffset = new Vector2(10, 10);

    private Transform _itemDataHolderPreview;

    public ItemData ItemData => _itemData;

    public void Initialize(int index)
    {
        _itemImagePlaceholder.gameObject.SetActive(true);
        EmptySlot();
    }

    public void SetupItem(EquipItemData itemData)
    {
        _itemData = Instantiate(itemData);
        ShowSlot();
    }

    public void ChangeItem(EquipItemData itemData)
    {
        _itemData = itemData;
        ShowSlot();
    }

    void EmptySlot()
    {
        HideSlot();
        _itemData = null;
        _itemImage.sprite = null;
    }

    void HideSlot()
    {
        _itemImage.gameObject.SetActive(false);
        _itemImagePlaceholder.gameObject.SetActive(true);
    }

    void ShowSlot()
    {
        _itemImage.gameObject.SetActive(true);
        _itemImagePlaceholder.gameObject.SetActive(false);
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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemData == null) return;

        HideSlot();

        _itemDataHolderPreview = Instantiate(_itemImage.gameObject, _itemDataHolder.parent).transform;
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
        if (_itemDataHolderPreview)
            Destroy(_itemDataHolderPreview.gameObject);

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
                slot.SetupItem(_itemData);
                OnUnequip.Invoke(slot, _itemData);
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
            _itemDataHolderPreview.position = eventData.position - _dragOffset;
        }
    }
}