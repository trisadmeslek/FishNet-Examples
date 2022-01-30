using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : NetworkBehaviour
{
    public float RotateSpeed = 150f;
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
        transform.Rotate(new Vector3(0f, horizontal * RotateSpeed * Time.deltaTime));
        Vector3 offset = new Vector3(0f, Physics.gravity.y, vertical) * (MoveSpeed * Time.deltaTime);
        offset = transform.TransformDirection(offset);

        _characterController.Move(offset);

        bool moving = (horizontal != 0f || vertical != 0f);
        _animating.SetMoving(moving);
        if (Input.GetKeyDown(KeyCode.Space))
            _animating.Jump();
    }

}
