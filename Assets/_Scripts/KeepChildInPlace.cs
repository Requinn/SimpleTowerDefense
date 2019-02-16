using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepChildInPlace : MonoBehaviour
{
    private Vector3 _storedRotation;

    private void Start() {
        _storedRotation = transform.rotation.eulerAngles;
    }

    private void FixedUpdate() {
        transform.rotation = Quaternion.Euler(0, 0, _storedRotation.z);
    }
}
