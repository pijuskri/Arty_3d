using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    Rigidbody rigid;
    float range = 5;
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
        foreach (var coll in Physics.OverlapSphere(transform.position,range))
        {
            if (coll.gameObject.GetComponent<EnemyAI>() != null)
            {
                EnemyAI enemy = coll.gameObject.GetComponent<EnemyAI>();
                float damage=0;
                float distance = (transform.position - coll.transform.position).magnitude;
                if (distance < 3) damage = 100;
                else damage = 100 / Mathf.Pow(distance-2, 2);
                enemy.health -= damage;
            }
        }
        Destroy(gameObject);

    }
    void OnCollisionEnter(Collision collision)
    {
        HitGround();
    }
}
