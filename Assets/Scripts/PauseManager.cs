using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject[] pausePanels;
    private static int pausedPanel = -1;
    private static PauseManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start(){
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        for(int i = 0; i < pausePanels.Length; i++){
            pausePanels[i].SetActive(false);
        }
    }

    void Update()
    {
        if(IsPaused()){
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
            {
                ResumeGame();
            }
        }
        else{
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame(0);
            }
        }
    }

    public static void PauseGame(int panelIndex)
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pausedPanel = panelIndex;
        instance.pausePanels[pausedPanel].SetActive(true);
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        instance.pausePanels[pausedPanel].SetActive(false);
        pausedPanel = -1;
    }

    public static bool IsPaused(){
        return pausedPanel != -1;
    }
}