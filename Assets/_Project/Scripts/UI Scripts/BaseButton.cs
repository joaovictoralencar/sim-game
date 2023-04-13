using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _btn;
    public UnityEvent<PointerEventData> onPointerEnter;
    public UnityEvent<PointerEventData> onPointerExit;

    private void Awake()
    {
        if (_btn == null) _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        //Play button SFX
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke(eventData);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke(eventData);
    }
}