using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DisplayColor : MonoBehaviour{
	public int[] buttonNumbers;
	public int[] viewID;
	public Color32[] colors;

	private GameObject namesObject;

	private void Start(){
		namesObject = GameObject.Find("NamesBackground");
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
}
