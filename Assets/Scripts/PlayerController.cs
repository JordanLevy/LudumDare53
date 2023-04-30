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

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        lastDropTime = -dropCooldown;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(Mathf.Abs(horizontal) < 0.01 && Mathf.Abs(vertical) < 0.01){
            animator.SetFloat("Speed", 0);
        }
        else {
            animator.SetFloat("Speed", 0.5f);
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
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isHoldingObject && Time.time > lastDropTime + dropCooldown && newspaperLayer == (newspaperLayer | (1 << collision.gameObject.layer)))
        {
            PickupObject(collision.gameObject);
        }
    }

    void PickupObject(GameObject gameObject){
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
        animator.SetTrigger("Bark");
        if(isHoldingObject){
            DropObject();
        }
        GameObject hitbox = Instantiate(barkHitbox, transform.position, transform.rotation, orientation);
        Destroy(hitbox, 0.1f);
    }
}
