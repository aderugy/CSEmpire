using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TESTMap : MonoBehaviour
{
    public Camera PlayerCamera;
    public float walkspeed = 6f;
    public float rundspeed = 12f;
    public float jumpower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXlimit = 45f;
    
    Vector3 movedirection = Vector3.zero;
    private float rtX = 0;

    public bool canMove = true;

    private CharacterController _characterController;

    public void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        #region Movenment

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isrunnig = Input.GetKey(KeyCode.LeftShift);
        float curX = canMove ? (isrunnig ? rundspeed : walkspeed) * Input.GetAxis("Vertical") : 0;
        float curY = canMove ? (isrunnig ? rundspeed : walkspeed) * Input.GetAxis("Horizontal") : 0;
        float movdirY = movedirection.y;
        movedirection = (forward * curX) + (right * curY);
        #endregion

        #region JUmp

        if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
        {
            movedirection.y = jumpower;
        }
        else
        {
            movedirection.y = movdirY;
        }

        if (!_characterController.isGrounded)
        {
            movedirection.y -= gravity * Time.deltaTime;
        }
        #endregion


        #region Rotation cam

        _characterController.Move(movedirection * Time.deltaTime);

        if (canMove)
        {
            rtX += Input.GetAxis("Mouse Y") * lookSpeed;
            rtX = Math.Clamp(rtX, -lookXlimit, lookXlimit);
            PlayerCamera.transform.localRotation = Quaternion.Euler(rtX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }


        #endregion
    }
}
