using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask _npcLayer;
    [SerializeField] private float _interactionDistance = 2;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private int fingerID = -1;

    private void Awake()
    {
#if !UNITY_EDITOR
    fingerID = 0;
#endif
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse click was on the NPC collider
            if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, _npcLayer);

            if (hit.collider == null) return;
            if (!hit.collider.isTrigger) return;

            if (Vector2.Distance((Vector2)transform.position, hit.point) > _interactionDistance) return;

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactable.OnInteract();
            }
        }
    }
}