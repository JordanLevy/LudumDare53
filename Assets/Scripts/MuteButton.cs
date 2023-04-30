using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public AudioSource audioSource;
    public Toggle muteToggle;

    void Start()
    {
    }

    public void ToggleMute()
    {
        audioSource.mute = !audioSource.mute;
    }
}