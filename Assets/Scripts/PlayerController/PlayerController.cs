using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private CharacterController controller;
    GameManager gameManager;
    public Animator anim;

    private float Gravity = 20.0f;

    public Vector3 _moveDirection = Vector3.zero;

    public float Speed = 5.0f;

    public float RotationSpeed = 240.0f;

    public float RotationGamepadSensibilityPercent = 1f;

    public float JumpSpeed = 7.0f;

    public bool doubleJumpBool, invincibilityBool, moveFastBool;

    private bool prevGrounded = false;

    private float doubleJumpReady = 0;

    private Transform cameraTranform;

    public AudioSource jumpSound;

    public MenuManager menuManager;

    // knockBack
    float knockBackCounter, knockBackTime, knockBackForce;

    void Start () {
        anim = GetComponent<Animator> ();
        gameManager = FindObjectOfType<GameManager> ();
        menuManager = FindObjectOfType<MenuManager> ();
        controller = GetComponent<CharacterController> ();
        cameraTranform = Camera.main.GetComponent<CameraPlayer> ().getForwardDirectionTransform ();
        setInvincibilityBool (false);
        jumpSound = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
       void FixedUpdate() {
        basicAnimMovement ();
        Move ();
        checkLevel ();
    }

    // checkPlayerPositionY: Controlla l'altezza del player rispetto all'acqua o la lava
    private void checkLevel () {
        if (transform.position.y < gameManager.fluidLevel) {
            gameManager.respawPlayer ();
            gameManager.triggerDead ();
            if (menuManager.getScene () == "Level1" || menuManager.getScene () == "Level2") {
                gameManager.Water.Play ();
            } else {
                if (menuManager.getScene () == "Level3") {
                    gameManager.lavaEffect.Play ();
                }
            }
        }
    }

    bool isSliding = false;
    bool isKnocked = false;
    float knockTimer = 0;

    public Vector3 forward_CAM;

    public void Move () {

        // Get Input for axis
        float h = Input.GetAxis ("Horizontal");
        float v = Input.GetAxis ("Vertical");

        // Calculate the forward vector
        Vector3 camForward_Dir = Vector3.Scale (cameraTranform.forward, new Vector3 (1, 0, 1)).normalized;
        Vector3 move = v * camForward_Dir + h * cameraTranform.right;
        Vector3 __movement_raw = new Vector3 (h, 0f, v);
        if (move.magnitude > 1f) move.Normalize ();

        // Calculate the rotation for the player
        Vector3 rotation = transform.InverseTransformDirection (move);

        // Get Euler angles
        float turnAmount = Mathf.Atan2 (rotation.x, rotation.z);
        float relativeTurnSpeed = RotationSpeed * ((1f - RotationGamepadSensibilityPercent) + (new Vector3 (__movement_raw.x, 0f, __movement_raw.z).magnitude / Vector3.forward.magnitude) * RotationGamepadSensibilityPercent);

        transform.Rotate (0, turnAmount * relativeTurnSpeed * Time.deltaTime, 0);

        if (!isSliding && !isKnocked) {
            _moveDirection.x = (move.x) * (Speed + (moveFastBool?Speed * 0.5f : 0f));
            _moveDirection.z = (move.z) * (Speed + (moveFastBool?Speed * 0.5f : 0f));
            _moveDirection.y -= (Gravity * Time.deltaTime);
        }
        //Is sliding (can be knocked)
        else if (isSliding) {
            if (!controller.isGrounded) isSliding = false;
            _moveDirection.x += Math.Max (0f, 1f - Math.Abs (hitNormal.normalized.x)) * (move.x) * Speed;
            _moveDirection.z += Math.Max (0f, 1f - Math.Abs (hitNormal.normalized.z)) * (move.z) * Speed;
            _moveDirection = _moveDirection * 100 * Time.deltaTime;
        }
        //Is Knocked
        else {
            if (Time.time - knockTimer > 0.1f) {
                knockTimer = Time.time;
                if ((_moveDirection.x > 0 && move.x < 0) || (_moveDirection.x < 0 && move.x > 0))
                    _moveDirection.x += Math.Max (0f, 1f - Math.Abs (_moveDirection.normalized.x)) * (move.x) * (Speed);
                if ((_moveDirection.z > 0 && move.z < 0) || (_moveDirection.z < 0 && move.z > 0))
                    _moveDirection.z += Math.Max (0f, 1f - Math.Abs (_moveDirection.normalized.z)) * (move.z) * (Speed);
            }
            _moveDirection.y -= Gravity * Time.deltaTime;
        }

        if (doubleJumpBool) Jump (true);
        if (controller.isGrounded && prevGrounded) {
            prevGrounded = false;
            //_moveDirection.y = 0;
        }

        controller.Move (_moveDirection * Time.deltaTime);
    }

    Vector3 hitNormal;

    void OnControllerColliderHit (ControllerColliderHit hit) {
        GameObject groundObject = null;
        if (controller.isGrounded) prevGrounded = true;
        if (hit.normal.y > 0.8f) {
            isKnocked = false;
            groundObject = hit.collider.gameObject;
            if (prevGrounded) _moveDirection.y = 0;
            isSliding = false;
        } else if (hit.normal.y > 0.1f && _moveDirection.y <= 0 && hit.point.y<transform.position.y) {
            hitNormal = hit.normal;
            //hitNormal.y = Math.Max(0.1f, hitNormal.y);
            if (!isSliding) {
                _moveDirection.x = 0;
                _moveDirection.z = 0;
            }
            _moveDirection.y = Math.Max (-Gravity * 2 * Time.deltaTime, -Gravity * (1f - hitNormal.y));
            //_moveDirection.x += (1f - hitNormal.y) * hitNormal.x * (1f - 0f);
            //_moveDirection.z += (1f - hitNormal.y) * hitNormal.z * (1f - 0f);
            _moveDirection.x = ((1f - hitNormal.y) * hitNormal.x * (1f - 0f));
            _moveDirection.z = ((1f - hitNormal.y) * hitNormal.z * (1f - 0f));
            _moveDirection *= 400 * Time.deltaTime;
            isSliding = true;
        }
        if (groundObject != null) {
            Jump (false);
            if (groundObject.tag == "RespawnPlatform") {
                Vector3 spawnPoint = new Vector3 (groundObject.transform.position.x, groundObject.GetComponent<Renderer> ().bounds.max.y + transform.lossyScale.y, groundObject.transform.position.z);
                gameManager.setSpawn (spawnPoint);
            }
            if (groundObject.tag == "StartPlatform") {
                gameManager.resetSpawn ();
            }
            if(groundObject.tag == "waterCollider")
            {
                gameManager.triggerDead();
                gameManager.Water.Play();
            }
            if (groundObject.tag == "deadCollider")
            {
                gameManager.triggerDead();
            }
            if (groundObject.tag == "lavaCollider")
            {
                gameManager.triggerDead();
                GameObject.Find("LavaCollider").GetComponent<AudioSource>().Play();
            }
        }
    }

    public void Jump (bool doublejump) {
        //doubleJumpReady = true;
        if (!doublejump && controller.isGrounded) {

            bool isDoubleJump = doubleJumpReady > 0;
            //_moveDirection.y = 0;
            if (Input.GetButton ("Jump")) {
                jumpSound.Play ();
                if (!isDoubleJump) doubleJumpReady = Time.time;
                prevGrounded = false;
                _moveDirection.y = JumpSpeed;
            } else {
                prevGrounded = true;
            }
        }
        if (prevGrounded && _moveDirection.y < 0) doubleJumpReady = Time.time;
        else if (doublejump && doubleJumpReady > 0 && (Time.time - doubleJumpReady > 0.27)) {
            if (Input.GetButton ("Jump")) {
                jumpSound.Play ();
                doubleJumpReady = 0;
                prevGrounded = false;
                _moveDirection.y = JumpSpeed;
            }
        }
        /*else if (prevGrounded)
        {
            prevGrounded = false;
            _moveDirection.y = 0;
        }*/
    }

    // funzioni power ups

    public void setInvincibilityBool (bool boolean) {
        invincibilityBool = boolean;
    }

    public bool getInvincibilityBool () {
        return invincibilityBool;
    }

    public bool getMoveFastBool () {
        return moveFastBool;
    }

    public bool getdoubleJumpBool () {
        return doubleJumpBool;
    }

    public void setMoveFastBool (bool boolean) {
        moveFastBool = boolean;
    }

    public void setDobleJumpBool (bool boolean) {
        doubleJumpBool = boolean;
    }

    public void basicAnimMovement () {

        anim.SetBool ("IsGroundedAnim", controller.isGrounded);
        anim.SetFloat ("SpeedAnim", (Mathf.Abs (Input.GetAxis ("Vertical"))) + (Mathf.Abs (Input.GetAxis ("Vertical"))));

    }

    /*
    private void OnTriggerEnter (Collider other) {

        if (other.tag == "lava") {
            gameManager.triggerDead ();
        }
        

    }
    */

    public void knockback (Vector3 knockBackDir) {
        _moveDirection.x = (knockBackDir.x) * (Speed * 1.5f);
        _moveDirection.z = (knockBackDir.z) * (Speed * 1.5f);

        isKnocked = true;
        knockTimer = Time.time;
        doubleJumpReady = 0;
        prevGrounded = false;
        _moveDirection.y = JumpSpeed;
    }

}