namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
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
        [SerializeField] Animator _Animator;
        [SerializeField] GameObject _Rotator;

        Vector3 _direction
        {
            get
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (MathUtils.InfiniteLinePlaneIntersection
                    (Vector3.up, transform.position, ray.origin, ray.direction,
                    out Vector3 intersection) == false) return transform.forward;

                return (intersection - transform.position).normalized;
            }
        }
        Quaternion _dirAsRot => Quaternion.LookRotation(_direction, Vector3.up);
        MovementMode currentMovementMode
        {
            get
            {
                if (_acceptMovementModeModifier)
                {
                    if (_movementModeModifiers.Count > 0) return _movementModeModifiers[0].Item2;
                    else return MovementMode.Normal;
                }

                return MovementMode.Normal;
            }
        }
        Camera _camera => _cameraManager.CurrentCamera;

        CameraManager _cameraManager;

        public List<Tuple<string, MovementMode>> _movementModeModifiers = new List<Tuple<string, MovementMode>>();
        bool _acceptMovementModeModifier = true;

        Vector3 _currentMovement = Vector3.zero;
        Vector3 _oldMovement = Vector3.zero;


        private void Start()
        {
            _RB = GetComponent<Rigidbody>();
            _RB.useGravity = false;
            _RB.angularDrag = 1000000;
            _RB.drag = 1000000;
            _RB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

            _cameraManager = CameraManager.Instance;
        }

        void Update()
        {
            if (Input.GetButtonDown("Inventory")) CanvasManager.Instance?.ToggleInventory();

            if(Time.timeScale <= 0 || _camera == null) return;

            _Rotator.transform.rotation = Quaternion.Lerp(_Rotator.transform.rotation, _dirAsRot, Time.deltaTime * _TurnSpeed);
        }

        void FixedUpdate()
        {
            if (_camera == null) return;

            Vector3 movement = CalculateMovement();

            _currentMovement = Vector3.Lerp(_currentMovement, movement, groundFriction);
            _RB.MovePosition(transform.position + _currentMovement);

            CalculateAnimation(Time.deltaTime);
        }

        Vector3 CalculateMovement()
        {
            Vector3 movement;

            if (currentMovementMode == MovementMode.Repeating)
            {
                movement = _oldMovement;
            }
            else
            {
                Quaternion rotation = Quaternion.AngleAxis(_camera.transform.rotation.eulerAngles.y, Vector3.up);
                movement = TakeInput() * _Speed * Time.deltaTime;
                movement = rotation * movement;

                _oldMovement = movement;
            }

            return movement;
        }

        void CalculateAnimation(float deltaTime)
        {
            Vector3 normalizedVelocity = _currentMovement / deltaTime / _Speed;
            normalizedVelocity.y = 0;
            float nVelMag = normalizedVelocity.magnitude;
            Quaternion InverseRotation = Quaternion.AngleAxis(-_camera.transform.rotation.eulerAngles.y, Vector3.up);

            normalizedVelocity = InverseRotation * normalizedVelocity;
            Vector3 rotDir = InverseRotation * _direction;
            float angleBetween = Vector3.SignedAngle(normalizedVelocity, rotDir, Vector3.up);
            normalizedVelocity = Quaternion.Euler(0, -angleBetween, 0) * Vector3.forward;
            normalizedVelocity = normalizedVelocity.normalized * nVelMag;

            _Animator.SetFloat("X", Mathf.Clamp(normalizedVelocity.x, -1, 1));
            _Animator.SetFloat("Z", Mathf.Clamp(normalizedVelocity.z, -1, 1));
        }

        Vector3 TakeInput()
        {
            Vector3 input;

            switch(currentMovementMode)
            {
                case MovementMode.Repeating:
                case MovementMode.Locked:
                    input = Vector3.zero; break;

                default: //This is MovementMode.Normal
                    input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized; 
                    break;
            }

            return input;
        }

        public void AddMovementMode(string id, MovementMode mode, bool @override = false)
        {
            Tuple<string, MovementMode> value = new Tuple<string, MovementMode>(id, mode);

            //check for override
            if (@override)
            {
                _movementModeModifiers.Insert(0, value);
            }
            else
            {
                int index = _movementModeModifiers.FindIndex(x => x.Item1 == id);
                if (index == -1) //MMM does not contain same id
                {
                    _movementModeModifiers.Add(value);
                }
                else //MMM contains id, replace it
                {
                    _movementModeModifiers[index] = value;
                }
            }
        }
        public void RemoveMovementMode(string ID)
        {
            int index = _movementModeModifiers.FindIndex(x => x.Item1 == ID);
            if(index != -1) _movementModeModifiers.RemoveAt(index);
        }
        public void AddMovementModifierForSeconds(string id, MovementMode mode, float seconds, bool @override = false)
        {
            AddMovementMode(id, mode, @override);

            IEnumerator IE = RemoveMMAfterSeconds(id, seconds);
            int index = _startedCoroutines.FindIndex(x => x.Item1 == id);

            if (index != -1)
            {
                StopCoroutine(_startedCoroutines[index].Item2);
                _startedCoroutines.RemoveAt(index);
            }

            _startedCoroutines.Add(new Tuple<string, IEnumerator>(id, IE));
            StartCoroutine(IE);
        }

        List<Tuple<string, IEnumerator>> _startedCoroutines = new List<Tuple<string, IEnumerator>>();
        IEnumerator RemoveMMAfterSeconds(string id, float seconds)
        {
            yield return new WaitForSeconds(seconds);

            _startedCoroutines.RemoveAt(_startedCoroutines.FindIndex(x => x.Item1 == id));
            RemoveMovementMode(id);
        }
    }

    public enum MovementMode { Normal, Locked, Repeating }
}
