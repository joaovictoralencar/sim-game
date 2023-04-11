using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[ExecuteAlways]
public class SpriteManager : MonoBehaviour
{
    [SerializeField] float _animationFrameTime = .16f;
    [SerializeField] private Color _skinTone = new(1, 1, 1, 1);
    [SerializeField] private Color _hairColor = new(1, 1, 1, 1);

    [Header("Assign Body Parts")] [SerializeField]
    private BodyPartAnimation _hair;

    [SerializeField] private BodyPartAnimation _head;
    [SerializeField] private BodyPartAnimation _eyes;
    [SerializeField] private BodyPartAnimation _torso;
    [SerializeField] private BodyPartAnimation _hands;
    [SerializeField] private BodyPartAnimation _legs;

    public float AnimationFrameTime => _animationFrameTime;

    public UnityEvent<Color> OnChangeSkinTone;
    public UnityEvent<Color> OnChangeHairColor;

    private void Start()
    {
        if (_head)
            OnChangeSkinTone.AddListener(_head.ChangeSpriteColor);

        if (_torso)
            OnChangeSkinTone.AddListener(_torso.ChangeSpriteColor);

        if (_hands)
            OnChangeSkinTone.AddListener(_hands.ChangeSpriteColor);

        if (_legs)
            OnChangeSkinTone.AddListener(_legs.ChangeSpriteColor);

        if (_hair)
            OnChangeHairColor.AddListener(_hair.ChangeSpriteColor);
    }

    private void OnValidate()
    {
        OnChangeSkinTone?.Invoke(_skinTone);
        OnChangeHairColor?.Invoke(_hairColor);
    }
}