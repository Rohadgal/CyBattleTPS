using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DisplayColor : MonoBehaviourPunCallbacks{
	public int[] buttonNumbers;
	public int[] viewID;
	public Color32[] colors;

	private GameObject namesObject, waitForPlayers;

	public AudioClip[] gunshotSounds;
	private void Start(){
		namesObject = GameObject.Find("NamesBackground");
		waitForPlayers = GameObject.Find("WaitingBackground");
	}

	private void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (GetComponent<PhotonView>().IsMine && !waitForPlayers.activeInHierarchy) {
				RemoveData();
				RoomExit();
			}
		}
	}

	public void DeliverDamage(string name, float damageAmount){
		GetComponent<PhotonView>().RPC("GunDamage", RpcTarget.AllBuffered, name, damageAmount);	
	}

	[PunRPC]
	void GunDamage(string name, float damageAmount){
		for (int i = 0; i < namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (name == namesObject.GetComponent<NicknamesScript>().names[i].text) {
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>().fillAmount -= damageAmount;
			}
		}
	}
	
	private void RoomExit(){
		StartCoroutine(GetReadyToLeave());
	}

	private IEnumerator GetReadyToLeave(){
		yield return new WaitForSeconds(1);
		namesObject.GetComponent<NicknamesScript>().Leaving();
		Cursor.visible = true;
		PhotonNetwork.LeaveRoom();
	}

	private void RemoveData(){
		GetComponent<PhotonView>().RPC("RemoveMe", RpcTarget.AllBuffered);
	}

	public void ChooseColor(){
		GetComponent<PhotonView>().RPC("AssignColor", RpcTarget.AllBuffered);
	}

	public void PlayGunShot(string name, int weaponNumber) {
		GetComponent<PhotonView>().RPC("PlaySound", RpcTarget.All, name, weaponNumber);
	}

	[PunRPC]
	void PlaySound(string name, int weaponNumber){
		for (int i = 0; i < namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (name == namesObject.GetComponent<NicknamesScript>().names[i].text) {
				GetComponent<AudioSource>().clip = gunshotSounds[weaponNumber];
				GetComponent<AudioSource>().Play();
			}
		}
	}

	[PunRPC]
	void AssignColor(){
		for (int i = 0; i < viewID.Length; i++) {
			if (this.GetComponent<PhotonView>().ViewID == viewID[i]) {
				this.transform.GetChild(1).GetComponent<Renderer>().material.color = colors[i];
				namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(true);
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.SetActive(true);
				namesObject.GetComponent<NicknamesScript>().names[i].text = this.GetComponent<PhotonView>().Owner.NickName;
			}	
		}
	}

	[PunRPC]
	void RemoveMe(){
		for (int i = 0; i < namesObject.gameObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (this.GetComponent<PhotonView>().Owner.NickName ==
			    namesObject.GetComponent<NicknamesScript>().names[i].text) {
				namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(false);
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.SetActive(false);
			}
		}
	}
}
