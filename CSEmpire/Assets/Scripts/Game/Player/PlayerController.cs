using Game.GameModes;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -70f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;

        private Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        private Animator _animator;
        private bool _grounded;
        private bool _hasAnimator;
        private int _xVelHash;
        private int _yVelHash;
        private int _jumpHash;
        private int _groundHash;
        private int _fallingHash;
        private int _zVelHash;
        private int _crouchHash;
        private float _xRotation;

        private const float WalkSpeed = 2f;
        private const float RunSpeed = 6f;
        private Vector2 _currentVelocity;

        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }
        
        private void Start() {
            _hasAnimator = TryGetComponent(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();
            
            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _zVelHash = Animator.StringToHash("Z_Velocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _crouchHash = Animator.StringToHash("Crouch");
        }

        private void FixedUpdate()
        {
            if (!_photonView.IsMine) return;
            
            SampleGround();
            HandleCrouch();

            if (GameModes.Game.Instance.isFrozen) return;
            
            Move();
            HandleJump();
        }
        private void LateUpdate()
        {
            if (!_photonView.IsMine) return;
            
            CamMovements();
        }

        private void Move()
        {
            if(!_hasAnimator) return;

            float targetSpeed = _inputManager.Run ? RunSpeed : WalkSpeed;
            if(_inputManager.Crouch) targetSpeed = 1.5f;
            if(_inputManager.Move ==Vector2.zero) targetSpeed = 0;

            if(_grounded)
            {
                
                _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
                _currentVelocity.y =  Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

                Vector3 velocity = _playerRigidbody.velocity;
                float xVelDifference = _currentVelocity.x - velocity.x;
                float zVelDifference = _currentVelocity.y - velocity.z;

                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0 , zVelDifference)), ForceMode.VelocityChange);
            }
            else
            {
                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance,0,_currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
            }


            _animator.SetFloat(_xVelHash , _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void CamMovements()
        {
            if(!_hasAnimator) return;

            float mouseX = _inputManager.Look.x;
            float mouseY = _inputManager.Look.y;
            Camera.position = CameraRoot.position;
            
            
            _xRotation -= mouseY * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0 , 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, mouseX * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void HandleCrouch() => _animator.SetBool(_crouchHash , _inputManager.Crouch);


        private void HandleJump()
        {
            if(!_hasAnimator) return;
            if(!_inputManager.Jump) return;
            if(!_grounded) return;
            _animator.SetTrigger(_jumpHash);
        }

        public void JumpAddForce()
        {
            _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            _animator.ResetTrigger(_jumpHash);
        }

        private void SampleGround()
        {
            if(!_hasAnimator) return;

            if(Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out RaycastHit _, Dis2Ground + 0.1f, GroundCheck))
            {
                _grounded = true;
                SetAnimationGrounding();
                return;
            }
            _grounded = false;
            _animator.SetFloat(_zVelHash, _playerRigidbody.velocity.y);
            SetAnimationGrounding();
        }

        private void SetAnimationGrounding()
        {
            _animator.SetBool(_fallingHash, !_grounded);
            _animator.SetBool(_groundHash, _grounded);
        }
    }
}
