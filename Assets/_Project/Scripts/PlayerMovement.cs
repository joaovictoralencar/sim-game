using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5; // Start is called before the first frame update

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

    private void FixedUpdate()
    {
        transform.position += _movement.normalized * _speed * Time.deltaTime;
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