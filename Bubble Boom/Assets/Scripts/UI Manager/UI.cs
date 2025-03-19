using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
   [SerializeField] private GameObject[] uiElements;
   [SerializeField] private GameObject startUI;
   
    private void Start()
    {
        Time.timeScale = 0; // Pause game saat pertama kali dijalankan
        if (startUI != null)
            startUI.SetActive(true);
    }

   public void SwitchTo(GameObject uiEnabled)
   {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false);
        }
        uiEnabled.SetActive(true);
   }

    public void PlayButton()
    {
        Time.timeScale = 1; // Melanjutkan permainan
        if (startUI != null)
            startUI.SetActive(false);
      
    }

   public void QuitButton()
   {
        // if(EditorApplication.isPlaying)
        //     EditorApplication.isPlaying = false;
        // else
            Application.Quit();
   }
}
