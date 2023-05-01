using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    public AudioClip[] soundClips;
    public float spatialBlend;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = spatialBlend;
    }

    public void PlayRandomSound(int min, int max)
    {
        int clipIndex = Random.Range(min, max + 1);
        audioSource.clip = soundClips[clipIndex];
        audioSource.Play();
    }
}