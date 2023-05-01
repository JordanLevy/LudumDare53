using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    private float walkThreshold;
    public float turnSpeed;
    public float turnRadius;

    public Transform orientation;
    public Transform cameraPivot;

    private Vector3 moveDirection;
    private Rigidbody rigidBody;

    private Animator animator;

    public LayerMask newspaperLayer;
    public Transform objectHoldLocation;
    public float dropSpeed;
    private GameObject heldObject;
    private bool isHoldingObject = false;
    public float dropCooldown;
    private float lastDropTime = 0.0f;
    public GameObject barkHitbox;
    public GameObject sitHitbox;
    private RandomSoundPlayer mouthSoundPlayer;
    public float barkHitboxRadius;
    public float sitHitboxRadius;
    public float barkCastDistance;
    public float sitCastDistance;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mouthSoundPlayer = GetComponent<RandomSoundPlayer>();
        lastDropTime = -dropCooldown;
    }

    void Update()
    {
        if(PauseManager.IsPaused()){
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // if we're sitting, we can't move
        int layerIndex = animator.GetLayerIndex("Base Layer");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        if(stateInfo.IsName("Armature|Sit")){
            horizontal = 0;
            vertical = 0;
        }

        if(Mathf.Abs(horizontal) < 0.01 && Mathf.Abs(vertical) < 0.01){
            animator.SetFloat("Speed", 0);
        }
        else {
            animator.SetFloat("Speed", 0.5f);
        }

        // turning while in reverse
        if(vertical < 0 && Mathf.Abs(horizontal) > 0.01){
            horizontal = -horizontal;
        }

        if(Mathf.Abs(horizontal) > 0.01 && Mathf.Abs(vertical) < 0.01){
            vertical = Mathf.Abs(horizontal) * Mathf.Sign(vertical);
            horizontal *= Mathf.Sign(vertical);
        }

        Vector3 velocity = orientation.forward * vertical * walkSpeed;

        rigidBody.angularVelocity = new Vector3(0, horizontal * turnSpeed , 0);

        velocity.y = rigidBody.velocity.y;
        rigidBody.velocity = velocity;

        if(Input.GetMouseButtonDown(0)){
            Bark();
        }

        if(Input.GetKeyDown(KeyCode.E)){
            Sit();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isHoldingObject && Time.time > lastDropTime + dropCooldown && newspaperLayer == (newspaperLayer | (1 << collision.gameObject.layer)))
        {
            PickupObject(collision.gameObject);
        }
    }

    void PickupObject(GameObject gameObject){
        mouthSoundPlayer.PlayRandomSound(3, 3);
        heldObject = gameObject;
        isHoldingObject = true;
        heldObject.transform.parent = objectHoldLocation;
        heldObject.transform.localRotation = Quaternion.Euler(0, 90, 0);
        heldObject.transform.localPosition = new Vector3(0, 0, 0);
        
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }
        
        Collider collider = heldObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    void DropObject()
    {
        if (isHoldingObject)
        {
            heldObject.transform.parent = null;
            
            Rigidbody rb = heldObject.AddComponent<Rigidbody>();
            heldObject.GetComponent<Rigidbody>().useGravity = true;
            
            Collider collider = heldObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            rb.velocity = orientation.forward * dropSpeed;
            
            lastDropTime = Time.time;
            isHoldingObject = false;
            heldObject = null;
        }
    }

    void Bark(){
        mouthSoundPlayer.PlayRandomSound(0, 2);
        animator.SetTrigger("Bark");
        if(isHoldingObject){
            DropObject();
        }
        RaycastHit[] hits = new RaycastHit[3];
        if (Physics.SphereCastNonAlloc(orientation.position, barkHitboxRadius, orientation.forward, hits, barkCastDistance, LayerMask.GetMask("Mailman")) > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                MailmanController mailman = null;
                try{
                    mailman = hit.collider.GetComponent<MailmanController>();
                }
                catch{
                    continue;
                }
                if (mailman != null && mailman.interactable)
                {
                    mailman.GetHit("Bark");
                    break; // exit the loop after hitting the first eligible mailman
                }
            }
        }
        Debug.DrawRay(orientation.position, orientation.forward * barkCastDistance, Color.red, 1f);
        Debug.DrawRay(orientation.position + orientation.right * barkHitboxRadius, orientation.forward * barkCastDistance, Color.red, 1f);
        Debug.DrawRay(orientation.position - orientation.right * barkHitboxRadius, orientation.forward * barkCastDistance, Color.red, 1f);
        Debug.DrawRay(orientation.position, orientation.right * barkHitboxRadius, Color.red, 1f);
        Debug.DrawRay(orientation.position, -orientation.right * barkHitboxRadius, Color.red, 1f);
    }

    void Sit(){
        animator.SetTrigger("Sit");
        RaycastHit[] hits = new RaycastHit[3];
        if (Physics.SphereCastNonAlloc(orientation.position, sitHitboxRadius, orientation.forward, hits, sitCastDistance, LayerMask.GetMask("Mailman")) > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                MailmanController mailman = hit.collider.GetComponent<MailmanController>();
                if (mailman != null && mailman.interactable)
                {
                    mailman.GetHit("Sit");
                    break; // exit the loop after hitting the first eligible mailman
                }
            }
        }
        Debug.DrawRay(orientation.position, orientation.forward * sitCastDistance, Color.red, 1f);
        Debug.DrawRay(orientation.position + orientation.right * sitHitboxRadius, orientation.forward * sitCastDistance, Color.red, 1f);
        Debug.DrawRay(orientation.position - orientation.right * sitHitboxRadius, orientation.forward * sitCastDistance, Color.red, 1f);
        Debug.DrawRay(orientation.position, orientation.right * sitHitboxRadius, Color.red, 1f);
        Debug.DrawRay(orientation.position, -orientation.right * sitHitboxRadius, Color.red, 1f);
    }
}
