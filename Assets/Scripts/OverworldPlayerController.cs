using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OverworldPlayerController : MonoBehaviour
{
    [Header("Values")]
    [SerializeField, Min(0)] float _Speed = 1;
    [SerializeField, Min(0.001f)] float groundFriction = 10;
    [SerializeField, Min(0)] float _TurnSpeed = 1;

    [Header("Reference")]
    [SerializeField] Rigidbody _RB;
    [SerializeField] Camera _Cam;
    [SerializeField] GameObject _MeshObj;

    Vector3 _direction
    {
        get
        {
            Vector3 targetVector = Input.mousePosition;
            targetVector.z = _Cam.transform.position.y;
            targetVector = _Cam.ScreenToWorldPoint(targetVector) - transform.position;
            targetVector.y = 0;
            targetVector.Normalize();
            return targetVector;
        }
    }
    Quaternion _dirAsRot => Quaternion.LookRotation(_direction, Vector3.up);

    Vector3 _currentMovement = Vector3.zero;

    private void Start()
    {
        _RB = GetComponent<Rigidbody>();
        _RB.useGravity = false;
        _RB.isKinematic = true;
        _RB.angularDrag = 0;
        _RB.drag = 0;
    }

    void Update()
    {
        _MeshObj.transform.rotation = Quaternion.Lerp(_MeshObj.transform.rotation, _dirAsRot, Time.deltaTime * _TurnSpeed);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * _Speed * Time.deltaTime;
        _currentMovement = Vector3.Lerp(_currentMovement, movement, groundFriction * Time.deltaTime);

        _RB.MovePosition(transform.position + _currentMovement);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _direction + transform.position);
    }
}
