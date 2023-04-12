using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5; // Start is called before the first frame update
    [SerializeField] private Rigidbody2D _rb; // Start is called before the first frame update
    [SerializeField] private Transform _gfx;
    
    [Header("Walking Animation")]
    [SerializeField] private float _moveUpDownTime = .2f;
    [SerializeField] private float _moveUpDownOffset = .1f;
    
    
    private Vector3 _movement;
    private MovementDirection _movementDirection;

    public Vector3 Movement => _movement;
    public MovementDirection MovementDirection => _movementDirection;

    void Update()
    {
        //Simple movement
        _movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        SetMoveDirection();

        //flip gameobject
        Vector3 flipScale = transform.localScale;
        if (_movementDirection == MovementDirection.Side)
        {
            if (_movement.x > 0) flipScale.x = 1;
            else if (_movement.x < 0) flipScale.x = -1;
            transform.localScale = flipScale;
        }
    }

    private Tween _moveYTween;


    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + _movement.normalized * _speed * Time.deltaTime);

        if (_gfx == null) return;

        if (_movement.magnitude > 0.05f)
        {
            if (_moveYTween == null || !_moveYTween.IsPlaying())
            {
                //start tween
                _moveYTween = _gfx.DOLocalMoveY(_moveUpDownOffset, _moveUpDownTime).SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutQuad);
            }
        }
        else if (_moveYTween != null && _moveYTween.IsPlaying())
        {
            _moveYTween.Restart();
            _moveYTween.Kill();
            _moveYTween = null;
        }
    }

    private void SetMoveDirection()
    {
        if (_movement.x != 0) _movementDirection = MovementDirection.Side;
        else if (_movement.y < 0) _movementDirection = MovementDirection.Front;
        else if (_movement.y > 0) _movementDirection = MovementDirection.Back;
    }
}

public enum MovementDirection
{
    Front,
    Back,
    Side
}