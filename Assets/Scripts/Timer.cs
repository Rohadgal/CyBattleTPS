using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour{
    public Text minutesText;
    public Text secondsText;
    public int minutes = 4;
    public int seconds = 20;

    public void BeginTimer(){
        GetComponent<PhotonView>().RPC("Count", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void Count(){
        BeginCounting();
    }

    void BeginCounting(){
        CancelInvoke();
        InvokeRepeating("TimeCountDown", 1, 1);
    }

    void TimeCountDown(){
        if (seconds > 10) {
            seconds--;
            secondsText.text = seconds.ToString();
            
        } else if (seconds > 0 && seconds < 11) {
            seconds--;
            secondsText.text = "0" + seconds.ToString();
        } else if (seconds == 0 && minutes > 0) {
            secondsText.text = "0" + seconds.ToString();
            minutes--;
            seconds = 59;
            minutesText.text = minutes.ToString();
            secondsText.text = seconds.ToString();
        }
    }
}
