using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BodyPartAnimation : MonoBehaviour
{
    [SerializeField] private BodyPartData _bodyPartData;
    
    private SpriteRenderer _spriteRenderer;
    private Sprite[] _currentAnimationSprites;
    private PlayerMovement _playerMovement;
    private int _animationFrameIndex;
    private bool _wasMoving;
    private float _animationTime;
    private MovementDirection _lastDirection = MovementDirection.Side;
    private SpriteManager _spriteManager;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
        _spriteManager = GetComponentInParent<SpriteManager>();
        SetSpriteListByMovement();
    }

    public void ChangeSpriteColor(Color newColor)
    {
        newColor.a = 1;
        _spriteRenderer.color = newColor;
    }

    private void Update()
    {
        SetSpriteListByMovement();
    }

    private void FixedUpdate()
    {
        AnimateSprites();
    }

    void SetSpriteListByMovement()
    {
        if (_playerMovement == null) return;
        //Moving
        bool moving = _playerMovement.Movement.sqrMagnitude > 0.01;

        if (_lastDirection == _playerMovement.MovementDirection && _wasMoving == moving) return;

        _animationTime = 0;
        _wasMoving = moving;
        _lastDirection = _playerMovement.MovementDirection;

        switch (_playerMovement.MovementDirection)
        {
            case MovementDirection.Front:
                _spriteRenderer.enabled = !_bodyPartData.DisabledOnFront;

                if (moving && _bodyPartData.FrontAnimationSprites.Length > 0)
                    SetCurrentSpriteList(_bodyPartData.FrontAnimationSprites);
                else
                    SetCurrentSpriteList(_bodyPartData.IdleFront);

                break;
            case MovementDirection.Back:
                _spriteRenderer.enabled = !_bodyPartData.DisabledOnBack;

                if (moving && _bodyPartData.BackAnimationSprites.Length > 0)
                    SetCurrentSpriteList(_bodyPartData.BackAnimationSprites);
                else SetCurrentSpriteList(_bodyPartData.IdleBack);

                break;
            case MovementDirection.Side:
                _spriteRenderer.enabled = !_bodyPartData.DisabledOnSides;

                if (moving && _bodyPartData.SideAnimationSprites.Length > 0)
                    SetCurrentSpriteList(_bodyPartData.SideAnimationSprites);
                else SetCurrentSpriteList(_bodyPartData.IdleSide);

                break;
        }
    }

    private void SetCurrentSpriteList(Sprite[] newSprites)
    {
        _currentAnimationSprites = newSprites;
    }

    private void SetCurrentSpriteList(Sprite sprite)
    {
        if (sprite == null) return;
        _currentAnimationSprites = new[] { sprite };
        _animationFrameIndex = 0;
    }

    void AnimateSprites()
    {
        if (_currentAnimationSprites.Length == 0 || _animationFrameIndex > _currentAnimationSprites.Length - 1) return;

        if (_currentAnimationSprites.Length == 1)
        {
            _spriteRenderer.sprite = _currentAnimationSprites[0];
            return;
        }

        if (_animationTime <= 0)
        {
            //Set current frame
            _spriteRenderer.sprite = _currentAnimationSprites[_animationFrameIndex];

            //Next frame
            _animationFrameIndex++;

            //Loops framesList
            if (_animationFrameIndex > _currentAnimationSprites.Length - 1) _animationFrameIndex = 0;

            _animationTime = _spriteManager.AnimationFrameTime; //reset frame time
        }
        else
        {
            _animationTime -= Time.deltaTime;
        }
    }
}