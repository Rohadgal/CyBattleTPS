using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Photon.Pun;

public class WeaponChange : MonoBehaviour
{
    public TwoBoneIKConstraint leftHand;

    public TwoBoneIKConstraint rightHand;
    public TwoBoneIKConstraint leftThumb;

    private CinemachineVirtualCamera cam;
    private GameObject camObject;
    public MultiAimConstraint[] aimObjects;
    private Transform aimTarget;
    
    public RigBuilder rig;
    public Transform[] leftTargets;
    public Transform[] rightTargets;
    public Transform[] thumbTargets;
    public GameObject[] weapons;
    private int _weaponIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        camObject = GameObject.Find("PlayerCam");
        //aimTarget = GameObject.Find("AimRef").transform;
        if (this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            cam = camObject.GetComponent<CinemachineVirtualCamera>();
            cam.Follow = this.gameObject.transform;
            cam.LookAt = this.gameObject.transform;
            //Invoke("SetLookAt", 0.1f);
        }
        else
        {
            this.gameObject.GetComponent<PlayerMovement>().enabled = false;
        }
        // leftHand.data.target = leftTargets[_weaponIndex];
        // rightHand.data.target = rightTargets[_weaponIndex];
        // rig.Build();
        
        
    }

    // void SetLookAt()
    // {
    //     if (aimTarget != null)
    //     {
    //         for (int i = 0; i < aimObjects.Length; i++)
    //         {
    //             var target = aimObjects[i].data.sourceObjects;
    //             target.SetTransform(0, aimTarget.transform);
    //             aimObjects[i].data.sourceObjects = target;
    //         }
    //
    //         rig.Build();
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeWeapon();
        }
    }

    private void ChangeWeapon()
    {
        _weaponIndex++;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        if (_weaponIndex >= weapons.Length)
        {
            _weaponIndex = 0;
        }
        
        weapons[_weaponIndex].SetActive(true);
        
        leftHand.data.target = leftTargets[_weaponIndex];
        rightHand.data.target = rightTargets[_weaponIndex];
        leftThumb.data.target = thumbTargets[_weaponIndex];
        rig.Build();
        
    }
}
