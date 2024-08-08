using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.UI;
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
    private GameObject testForWeapons;
    private Image weaponIcon;
    private Text ammoAmountText;
    public Sprite[] weaponIcons;
    public int[] ammoAmounts;
    public GameObject[] muzzleFlash;
    //private string shooterName, gotShotName;
    public float[] damageAmount;

    public bool isDead =
        false;
    
    void Start(){
        weaponIcon = GameObject.Find("WeaponUI").GetComponent<Image>();
        ammoAmountText = GameObject.Find("AmmoAmount").GetComponent<Text>();
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
        
        testForWeapons = GameObject.Find("WeaponPickUp1(Clone)");
        if (!testForWeapons)
        {
            if (this.gameObject.GetComponent<PhotonView>().Owner.IsMasterClient) {
                var spawner = GameObject.Find("SpawnManager");
                spawner.GetComponent<SpawnCharacters>().SpawnWeaponStart();
            }
        }
        
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


    void Update()
    {
        if (isDead) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            GunshotAction();
        }
        if (Input.GetKeyDown(KeyCode.Q) && this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            ChangeWeapon();
        }
        
    }

    private void GunshotAction(){
        if (this.GetComponent<PhotonView>().IsMine) {
            GetComponent<DisplayColor>().PlayGunShot(GetComponent<PhotonView>().Owner.NickName, _weaponIndex);
            this.GetComponent<PhotonView>().RPC("GunMuzzleFlash", RpcTarget.All);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (Physics.Raycast(ray, out hit, 500) && 
                hit.transform.gameObject.GetComponent<PhotonView>() &&
                hit.transform.gameObject.GetComponent<DisplayColor>()) {
                string gotShotName = hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName;
                string shooterName = GetComponent<PhotonView>().Owner.NickName;
                hit.transform.gameObject.GetComponent<DisplayColor>().DeliverDamage(shooterName, gotShotName, damageAmount[_weaponIndex]);
               // Debug.Log(gotShotName + " got hit by " + shooterName);
            }
            this.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    [PunRPC]
    void GunMuzzleFlash(){
        muzzleFlash[_weaponIndex].SetActive(true);
        StartCoroutine(MuzzleOff());
    }
    IEnumerator MuzzleOff(){
        yield return new WaitForSeconds(0.03f);
        this.GetComponent<PhotonView>().RPC("MuzzleFlashOff", RpcTarget.All);
        
    }

    [PunRPC]
    void MuzzleFlashOff(){
        muzzleFlash[_weaponIndex].SetActive(false);
    }

    private void ChangeWeapon()
    {
        this.GetComponent<PhotonView>().RPC("Change", RpcTarget.AllBuffered);
        //_weaponIndex++;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        if (_weaponIndex >= weapons.Length) {
            weaponIcon.GetComponent<Image>().sprite = weaponIcons[0];
            ammoAmountText.text = ammoAmounts[0].ToString();
            _weaponIndex = 0;
        }
        
        weapons[_weaponIndex].SetActive(true);
        weaponIcon.GetComponent<Image>().sprite = weaponIcons[_weaponIndex];
        ammoAmountText.text = ammoAmounts[_weaponIndex].ToString();
        leftHand.data.target = leftTargets[_weaponIndex];
        rightHand.data.target = rightTargets[_weaponIndex];
        leftThumb.data.target = thumbTargets[_weaponIndex];
        rig.Build();
        
    }

    [PunRPC]
    public void Change()
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
