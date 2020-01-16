using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Small camera movement class to track the player
public class CameraMovement : MonoBehaviour
{

    public Transform player;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            player.position.x + offset.x, 
            player.position.y + offset.y, 
            offset.z); // Camera follows the player with specified offset position
    }
}
