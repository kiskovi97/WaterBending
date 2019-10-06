using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveParticleAWSD : MonoBehaviour
{
    public float speed = 3f;
    public float rotatingSped = 5f;


    // Update is called once per frame
    void Update()
    {
        var left = Input.GetAxis("Horizontal");
        var front = Input.GetAxis("Vertical");
        var upDown = Input.GetAxis("UpDown");

        transform.Rotate(Vector3.up, left * Time.deltaTime * rotatingSped * 30f);

        var dir = Vector3.up * upDown;
        dir += transform.forward * front;
       

        transform.position += dir * Time.deltaTime * transform.lossyScale.x * speed;
    }
}
