using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    //public float rotationInput;  //"Deleted" to separete local camera of network camera
    public Vector3 aimForwardVector;
    public NetworkBool isJumpPressed;
    public NetworkBool isFireButtonPressed;
}
