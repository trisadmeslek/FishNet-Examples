using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private Camera _camera;

    private void Update()
    {
        if (_camera == null)
            _camera = Camera.main;
        if (_camera == null)
            return;

        transform.LookAt(_camera.transform); 
    }
}
