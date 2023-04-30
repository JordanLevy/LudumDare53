using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailmanController : MonoBehaviour
{
    private Animator animator;
    public GameObject newspaperPrefab;
    public Transform objectHoldLocation;
    public float dropSpeed;
    private GameObject heldObject;
    private bool isHoldingObject = false;
    public string[] solutionSequence;
    private int solutionIndex = 0;
    private string[] nextSolution;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        PickupObject(Instantiate(newspaperPrefab));
        if(solutionIndex < solutionSequence.Length){
            nextSolution = solutionSequence[solutionIndex].Split(",");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit(string hitboxType){
        if(hitboxType == nextSolution[0]){
            animator.SetTrigger(nextSolution[1]);
            if(nextSolution[2] == "true"){
                DropObject();
            }
            solutionIndex++;
            if(solutionIndex < solutionSequence.Length){
                nextSolution = solutionSequence[solutionIndex].Split(",");
            }
        }
    }

    void PickupObject(GameObject gameObject){
        heldObject = gameObject;
        isHoldingObject = true;
        heldObject.transform.parent = objectHoldLocation;
        heldObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
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

            rb.velocity = -transform.up * dropSpeed;
            
            isHoldingObject = false;
            heldObject = null;
        }
    }
}
