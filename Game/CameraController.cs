using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Camera cam;

    public static CameraController main;
    public UnityEvent camChange = new UnityEvent();
    private void Awake()
    {
        main = this;
    }
    bool moving = false;
    private void Update()
    {
        moving = false;
        if (!Player.myPlayer.isHoveringUI && Input.GetMouseButton(2))
        {
            if (Input.mousePosition.x > Screen.width - 100)
            {
                rb.velocity += new Vector3(1f, 0, 0);
            }
            if (Input.mousePosition.x < 100)
            {
                rb.velocity += new Vector3(-1f, 0, 0);
            }
            if (Input.mousePosition.y > Screen.height - 100)
            {
                rb.velocity += new Vector3(0, 1f, 0);
            }
            if (Input.mousePosition.y < 100)
            {
                rb.velocity += new Vector3(0, -1f, 0);
            }
        }
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
                if (cam.orthographicSize < 30)
                {
                    cam.orthographicSize -= Input.mouseScrollDelta.y;
                }
            }
            if (Input.mouseScrollDelta.y != 0)
            {
                camChange.Invoke();
                foreach (var civ in Game.main.civs)
                {
                    if (civ.countryNames != null)
                    {
                        Color c = civ.c;
                        c.a = Mathf.Clamp(cam.orthographicSize / 20f - 0.5f, 0f, 1f);
                        civ.countryNames.ForEach(i=>i.color = c);
                    }
                     
                }
            }
        }
    }
}
