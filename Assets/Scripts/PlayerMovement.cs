using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float rotateSpeed = 100.0f;
    public float jumpForce = 1200.0f;
    private Rigidbody _rb;
    private Animator _anim;
    private bool _canJump = true;
    public bool isDead;
    private Vector3 startPos;
    private bool respawned;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead) {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            
            Vector3 rotateY = new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);
            if (movement != Vector3.zero)
            {
                _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotateY));
            }
            _rb.MovePosition(_rb.position + (((transform.forward * movement.z) + (transform.right * movement.x)) * (moveSpeed * Time.deltaTime)));
            
            _anim.SetFloat("BlendVertical", movement.z);
            _anim.SetFloat("BlendHorizontal", movement.x);
        }
    }

    private void Update()
    {
        if (!isDead) {
            if(Input.GetButtonDown("Jump") && _canJump)
            {
                _canJump = false;
                _rb.AddForce(Vector3.up * (jumpForce * Time.deltaTime), ForceMode.VelocityChange);
                StartCoroutine(JumpAgain());
            }

            return;
        }

        if (!respawned) {
            respawned = true;
            StartCoroutine(RespawnWait());
        }
    }

    IEnumerator JumpAgain()
    {
        yield return new WaitForSeconds(1.2f);
        _canJump = true;
    }

    IEnumerator RespawnWait(){
        yield return new WaitForSeconds(3f);
        isDead = false;
        respawned = false;
        transform.position = startPos;
        GetComponent<DisplayColor>().Respawn(GetComponent<PhotonView>().Owner.NickName);
    }
}
