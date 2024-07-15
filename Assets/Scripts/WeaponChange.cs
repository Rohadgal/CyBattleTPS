using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponChange : MonoBehaviour
{
    public TwoBoneIKConstraint leftHand;

    public TwoBoneIKConstraint rightHand;

    public RigBuilder rig;
    
    public Transform[] leftTargets;
    public Transform[] rightTargets;
    public GameObject[] weapons;
    private int _weaponIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        leftHand.data.target = leftTargets[_weaponIndex];
        rightHand.data.target = rightTargets[_weaponIndex];
        rig.Build();
    }

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
        rig.Build();
        
    }
}
