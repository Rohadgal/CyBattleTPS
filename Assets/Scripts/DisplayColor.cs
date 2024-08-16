using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DisplayColor : MonoBehaviourPunCallbacks{
	public int[] buttonNumbers;
	public int[] viewID;
	public Color32[] colors;
	public Color32[] teamColors;
	private bool teamMode = false;
	private GameObject namesObject, waitForPlayers;
	public AudioClip[] gunshotSounds;
	private bool isRespawn = false;
	
	private void Start(){
		namesObject = GameObject.Find("NamesBackground");
		waitForPlayers = GameObject.Find("WaitingBackground");
		InvokeRepeating("CheckTime", 1, 1);
		teamMode = namesObject.GetComponent<NicknamesScript>().teamMode;
		isRespawn = namesObject.GetComponent<NicknamesScript>().noRespawn;
		GetComponent<PlayerMovement>().noRespawn = isRespawn;
	}

	private void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (GetComponent<PhotonView>().IsMine && !waitForPlayers.activeInHierarchy) {
				RemoveData();
				RoomExit();
			}
		}

		if (this.GetComponent<Animator>().GetBool("isHit")) {
			StartCoroutine(Recover());
		}
	}

	public void NoRespawnExit(){
		namesObject.GetComponent<NicknamesScript>().eliminationPanel.SetActive(true);
		StartCoroutine(WaitToExit());
	}

	void CheckTime(){
		if (namesObject.GetComponent<Timer>().timeStop) {
			this.gameObject.GetComponent<PlayerMovement>().isDead = true;
			this.gameObject.GetComponent<PlayerMovement>().gameOver = true;
			this.gameObject.GetComponent<WeaponChange>().isDead = true;
			this.gameObject.GetComponentInChildren<AimLookAtRef>().isDead = true;
			this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
	}

	public void Respawn(string name){
		GetComponent<PhotonView>().RPC("ResetForReplay", RpcTarget.AllBuffered, name);
	}

	[PunRPC]
	void ResetForReplay(string name){
		for (int i = 0; i < namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (name == namesObject.GetComponent<NicknamesScript>().names[i].text) {
				this.GetComponent<Animator>().SetBool("isDead", false);
				this.gameObject.GetComponent<WeaponChange>().isDead = false;
				this.gameObject.GetComponentInChildren<AimLookAtRef>().isDead = false;
				this.gameObject.layer = LayerMask.NameToLayer("Default");
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>().fillAmount =
					1;
			}
		}
	}

	public void DeliverDamage(string shooterName, string name, float damageAmount){
		GetComponent<PhotonView>().RPC("GunDamage", RpcTarget.AllBuffered, shooterName, name, damageAmount);	
	}

	[PunRPC]
	void GunDamage(string shooterName, string name, float damageAmount){
		for (int i = 0; i < namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (name == namesObject.GetComponent<NicknamesScript>().names[i].text) {
				if (namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>()
					    .fillAmount > 0.1f) {
					this.GetComponent<Animator>().SetBool("isHit", true);
					namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>().fillAmount -= damageAmount;
					return;
				}
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>().fillAmount =
					0;
				this.GetComponent<Animator>().SetBool("isDead", true);
				this.gameObject.GetComponent<PlayerMovement>().isDead = true;
				this.gameObject.GetComponent<WeaponChange>().isDead = true;
				this.gameObject.GetComponentInChildren<AimLookAtRef>().isDead = true;
				namesObject.GetComponent<NicknamesScript>().RunMessage(shooterName, name);
				this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
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
				this.transform.GetChild(1).GetComponent<Renderer>().material.color = (!teamMode) ? colors[i] : teamColors[i];
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

	IEnumerator Recover(){
		yield return new WaitForSeconds(0.03f);
		this.GetComponent<Animator>().SetBool("isHit", false);
	}

	IEnumerator WaitToExit(){
		yield return new WaitForSeconds(3f);
		RemoveMe();
		RoomExit();
	}
}
