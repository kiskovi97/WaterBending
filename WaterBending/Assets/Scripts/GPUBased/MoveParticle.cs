using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveParticle : MonoBehaviour
{
    public Vector3 goal;
    public Vector3 center;
    public float distance = 10f;
    public float speed = 2f;
    private float angel = 0;
    Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        VRUpdate();
       

        var current = transform.position;

        Debug.DrawLine(current, goal, Color.red);

        var dir = goal - current;

        if (dir.magnitude > 1f)
        {
            dir = dir.normalized * speed;
        }
        Move(dir);

    }

    private void NonVRUpdate()
    {
        if (Input.mousePresent && Input.GetMouseButton(0))
        {
            var v3 = Input.mousePosition;
            v3.z = 196.5f;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            center = v3;
            goal = center;
        }
        else
        {
            angel += speed * 4f * Time.deltaTime;
            goal = GetByAngle();
        }
    }

    private void VRUpdate()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            var position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            goal = position;
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            var position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            goal = position;
        }
    }

    private void Move(Vector3 dir)
    {
        rigidbody.MovePosition(transform.position + Time.deltaTime * dir);
       //transform.position += Time.deltaTime * dir;
    }

    private Vector3 GetByAngle()
    {
        return center;// + Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.up * distance;
    }
}
