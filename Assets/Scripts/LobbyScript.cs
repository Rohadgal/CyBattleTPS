using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LobbyScript : MonoBehaviourPunCallbacks
{
    private TypedLobby killCount = new TypedLobby("killCount", LobbyType.Default);
    private TypedLobby teamBattle = new TypedLobby("teamBattle", LobbyType.Default);
    private TypedLobby noRespawn = new TypedLobby("noRespawn", LobbyType.Default);

    public Text roomNumber;
    private string levelName = "";

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void JoinGameKillCount(){
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(killCount);
    }
    
    public void JoinTeamBattle()
    {
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(teamBattle);
    }
    
    public void JoinNoRespawn()
    {
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(noRespawn);
    }

    public override void OnJoinedLobby(){
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message){
        Debug.Log("Joined random room failed, creating a new room");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6;
        PhotonNetwork.CreateRoom("Arena" + Random.Range(1, 1000), roomOptions);
    }

    public override void OnJoinedRoom(){
        roomNumber.text = PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.LoadLevel(levelName);
    }
}
