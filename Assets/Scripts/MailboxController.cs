using UnityEngine;

public class MailboxController : MonoBehaviour
{
    public GameObject particleEffect;
    public LayerMask newspaperLayer;

    private void OnTriggerEnter(Collider collider)
    {
        if (newspaperLayer == (newspaperLayer | (1 << collider.gameObject.layer)))
        {
            ScoreController.scoreValue++;
            //Instantiate(particleEffect, transform.position, Quaternion.identity);
            Destroy(collider.gameObject);
        }
    }
}