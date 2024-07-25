using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LookAtAim : MonoBehaviour
{
    private Vector3 worldPosition;

    private Vector3 screenPosition;
    public GameObject crosshair;
    public Text nicknameText;    

    void Start()
    {
        Cursor.visible = false;
        nicknameText.text = PhotonNetwork.LocalPlayer.NickName;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        screenPosition = Input.mousePosition;
        screenPosition.z = 3f;
        
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = worldPosition;

        crosshair.transform.position = Input.mousePosition;
    }
}
