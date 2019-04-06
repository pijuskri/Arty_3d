using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artilerry : MonoBehaviour
{
    #region publicVariables
    public Transform barrel;
    public Transform cannon;
    public GameObject shell;
    public Transform breach;
    public GameObject target;
    #endregion

    Transform shellJustShot;
    #region gameVariables
    float rotateSpeed = 40;
    float shootCooldownTimer=0;
    float shootCooldown = 0.1f;
    bool followCamera=false;
    #endregion

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timing();
        CameraModes();
        Rotation();
        Shoot();
    }
    void Rotation()
    {
        barrel.Rotate(new Vector3(-Input.GetAxisRaw("Vertical"), 0, 0) * rotateSpeed * Time.deltaTime, Space.Self);
        barrel.localRotation = Quaternion.Euler(Mathf.Clamp(barrel.localRotation.eulerAngles.x, 10, 60), barrel.localRotation.eulerAngles.y, barrel.localRotation.eulerAngles.z);
        cannon.Rotate(new Vector3(0, Input.GetAxisRaw("Horizontal"), 0) * rotateSpeed * Time.deltaTime, Space.Self);
        //barrel.rotation = Quaternion.Euler(barrel.rotation.eulerAngles.x, 0, barrel.rotation.eulerAngles.z);
    }
    void Timing()
    {
        shootCooldownTimer -= Time.deltaTime;
    }
    void CameraModes()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!followCamera) followCamera = true;
            else { followCamera = false; Camera.main.transform.position = new Vector3(cannon.position.x, cannon.position.y+2, cannon.position.z - 3); }
        }
        if (followCamera && shellJustShot != null)
        {
            Camera.main.transform.position = new Vector3( shellJustShot.position.x, shellJustShot.position.y+2, shellJustShot.position.z - 2);
        }
    }
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && shootCooldownTimer < 0)
        {
            shootCooldownTimer = shootCooldown;
            GameObject temp = Instantiate(shell,breach.position, barrel.rotation);
            //temp.GetComponent<Rigidbody>().AddForce(barrel.up*1000);
            temp.GetComponent<Rigidbody>().velocity = -barrel.forward*20;
            shellJustShot = temp.transform;
            float dist = FinalShellDistance(temp.GetComponent<Rigidbody>());
            Instantiate(target, cannon.position + cannon.forward*dist, new Quaternion());
        }
    }
    float FinalShellDistance(Rigidbody rigid)
    {
        float landHeight=breach.position.y*2f;
        float gravity = 12f;
        float x=0;
        float t=0;
        float t1=0, t2=0;
        Vector3 vel = rigid.velocity;
        t1 = (-vel.y + Mathf.Sqrt(Mathf.Pow(vel.y,2) + 4* (gravity / 2) * landHeight))/(2*-(gravity/2));
        t2 = (-vel.y - Mathf.Sqrt(Mathf.Pow(vel.y, 2) + 4 * (gravity / 2) * landHeight)) / (2 * -(gravity / 2));
        if (t1 > t2) t = t1;
        else t = t2;
        x = t * (vel.x + vel.z);
        Debug.Log(x);
        return x;
    }
}
