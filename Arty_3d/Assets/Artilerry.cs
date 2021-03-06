﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Artilerry : MonoBehaviour
{
    enum CameraModes { gun, follow, overview }

    #region publicVariables
    public Transform barrel;
    public Transform cannon;
    public GameObject shell;
    public Transform breach;
    public GameObject target;
    public GameObject instantiatedTarget;
    public Text textAngle;
    public AudioSource gunAudioSource;
    public AudioClip shotSound;
    public AudioClip shellLand;
    public GameObject enemyPrefab;
    public GameObject explosionPrefab;
    #endregion

    Transform shellJustShot;
    #region gameVariables
    float rotateSpeed = 20;
    float shootCooldownTimer=0;
    float shootCooldown = 0.1f;
    float shootPower = 45f;
    float enemySpawnCooldownTimer = 0;
    float enemySpawnCooldown = 5;
    CameraModes cameraMode=0;
    #endregion
    

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
        //Targeting();
        UI();
        EnemySpawn();
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
        textAngle.text = "Angle: " + System.String.Format("{0:F1}",barrel.transform.rotation.eulerAngles.x) + '\n'+"Azimuth:"+ System.String.Format("{0:F1}", cannon.transform.rotation.eulerAngles.y);
    }
    void Timing()
    {
        shootCooldownTimer -= Time.deltaTime;
        enemySpawnCooldownTimer -= Time.deltaTime;
    }
    void CameraLogic()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraMode++;
            if ((int)cameraMode >= System.Enum.GetNames(typeof(CameraModes)).Length) cameraMode = 0;
        }
        if (cameraMode == CameraModes.gun)
        {
            Camera.main.transform.position = cannon.position;
            Camera.main.transform.position += cannon.forward * 7 + new Vector3(0,2,0) + cannon.right*2;
            Camera.main.transform.rotation = cannon.rotation;
            Camera.main.transform.Rotate(10,180,0);
            //Camera.main.transform.rotation = Quaternion.Euler(cannon.rotation.x+10, cannon.rotation.y, cannon.rotation.z);
        }
        else if (cameraMode == CameraModes.follow)
        {
            if (shellJustShot != null)
            {
                Camera.main.transform.position = new Vector3(shellJustShot.position.x, shellJustShot.position.y + 2, shellJustShot.position.z - 2);
                Camera.main.transform.rotation = Quaternion.Euler(30, 0, 0);
            }
            else cameraMode = CameraModes.gun;
        }
        else if (cameraMode == CameraModes.overview)
        {
            Camera.main.transform.position = new Vector3(40, 20, 20);
            Camera.main.transform.rotation = Quaternion.Euler(45, -90, 0);
        }
    }
    #region shooting
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && shootCooldownTimer < 0)
        {
            shootCooldownTimer = shootCooldown;
            GameObject temp = Instantiate(shell,breach.position, barrel.rotation);
            //temp.GetComponent<Rigidbody>().AddForce(barrel.up*1000);
            temp.GetComponent<Rigidbody>().velocity = -barrel.forward*shootPower;
            temp.GetComponent<Shell>().shellLand = shellLand;
            temp.GetComponent<Shell>().explosionPrefab = explosionPrefab;
            shellJustShot = temp.transform;
            gunAudioSource.PlayOneShot(shotSound);
        }
    }
    void Targeting()
    {
        if (instantiatedTarget == null) instantiatedTarget = Instantiate(target, new Vector3(), new Quaternion());
        Vector3 dist = FinalShellDistance(-barrel.forward * shootPower);
        instantiatedTarget.transform.position = cannon.position + dist;
        float height = barrel.position.y;
        if (Physics.Raycast(instantiatedTarget.transform.position, Vector3.down, out RaycastHit hit))
        {
            height = hit.point.y+0.2f;
        }
        instantiatedTarget.transform.position = new Vector3(instantiatedTarget.transform.position.x, height, instantiatedTarget.transform.position.z);
        //Debug.Log(dist);
    }
    Vector3 FinalShellDistance(Vector3 vel)
    {
        float height = 0;
        if (Physics.Raycast(instantiatedTarget.transform.position, Vector3.down, out RaycastHit hit)) height = hit.point.y;
        float landHeight=breach.position.y*1.8f - height;
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

        //if (Physics.Raycast(instantiatedTarget.transform.position, Vector3.down, out hit)) { height = hit.point.y; if()}
        //Debug.Log(x);
        return new Vector3(x,0,z);
    }
    #endregion
    void EnemySpawn()
    {
        if (enemySpawnCooldownTimer <= 0)
        {
            Vector3 pos = new Vector3(Random.Range(0, 40), 0, Random.Range(0, 40));
            if(Vector3.Distance(cannon.position, pos)<50) pos=pos/(Vector3.Distance(cannon.position,pos)/50)*2;
            pos = new Vector3(pos.x, Terrain.activeTerrain.SampleHeight(pos), pos.z);
            GameObject temp = Instantiate(enemyPrefab,pos, new Quaternion());
            temp.GetComponent<EnemyAI>().player = cannon;
            enemySpawnCooldownTimer = enemySpawnCooldown;
            Debug.Log(Vector3.Distance(cannon.position, pos));
        }
    }
}
