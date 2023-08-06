using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]

    public class Player_movements : MonoBehaviour
    {
        public Camera playerCamera;

        public float walkSpeed = 6f;
        public float runSpeed = 12f; 
        public float jumpPower = 7f;  
        public float gravity = 10f;
        public float lookSpeed = 2f;  
        public float lookXLimit = 45f;

        private Vector3 _moveDirection = Vector3.zero;
        private float _rotationX = 0;
        public bool canMove = true;
        private CharacterController _characterController;
    
        // Start is called before the first frame update
        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            #region Handle Movement

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
        
            // Press Left Shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = _moveDirection.y;
            _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            #endregion

            #region Handle Jumping

            if (Input.GetButton("Jump") && canMove && _characterController.isGrounded) 
            {
                _moveDirection.y = jumpPower;
            }
            else
            {
                _moveDirection.y = movementDirectionY;
            }

            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= gravity * Time.deltaTime;
            }

            #endregion

            #region Handle Rotation

            _characterController.Move(_moveDirection * Time.deltaTime);

            if (canMove)
            {
                _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }

            #endregion
        }
    }
}
