using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NicknamesScript : MonoBehaviourPunCallbacks{
	public Text[] names;
	public Image[] healthbars;
	private GameObject waitCanvasObject;

	private void Start(){
		for (int  i = 0;  i < names.Length;  i++) {
			names[i].gameObject.SetActive(false);
			healthbars[i].gameObject.SetActive(false); 
		}

		waitCanvasObject = GameObject.Find("WaitingBackground");
	}

	public void Leaving(){
		StartCoroutine("BackToLobby");
	}

	private IEnumerator BackToLobby(){
		yield return new WaitForSeconds(0.5f);
		PhotonNetwork.LoadLevel("Lobby");
	}
	
	//This is for the waiting screen
	public void ReturnToLobby(){
		waitCanvasObject.SetActive(false);
		RoomExit();
	}

	private void RoomExit(){
		StartCoroutine(ToLobby());
	}

	IEnumerator ToLobby(){
		yield return new WaitForSeconds(0.4f);
		Cursor.visible = true;
		PhotonNetwork.LeaveRoom();
	}

	public override void OnLeftRoom(){
		PhotonNetwork.LoadLevel("Lobby");
	}
}
