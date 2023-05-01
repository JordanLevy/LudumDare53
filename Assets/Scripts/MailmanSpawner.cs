using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailmanSpawner : MonoBehaviour
{
    public GameObject mailmanPrefab;
    public float spawnInterval;
    private SphereCollider spawnAreaCollider;
    private float spawnTimer = 0f;
    public int numTypes;
    private int minType = 0;
    private int maxType = 0; // the maximum mailman type we will spawn. Goes up over time
    private int pausePhase = 0;
    private float startTime;


    void Start(){
        spawnAreaCollider = GetComponent<SphereCollider>();
        spawnTimer = spawnInterval;
        startTime = Time.time;
        pausePhase = 0;
        minType = 0;
        maxType = 0;
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnMailman();
            spawnTimer = 0f;
        }

        if(pausePhase == 0 && Time.time - startTime >= 2f){ // movement controls
            pausePhase = 1;
            PauseManager.PauseGame(0);
        }
        else if(pausePhase == 1 && Time.time - startTime >= 5f){ // mailman0
            minType = 0;
            maxType = 0;
            pausePhase = 2;
            PauseManager.PauseGame(1);
        }
        else if(pausePhase == 2 && Time.time - startTime >= 20f){ // mailman1
            minType = 1;
            maxType = 1;
            pausePhase = 3;
            PauseManager.PauseGame(2);
        }
        else if(pausePhase == 3 && Time.time - startTime >= 45f){ // mailman2
            minType = 2;
            maxType = 2;
            pausePhase = 4;
            PauseManager.PauseGame(3);
        }
        else if(pausePhase == 4 && Time.time - startTime >= 70f){ // mailman3
            minType = 3;
            maxType = 3;
            pausePhase = 5;
            PauseManager.PauseGame(4);
        }
        else if(pausePhase == 5 && Time.time - startTime >= 90f){ // mailman3
            minType = 0;
            maxType = 3;
            pausePhase = 6;
        }
    }

    void SpawnMailman()
    {
        float radius = spawnAreaCollider.radius;
        Vector3 center = spawnAreaCollider.transform.position;

        Vector3 randomUnitSphere = Random.insideUnitSphere;
        randomUnitSphere.y = 0;
        Vector3 spawnPosition = center + randomUnitSphere * radius;

        int maxAttempts = 10;
        int attempts = 0;
        LayerMask mask = LayerMask.GetMask("Mailman", "Dog");
        while (Physics.CheckSphere(spawnPosition, 1f, mask) && attempts < maxAttempts)
        {
            randomUnitSphere = Random.insideUnitSphere;
            randomUnitSphere.y = 0;
            spawnPosition = center + randomUnitSphere * radius;
            attempts++;
        }

        if (attempts < maxAttempts)
        {
            GameObject mailman = Instantiate(mailmanPrefab, spawnPosition, Quaternion.identity);
            MailmanController controller = mailman.GetComponent<MailmanController>();
            int mailmanType = Random.Range(minType, maxType);
            controller.SetType(mailmanType);
        }
        else
        {
            Debug.LogWarning("Could not find a free spawn position for mailman");
        }
    }
}