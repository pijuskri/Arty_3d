using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Artilerry : MonoBehaviour
{
    #region publicVariables
    public Transform barrel;
    public Transform cannon;
    public GameObject shell;
    public Transform breach;
    public GameObject target;
    public GameObject instantiatedTarget;
    public Text textAngle;
    #endregion

    Transform shellJustShot;
    #region gameVariables
    float rotateSpeed = 20;
    float shootCooldownTimer=0;
    float shootCooldown = 0.1f;
    float shootPower = 28f;
    CameraModes cameraMode=0;
    #endregion
    enum CameraModes{gun,follow,overview}

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timing();
        CameraLogic();
        Rotation();
        Shoot();
        Targeting();
        UI();
    }
    void Rotation()
    {
        barrel.Rotate(new Vector3(-Input.GetAxisRaw("Vertical"), 0, 0) * rotateSpeed * Time.deltaTime, Space.Self);
        barrel.localRotation = Quaternion.Euler(Mathf.Clamp(barrel.localRotation.eulerAngles.x, 3, 60), barrel.localRotation.eulerAngles.y, barrel.localRotation.eulerAngles.z);
        cannon.Rotate(new Vector3(0, Input.GetAxisRaw("Horizontal"), 0) * rotateSpeed * Time.deltaTime, Space.Self);
        //barrel.rotation = Quaternion.Euler(barrel.rotation.eulerAngles.x, 0, barrel.rotation.eulerAngles.z);
    }
    void UI()
    {
        textAngle.text = "Angle: " + barrel.transform.rotation.z;
    }
    void Timing()
    {
        shootCooldownTimer -= Time.deltaTime;
    }
    void CameraLogic()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraMode++;
            if ((int)cameraMode >= Enum.GetNames(typeof(CameraModes)).Length) cameraMode = 0;
        }
        if (cameraMode == CameraModes.gun)
        {
            Camera.main.transform.position = new Vector3(cannon.position.x, cannon.position.y + 2, cannon.position.z - 3);
            Camera.main.transform.rotation = Quaternion.Euler(30, 0, 0);
        }
        else if (cameraMode == CameraModes.follow && shellJustShot != null)
        {
            Camera.main.transform.position = new Vector3(shellJustShot.position.x, shellJustShot.position.y + 2, shellJustShot.position.z - 2);
            Camera.main.transform.rotation = Quaternion.Euler(30, 0, 0);
        }
        else if (cameraMode == CameraModes.overview)
        {
            Camera.main.transform.position = new Vector3(20, 30, 20);
            Camera.main.transform.rotation = Quaternion.Euler(65, -90, 0);
        }
    }
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && shootCooldownTimer < 0)
        {
            shootCooldownTimer = shootCooldown;
            GameObject temp = Instantiate(shell,breach.position, barrel.rotation);
            //temp.GetComponent<Rigidbody>().AddForce(barrel.up*1000);
            temp.GetComponent<Rigidbody>().velocity = -barrel.forward*shootPower;
            shellJustShot = temp.transform;
        }
    }
    void Targeting()
    {
        if (instantiatedTarget == null) instantiatedTarget = Instantiate(target, new Vector3(), new Quaternion());
        Vector3 dist = FinalShellDistance(-barrel.forward * shootPower);
        instantiatedTarget.transform.position = cannon.position + dist;
        instantiatedTarget.transform.position = new Vector3(instantiatedTarget.transform.position.x, 0, instantiatedTarget.transform.position.z);
        //Debug.Log(dist);
    }
    Vector3 FinalShellDistance(Vector3 vel)
    {
        float landHeight=breach.position.y*1.8f;
        float gravity = 12f;
        float x=0;
        float t=0;
        float t1=0, t2=0;
        t1 = (-vel.y + Mathf.Sqrt(Mathf.Pow(vel.y,2) + 4* (gravity / 2) * landHeight))/(2*-(gravity/2));
        t2 = (-vel.y - Mathf.Sqrt(Mathf.Pow(vel.y, 2) + 4 * (gravity / 2) * landHeight)) / (2 * -(gravity / 2));
        if (t1 > t2) t = t1;
        else t = t2;
        x = t * (vel.x);
        float z = t * (vel.z);
        //Debug.Log(x);
        return new Vector3(x,0,z);
    }
}
