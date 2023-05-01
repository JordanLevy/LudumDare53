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
    public float walkSpeed;

    public int type;
    private string[] solutionSequence;
    private int solutionIndex = 0;
    private string[] nextSolution;
    public bool interactable = true;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Color[] clothesColors;
    private RandomSoundPlayer randomSoundPlayer;
    private Rigidbody rigidBody;
    public Vector3[] turningRange;
    public Vector3[] lawnRange;
    public Vector3[] fleeRange;
    private List<Vector3> targetPoints = new List<Vector3>();
    private Vector3 fleePoint;
    public Vector3 mailbox;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        randomSoundPlayer = GetComponent<RandomSoundPlayer>();
        rigidBody = GetComponent<Rigidbody>();
        SetType(type);
        PickupObject(Instantiate(newspaperPrefab));
        targetPoints = new List<Vector3>();
        float random = Random.Range(0f, 1f);
        targetPoints.Add(Vector3.Lerp(turningRange[0], turningRange[1], random));
        random = Random.Range(0f, 1f);
        targetPoints.Add(Vector3.Lerp(lawnRange[0], lawnRange[1], random));
        random = Random.Range(0f, 1f);
        targetPoints.Add(mailbox);
        fleePoint = Vector3.Lerp(fleeRange[0], fleeRange[1], random);
        targetPoints.Add(fleePoint);
    }

    // Update is called once per frame
    void Update()
    {
        if(targetPoints.Count == 0){
            return;
        }
        if(Vector3.Distance(targetPoints[0], transform.position) < 1f){
            targetPoints.RemoveAt(0);
            if(targetPoints.Count == 0){
                Destroy(gameObject);
                return;
            }
        }
        

        Vector3 velocity = walkSpeed * Vector3.Normalize(targetPoints[0] - transform.position);
        velocity.y = rigidBody.velocity.y;

        int layerIndex = animator.GetLayerIndex("Base Layer");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        if(!stateInfo.IsName("Armature|Walk") && !stateInfo.IsName("Armature|RunAway") && 
            !stateInfo.IsName("Armature|WalkAway")){
            velocity = Vector3.zero;
        }
        
        // if mailman should flee, flee
        stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        if(stateInfo.IsName("Armature|RunAway") || stateInfo.IsName("Armature|WalkAway")){
            targetPoints = new List<Vector3>();
            float random = Random.Range(0f, 1f);
            targetPoints.Add(fleePoint);
            gameObject.layer = LayerMask.NameToLayer("FleeingMailman");
        }
        rigidBody.velocity = velocity;
        if (velocity.sqrMagnitude > 0.01f) {
            Quaternion rotation = Quaternion.LookRotation(velocity, Vector3.up);
            rigidBody.MoveRotation(rotation);
        }

    }

    public void SetType(int type){
        this.type = type;
        solutionIndex = 0;
        interactable = true;
        if(type == 0){
            solutionSequence = new string[1];
            solutionSequence[0] = "Bark,FallOver,true";
            
        }
        else if(type == 1){
            solutionSequence = new string[3];
            solutionSequence[0] = "Bark,Startle,false";
            solutionSequence[1] = "Bark,Startle,false";
            solutionSequence[2] = "Bark,FallOver,true";
        }
        else if(type == 2){
            solutionSequence = new string[1];
            solutionSequence[0] = "Sit,Kneel,true";
        }
        else if(type == 3){
            solutionSequence = new string[3];
            solutionSequence[0] = "Bark,Nod,false";
            solutionSequence[1] = "Bark,Nod,false";
            solutionSequence[2] = "Sit,Kneel,true";
        }
        skinnedMeshRenderer.materials[0].SetColor("_BaseColor", clothesColors[type]);
        if(solutionIndex < solutionSequence.Length){
            nextSolution = solutionSequence[solutionIndex].Split(",");
        }
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
            else{
                interactable = false;
            }
        }
        else{
            animator.SetTrigger("Wrong");
            randomSoundPlayer.PlayRandomSound(0, 0);
            ScoreController.IncrementStrikes();
            interactable = false;
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

    public void WalkAway(){
        animator.SetTrigger("WalkAway");
        interactable = false;
    }
}
