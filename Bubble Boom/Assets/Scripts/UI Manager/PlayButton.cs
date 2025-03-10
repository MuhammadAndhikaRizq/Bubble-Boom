using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public string playSceneName = "GameScene"; // Ganti dengan nama scene yang ingin dimuat saat menekan Play

    void Update()
    {
        // Tekan ESC untuk keluar dari game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    // Fungsi untuk tombol Play
    public void PlayGame()
    {
        SceneManager.LoadScene(playSceneName);
    }

    // Fungsi untuk keluar dari game
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
