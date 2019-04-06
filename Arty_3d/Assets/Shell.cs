using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    Rigidbody rigid;
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Drag();
        transform.rotation = Quaternion.LookRotation(rigid.velocity);
        transform.Rotate(90,0,0);
    }
    void Drag()
    {
        float gravity = 12f;
        float drag = 0.01f;
        //rigid.velocity -= (rigid.velocity * drag);
        rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y-(gravity*Time.deltaTime), rigid.velocity.z);
    }
    void HitGround()
    {
        Destroy(gameObject);

    }
    void OnCollisionEnter(Collision collision)
    {
        HitGround();
    }
}
