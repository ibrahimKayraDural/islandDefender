namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, ISpeedUpgradable
    {
        float Speed { get => _BaseSpeed * SpeedUpgradeValue * _currentSpeedEnhancer * _currentSpeedHinderer; }

        [Header("Values")]
        [SerializeField, Min(0)] float _BaseSpeed = 1;
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
        Camera _camera => _CameraManager.CurrentCamera;

        CameraManager _CameraManager
        {
            get
            {
                if (AUTO_cameraManager == null)
                    AUTO_cameraManager = CameraManager.Instance;

                return AUTO_cameraManager;
            }
        }
        CameraManager AUTO_cameraManager = null;

        CanvasManager _CanvasManager
        {
            get
            {
                if (AUTO_canvasManager == null)
                    AUTO_canvasManager = CanvasManager.Instance;

                return AUTO_canvasManager;
            }
        }
        CanvasManager AUTO_canvasManager = null;

        public UpgradeData CurrentSpeedUpgrade { get; set; } = null;
        public float SpeedUpgradeValue { get; set; } = 1;


        public List<Tuple<string, MovementMode>> _movementModeModifiers = new List<Tuple<string, MovementMode>>();
        bool _acceptMovementModeModifier = true;

        public List<Tuple<string, float>> _speedModifiers = new List<Tuple<string, float>>();
        float _currentSpeedHinderer = 1;
        float _currentSpeedEnhancer = 1;

        Vector3 _currentMovement = Vector3.zero;
        Vector3 _oldMovement = Vector3.zero;

        private void Start()
        {
            _RB = GetComponent<Rigidbody>();
            _RB.useGravity = false;
            _RB.angularDrag = 1000000;
            _RB.drag = 1000000;
            _RB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

            (this as ISpeedUpgradable).RefreshSpeedUpgrade();
        }

        void Update()
        {
            if (Input.GetButtonDown("Inventory")) _CanvasManager?.ToggleInventory();

            if (Time.timeScale <= 0 || _camera == null) return;

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

        /// <summary>
        /// Add a new speed modifier with ID
        /// </summary>
        /// <param name="id">ID for the modifier</param>
        /// <param name="value">Speed value. Most extreme value will be used.</param>
        /// <param name="usedID">The ID that end up getting used. Use this ID to remove modifier later. null if operation was unsuccessfull</param>
        /// <returns>If the operation was successfull</returns>
        public bool AddSpeedModifier(string id, float value, out string usedID, bool @override = false)
        {
            usedID = null;
            if (value == 1) return false;

            value = Mathf.Max(value, 0);

            var i = _speedModifiers.FindIndex(x => x.Item1 == id);
            if (i != -1)
            {
                if (@override) _speedModifiers.RemoveAt(i);
                else id = id + Guid.NewGuid().ToString();
            }

            usedID = id;
            _speedModifiers.Add(new Tuple<string, float>(usedID, value));
            RefreshSpeedModifiers();
            return true;
        }
        public void RemoveSpeedModifier(string id)
        {
            int i = _speedModifiers.FindIndex(x => x.Item1 == id);
            if (i != -1)
            {
                _speedModifiers.RemoveAt(i);
                RefreshSpeedModifiers();
            }
        }
        void RefreshSpeedModifiers()
        {
            if (_speedModifiers.Count <= 0)
            {
                _currentSpeedEnhancer = 1;
                _currentSpeedHinderer = 1;
                return;
            }

            var list = _speedModifiers.OrderBy(x => x.Item2);
            float first = list.First().Item2;
            float last = list.Last().Item2;

            _currentSpeedEnhancer = first > 1 ? first : 1;
            _currentSpeedHinderer = last < 1 ? last : 1;
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
                movement = TakeInput() * Speed * Time.deltaTime;
                movement = rotation * movement;

                _oldMovement = movement;
            }

            return movement;
        }

        void CalculateAnimation(float deltaTime)
        {
            Vector3 normalizedVelocity = _currentMovement / deltaTime / Speed;
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

            switch (currentMovementMode)
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

        #region Movement Mode
        public void AddMovementMode(string id, MovementMode mode, bool @override = false)
        {
            Tuple<string, MovementMode> value = new Tuple<string, MovementMode>(id, mode);

            //check for override
            if (@override)
            {
                int sameIDIndex = _movementModeModifiers.FindIndex(x => x.Item1 == id);
                if (sameIDIndex != -1) _movementModeModifiers.RemoveAt(sameIDIndex);

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
            if (index != -1) _movementModeModifiers.RemoveAt(index);
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
        #endregion
    }

    public enum MovementMode { Normal, Locked, Repeating }
}
