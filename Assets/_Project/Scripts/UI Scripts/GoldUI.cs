using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private PlayerGold _gold;
    [SerializeField] private TextMeshProUGUI[] _goldValueTexts;

    private void Start()
    {
        if (_gold == null) return;
        
        _gold.OnChangeValue.AddListener(OnChangeGold);
    }

    private void OnChangeGold(float changeAmount, float goldValue)
    {
        string goldStringValue = goldValue.ToString("00000");

        for (int i = 0; i < goldStringValue.Length; i++)
        {
            _goldValueTexts[i].text = goldStringValue[i].ToString();
        }
    }
}