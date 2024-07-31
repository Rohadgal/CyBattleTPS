using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NicknamesScript : MonoBehaviour{
	public Text[] names;
	public Image[] healthbars;

	private void Start(){
		for (int  i = 0;  i < names.Length;  i++) {
			names[i].gameObject.SetActive(false);
			healthbars[i].gameObject.SetActive(false); 
		}
	}

	public void Leaving(){
		StartCoroutine("BackToLobby");
	}

	private IEnumerator BackToLobby(){
		yield return new WaitForSeconds(0.5f);
		PhotonNetwork.LoadLevel("Lobby");
	}
}
