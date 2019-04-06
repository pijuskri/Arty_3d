using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    Rigidbody rigid;
    public AudioClip shellLand;
    float range = 10;
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
        float gravity = 10f;
        float drag = 0.005f;
        rigid.velocity -= new Vector3( Mathf.Pow(rigid.velocity.x * drag, 2), Mathf.Pow(rigid.velocity.y * drag, 2), Mathf.Pow(rigid.velocity.z * drag, 2));
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
                if (distance < 5) damage = 100;
                else damage = 100 / Mathf.Pow(distance-4, 2);
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
