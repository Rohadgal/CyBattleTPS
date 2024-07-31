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
