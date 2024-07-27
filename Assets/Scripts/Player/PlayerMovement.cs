using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
  [Header("Movement")]
  private bool grounded;
  private bool crouched = false;
  private float groundDistance;
  [SerializeField]private float groundCheckDistance = 0.1f;

  private float standHeight;
  private float capsuleStandHeight;
  private float camStandHeight;
  [SerializeField]private float crouchDistance = 0.5f;
  [SerializeField]private float crouchSpeedMultiplier = 0.5f;
  [SerializeField]private CapsuleCollider capsuleCollider;

  [SerializeField]private float jumpForce = 35f;
  [SerializeField]private float gravityMultiplier = 2f;
  [SerializeField]private float maxDragDelay = 0.05f;
  [SerializeField]private float maxCoyoteTime = 0.2f;
  [SerializeField]private float maxNoyoteTime = 0.3f; //time after jumping that player can't jump again
  private float dragDelay;
  private float coyoteTime = 0f;
  private float noyoteTime = 0f;
  private bool jumpBuffered = false;

  [SerializeField]private string horizontalMovementAxis = "Horizontal";
  [SerializeField]private string verticalMovementAxis = "Vertical";
  [SerializeField]private float forwardSpeed = 5f;
  [SerializeField]private float sideSpeed = 1f;
  [SerializeField]private float maxGroundSpeed = 50f;
  [SerializeField]private float maxAirSpeed = 9f;
  [SerializeField]private float maxAcceleration = 10;
  [SerializeField]private float groundDrag = 1f;
  [SerializeField]private float dragRamp = 0.7f;

  [HideInInspector]public float haltMovementTime = 0f;

  [SerializeField]private string horizontalLookAxis = "Mouse X";
  [SerializeField]private string verticalLookAxis = "Mouse Y";
  [SerializeField]private float lookSensitivity = 20f;
  private float camRotationX;

  private Rigidbody rb;
  private Collider col;
  public Camera cam;

  public override void OnNetworkSpawn(){
    rb = GetComponent<Rigidbody>();

    maxAcceleration *= maxGroundSpeed;

    groundDistance = capsuleCollider.bounds.extents.y;
    groundCheckDistance += groundDistance;
    capsuleStandHeight = capsuleCollider.center.y;
    standHeight = capsuleCollider.height;
    camStandHeight = cam.transform.localPosition.y;
    camRotationX = cam.transform.localRotation.eulerAngles.x;

    if(!IsOwner){
      Destroy(cam.gameObject);
      return;
    }

    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

    base.OnNetworkSpawn();
  }

  void FixedUpdate(){
    if(!IsOwner){return;}
    haltMovementTime -= Time.fixedDeltaTime;

    if(haltMovementTime <= 0f){
      Move();
    }
    DoGravity();
  }

  void Update(){
    if(!IsOwner){return;}
    dragDelay -= Time.deltaTime;

    Look();
    //groundDrag = CheckGrounded();
    Jump();
    Crouch();
    DoDeathBarrier();

    if(grounded){
      Debug.DrawRay(cam.transform.position, cam.transform.forward * 3, Color.green);
    } else {
      Debug.DrawRay(cam.transform.position, cam.transform.forward * 3, Color.red);
    }
  }

  void DoGravity(){
    rb.AddForce(Physics.gravity * (25 * (gravityMultiplier - 1)) * Time.fixedDeltaTime, ForceMode.Acceleration);
  }

  void Move(){
    Vector3 wishVector = transform.TransformDirection(new Vector3(Input.GetAxis(horizontalMovementAxis) * sideSpeed, 0f, Input.GetAxis(verticalMovementAxis) * forwardSpeed)).normalized;
    float currentSpeed = Vector3.Dot(rb.velocity, wishVector);

    if(grounded && dragDelay <= 0f){
      //drag
      float subtractSpeed = Mathf.Clamp(rb.velocity.magnitude * dragRamp, 0f, groundDrag * Time.fixedDeltaTime);
      Vector3 dragVector = subtractSpeed * rb.velocity;
      //dragVector.y = 0f;
      rb.velocity -= dragVector;

      //recalculate speed
      currentSpeed = Vector3.Dot(rb.velocity, wishVector);
    }

    float speedUsed = (grounded ? maxGroundSpeed : maxAirSpeed);
    speedUsed *= crouched ? crouchSpeedMultiplier : 1f;

    float addSpeed = Mathf.Clamp(speedUsed / 10f - currentSpeed, 0f, maxAcceleration * Time.fixedDeltaTime);

    rb.velocity = (rb.velocity + addSpeed * wishVector);
  }

  bool CheckGrounded(){
    RaycastHit hit;
    if(Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDistance)){
      if(!grounded){
        dragDelay = maxDragDelay;
      }

      coyoteTime = maxCoyoteTime;
      return true;
    } else {
      return false;
    }
  }

  void Jump(){
    noyoteTime -= Time.deltaTime;

    if(!grounded){
      coyoteTime -= Time.deltaTime;
    }

    if(Input.GetKeyDown(KeyCode.Space)){
      jumpBuffered = true;
    }
    if(!Input.GetKey(KeyCode.Space)){
      jumpBuffered = false;
    }

    if(jumpBuffered && (grounded || coyoteTime > 0f) && noyoteTime <= 0f){
      rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
      rb.AddForce(new Vector3(0, jumpForce * 0.25f, 0), ForceMode.Impulse);
      grounded = false;
      jumpBuffered = false;
      coyoteTime = 0f;
      noyoteTime = maxNoyoteTime;
    }
  }

  void Crouch(){
    if(Input.GetKey(KeyCode.LeftShift)){
      crouched = true;
      cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, camStandHeight - crouchDistance, cam.transform.localPosition.z);
      capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleStandHeight - crouchDistance/2, capsuleCollider.center.z);
      capsuleCollider.height = standHeight - crouchDistance;
    } else {
      crouched = false;
      cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, camStandHeight, cam.transform.localPosition.z);
      capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleStandHeight, capsuleCollider.center.z);
      capsuleCollider.height = standHeight;
    }
  }

  void Look(){
    transform.Rotate(0f, Input.GetAxis(horizontalLookAxis) * lookSensitivity, 0f);
    camRotationX = Mathf.Clamp(camRotationX + Input.GetAxis(verticalLookAxis) * lookSensitivity, -85f, 85f);

    cam.transform.localRotation = Quaternion.Euler(-camRotationX, 0, 0);
  }

  void DoDeathBarrier(){
    if(transform.position.y <= -10f){
      GetComponent<Health>().ChangeHealthServerRpc(-99999, 9999);
    }
  }

  void OnCollisionStay(Collision collisionInfo){
    if(collisionInfo.transform.tag == "Ground"){
      grounded = CheckGrounded();
    }
  }

  void OnCollisionExit(){
    grounded = false;
  }
}
