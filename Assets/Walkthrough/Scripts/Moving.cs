using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : NetworkBehaviour
{

    public float MoveSpeed = 5f;
    private CharacterController _characterController;
    private Animating _animating;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animating = GetComponent<Animating>();
    }
    private void Update()
    {
        if (!base.IsOwner)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 offset = new Vector3(horizontal, Physics.gravity.y, vertical) * (MoveSpeed * Time.deltaTime);

        _characterController.Move(offset);

        bool moving = (horizontal != 0f || vertical != 0f);
        _animating.SetMoving(moving);
        if (Input.GetKeyDown(KeyCode.Space))
            _animating.Jump();
    }

}
