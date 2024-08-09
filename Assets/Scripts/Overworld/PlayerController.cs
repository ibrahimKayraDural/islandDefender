namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField, Min(0)] float _Speed = 1;
        [SerializeField, Min(0.001f)] float groundFriction = 10;
        [SerializeField, Min(0)] float _TurnSpeed = 1;

        [Header("Reference")]
        [SerializeField] Rigidbody _RB;
        [SerializeField] Camera _Cam;
        [SerializeField] GameObject _Rotator;
        [SerializeField] GameObject _DebugCube;

        Vector3 _direction
        {
            get
            {
                Ray ray = _Cam.ScreenPointToRay(Input.mousePosition);
                if (MathUtils.InfiniteLinePlaneIntersection
                    (Vector3.up, transform.position, ray.origin, ray.direction,
                    out Vector3 intersection) == false) return transform.forward;

                return (intersection - transform.position).normalized;
            }
        }
        Quaternion _dirAsRot => Quaternion.LookRotation(_direction, Vector3.up);

        Vector3 _currentMovement = Vector3.zero;

        private void Start()
        {
            _RB = GetComponent<Rigidbody>();
            _RB.useGravity = false;
            _RB.angularDrag = 1000000;
            _RB.drag = 1000000;
            _RB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }

        void Update()
        {
            _Rotator.transform.rotation = Quaternion.Lerp(_Rotator.transform.rotation, _dirAsRot, Time.deltaTime * _TurnSpeed);
        }

        void FixedUpdate()
        {
            Quaternion rotation = Quaternion.AngleAxis(_Cam.transform.rotation.eulerAngles.y, Vector3.up);
            Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * _Speed * Time.deltaTime;
            movement = rotation * movement;
            _currentMovement = Vector3.Lerp(_currentMovement, movement, groundFriction * Time.deltaTime);

            _RB.MovePosition(transform.position + _currentMovement);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _direction + transform.position);
        }
    }
}
