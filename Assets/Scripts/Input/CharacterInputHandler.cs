using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;

    bool isJumpButtonPressed = false;

    bool isFireButtonPressed = false;

    //"Deleted" to separete local camera of network camera
    //CharacterMovementHandler characterMovementHandler;

    LocalCameraHandler localCameraHandler;
    CharacterMovementHandler characterMovementHandler;

    private void Awake()
    {
        
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>(); //"Deleted" to separete local camera of network camera
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (!characterMovementHandler.Object.HasInputAuthority)
            return;


        // View data input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1;

        //"Deleted" to separete local camera of network camera
        //characterMovementHandler.SetViewInputVector(viewInputVector);

        // Move data input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        //JUmp
        if (Input.GetButtonDown("Jump"))
        {
            isJumpButtonPressed = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            isFireButtonPressed = true;
        }

        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //View Data
        //networkInputData.rotationInput = viewInputVector.x; //"Deleted" to separete local camera of network camera

        //Aim Data
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;

        //Move Data
        networkInputData.movementInput = moveInputVector;

        //jump data
        networkInputData.isJumpPressed = isJumpButtonPressed;

        //Fire data
        networkInputData.isFireButtonPressed = isFireButtonPressed;

        //Reset variables
        isJumpButtonPressed = false;
        isFireButtonPressed = false;

        return networkInputData;
    }
}
