using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    bool isRespawnRequested = false;

    // Vector2 viewInput; //"Deleted" to separete local camera of network camera

    //Rotation
    //float cameraRotationX = 0; //"Deleted" to separete local camera of network camera

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    HPHandler hpHandler;

    Camera localCamera; //CINEMACHINE ??

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
        hpHandler = GetComponent<HPHandler>();
    }
    

    void Start()
    {
        
    }

    /* //"Deleted" to separete local camera of network camera
    void Update()
    {
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }*/

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (isRespawnRequested)
            {
                Respawn();
                return;
            }

            if (hpHandler.isDead) //don't update clients position when they are dead
                return;
        }
        

        if (GetInput(out NetworkInputData networkInputData))
        {
            //Rotate view //"Deleted" to separete local camera of network camera
            //networkCharacterControllerPrototypeCustom.Rotate(networkInputData.rotationInput); 

            //Rotate the transfor acording to the client aim vector
            transform.forward = networkInputData.aimForwardVector;

            //Cancel out Rotation on X axis, no tilt
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;


            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(moveDirection);

            //JUmp
            if (networkInputData.isJumpPressed)
                networkCharacterControllerPrototypeCustom.Jump();

            //If player fallen of platform
            CheckFallRespawn();
        }
    }

    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            Debug.Log($"{Time.time} Respawn due to fall outside of map at position {transform.position}");

            Respawn();
        }
    }

    /*//"Deleted" to separete local camera of network camera
    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }*/

    public void RequestRespawn()
    {
        isRespawnRequested = true;
    }

    void Respawn()
    {
        networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();

        isRespawnRequested = false;
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
    }
}
