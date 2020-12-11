using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public PlayerController player;
    public Transform PlayerTransform;

    GameManager gameManager;

    private Vector3 _cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    public float PlayerDistance = 2.3095f;

    public bool LookAtPlayer = false;

    public bool RotateAroundPlayer = true;

    public bool RotateMiddleMouseButton = true;

    public float RotationsSpeed = 5.0f;

    public float CameraPitchMin = -2.8f;

    public float CameraPitchMax = 2.9f;

    private GameObject ForwardDirection;

    private Renderer[] mRenderers;

    private Shader[] mShaders;

    private Shader CameraClosingPlayerShader;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        this.PlayerTransform = player.transform;

        CameraClosingPlayerShader = Shader.Find("Custom/CameraClosingPlayerShader");
        mRenderers = PlayerTransform.gameObject.GetComponentsInChildren<Renderer>();
        mShaders = new Shader[mRenderers.Length];
        ForwardDirection = getForwardDirection();
        _cameraOffset = (transform.position - PlayerTransform.position).normalized * PlayerDistance;
        transform.position = PlayerTransform.position + _cameraOffset;
        gameManager = FindObjectOfType<GameManager>();
    }

    bool shaderChange = false;
    public void SetPlayerAlpha(float alpha) {
        if (alpha>=1) alpha = 1;
        if (!shaderChange && alpha>=1) return;
        for(int i = 0; i < mRenderers.Length; i++) {
            if (!shaderChange && alpha<1) {
                if (mShaders[i]==null) mShaders[i] = mRenderers[i].material.shader;
                mRenderers[i].material.shader = CameraClosingPlayerShader;
            }
            else if (shaderChange && alpha>=1) mRenderers[i].material.shader = mShaders[i];
            for(int j = 0; j < mRenderers[i].materials.Length; j++) {
                Color matColor = mRenderers[i].materials[j].color;
                matColor.a = alpha;
                mRenderers[i].materials[j].color = matColor;
            }
        }
        if (!shaderChange && alpha<1) shaderChange = true;
        else if (shaderChange && alpha>=1) shaderChange = false;
    }

    private bool IsRotateActive
    {
        get
        {
            if (!RotateAroundPlayer)
                return false;

            if (!RotateMiddleMouseButton)
                return true;

            if (RotateMiddleMouseButton && Input.GetMouseButton(2))
                return true;

            return false;
        }
    }

    // LateUpdate is called after Update methods
    void Update()
    {
        
        if(!gameManager.gamePaused)
            principalFunction();
    }

    void LateUpdate()
    {
        if(!gameManager.gamePaused)
            collide();
    }

    void genForwardDirection() {
        getForwardDirection().transform.position = PlayerTransform.position+_cameraOffset;
        getForwardDirection().transform.LookAt(PlayerTransform);
    }

    private GameObject getForwardDirection() {
        return ForwardDirection!=null ? ForwardDirection : (ForwardDirection = new GameObject("_null"));
    }

    public Transform getForwardDirectionTransform() {
        return getForwardDirection().transform;
    }

    public void principalFunction()
    {
        if (true)
        {

            float h = Input.GetAxis("Mouse X") * RotationsSpeed;
            float v = Input.GetAxis("Mouse Y") * RotationsSpeed;

            Quaternion camTurnAngle = Quaternion.AngleAxis(h, Vector3.up);

            Quaternion camTurnAngleY = Quaternion.AngleAxis(-v, transform.right);

            Vector3 newCameraOffset = camTurnAngle * camTurnAngleY * _cameraOffset;

            // Limit camera pitch
            if (newCameraOffset.y < CameraPitchMin || newCameraOffset.y > CameraPitchMax)
            {
                newCameraOffset = camTurnAngle * _cameraOffset;
            }

            _cameraOffset = newCameraOffset;

        }

        RaycastHit hit;
        genForwardDirection();
 
        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(PlayerTransform.TransformPoint(bumperRayOffset), _cameraOffset, out hit, bumperDistanceCheck)
            && hit.transform != PlayerTransform) // ignore ray-casts that hit the user. DR
        {
            // clamp wanted position to hit position
            Collider collider = hit.transform.gameObject.GetComponent<Collider>();
            if (collider!=null && !collider.isTrigger) return;
        }

        Vector3 newPos = PlayerTransform.position + _cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer || RotateAroundPlayer)
            transform.LookAt(PlayerTransform);
    }

    [SerializeField] private float damping = 10.0f;
    [SerializeField] private float rotationDamping = 10.0f;
    private Vector3 targetLookAtOffset; // allows offsetting of camera lookAt, very useful for low bumper heights
    [SerializeField] private float bumperDistanceCheck = 2.5f; // length of bumper ray
   // [SerializeField] private float bumperCameraHeight = 1.0f; // adjust camera height while bumping
    [SerializeField] private Vector3 bumperRayOffset = new Vector3(0, -0.05f, 0);

    public void collide() {
        Vector3 wantedPosition = PlayerTransform.position + _cameraOffset;
 
        // check to see if there is anything behind the target
        RaycastHit hit;
 
        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(PlayerTransform.TransformPoint(bumperRayOffset), _cameraOffset, out hit, bumperDistanceCheck)
            && hit.transform != PlayerTransform) // ignore ray-casts that hit the user. DR
        {
            Collider collider = hit.transform.gameObject.GetComponent<Collider>();
            if (collider==null || collider.isTrigger) return;
            // clamp wanted position to hit position
            wantedPosition.x = hit.point.x;
            wantedPosition.z = hit.point.z;
            wantedPosition.y = hit.point.y>wantedPosition.y ? hit.point.y + 0.01f : hit.point.y;
            //wantedPosition.y = Mathf.Lerp(hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * damping);
        } 
 
        SetPlayerAlpha(((PlayerTransform.position-wantedPosition).magnitude*0.75f)+0.25f);
        //transform.position = wantedPosition;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
 
        Vector3 lookPosition = PlayerTransform.TransformPoint(targetLookAtOffset);

        Quaternion wantedRotation = Quaternion.LookRotation(lookPosition - transform.position, PlayerTransform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
        transform.LookAt(PlayerTransform);
        //transform.rotation = Quaternion.LookRotation(lookPosition - transform.position, PlayerTransform.up);
    }
}