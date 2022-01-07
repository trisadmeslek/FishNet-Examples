using FishNet.Component.Animating;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animating : MonoBehaviour
{
    private Animator _animator;
    private NetworkAnimator _networkAnimator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();
    }
 
    public void SetMoving(bool value)
    {
        _animator.SetBool("Moving", value);
    }
    public void Jump()
    {
        _networkAnimator.SetTrigger("Jump");
    }
}
