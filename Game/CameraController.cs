using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera cam;

    bool moving = false;
    private void Update()
    {
        moving = false;
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity += new Vector3(-1, 0, 0);
            moving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity += new Vector3(1, 0, 0);
            moving = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity += new Vector3(0, 1);
            moving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity += new Vector3(0, -1);
            moving = true;
        }
        if (rb.velocity.magnitude > cam.orthographicSize) 
        {
            rb.velocity = rb.velocity.normalized * cam.orthographicSize;
        }
        if(!moving) 
        {
            rb.velocity = rb.velocity * 0.9f;
            if(rb.velocity.magnitude < 0.5f) { rb.velocity = Vector2.zero; }
        }
        if (!Player.myPlayer.isHoveringUI)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                if (cam.orthographicSize > 5)
                {
                    cam.orthographicSize -= Input.mouseScrollDelta.y;
                }
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                if (cam.orthographicSize < 1000)
                {
                    cam.orthographicSize -= Input.mouseScrollDelta.y;
                }
            }
        }
    }
}
