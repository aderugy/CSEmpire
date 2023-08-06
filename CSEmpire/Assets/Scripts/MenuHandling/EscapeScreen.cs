using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeScreen : MonoBehaviour
{

    public GameObject UiObject;
    private bool IsDisplayed;

    private void Start()
    {
        IsDisplayed = false;
        UiObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsDisplayed)
            {
                UiObject.SetActive(false);
                IsDisplayed = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                UiObject.SetActive(true);
                IsDisplayed = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }


    }
}