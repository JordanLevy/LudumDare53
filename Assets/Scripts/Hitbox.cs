using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public string hitboxType;

    private void OnTriggerEnter(Collider other)
    {
        MailmanController mailman = other.GetComponent<MailmanController>();
        if (mailman != null)
        {
            mailman.GetHit(hitboxType);
        }
    }
}