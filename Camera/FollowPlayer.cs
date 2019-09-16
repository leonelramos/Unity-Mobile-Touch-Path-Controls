using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    public Vector3 cameraOffset = new Vector3(0.0f,30.0f,20.0f);
    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + cameraOffset;
    }
}
