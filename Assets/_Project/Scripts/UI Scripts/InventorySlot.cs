using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Button _closeBtn;

    private ItemData _itemData;
    private int _itemAmount = 1;
    private int _index;
    
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
    }

    public void ChangeItem(ItemData itemData, int amount)
    {
        _itemData = Instantiate(itemData);
        _itemAmount = amount;
        UpdateUI();
    }

    void EmptySlot()
    {
        _itemData = null;
        _itemAmount = 0;
        _itemAmountText.text = "";
        _itemImage.sprite = null;
        _itemImage.enabled = false;
        _closeBtn.gameObject.SetActive(false);
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
    }

    public void DeleteItem()
    {
        EmptySlot();
    }
}