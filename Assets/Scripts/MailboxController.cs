using UnityEngine;

public class MailboxController : MonoBehaviour
{
    public GameObject particleEffect;
    public LayerMask newspaperLayer;
    public LayerMask mailmanLayer;
    private RandomSoundPlayer scoreSoundPlayer;

    void Start(){
        scoreSoundPlayer = GetComponent<RandomSoundPlayer>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (newspaperLayer == (newspaperLayer | (1 << collider.gameObject.layer)))
        {
            ScoreController.IncrementScore();
            scoreSoundPlayer.PlayRandomSound(0, 0);
            //Instantiate(particleEffect, transform.position, Quaternion.identity);
            Destroy(collider.gameObject);
        }
        else if (mailmanLayer == (mailmanLayer | (1 << collider.gameObject.layer)))
        {
            ScoreController.IncrementStrikes();
            scoreSoundPlayer.PlayRandomSound(1, 1);
            //Instantiate(particleEffect, transform.position, Quaternion.identity);
            MailmanController mailman = collider.gameObject.GetComponent<MailmanController>();
            mailman.WalkAway();
        }
    }
}