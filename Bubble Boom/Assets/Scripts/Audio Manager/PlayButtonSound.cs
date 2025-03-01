using UnityEngine;
using UnityEngine.UI;

public class PlayButtonSound : MonoBehaviour
{
    public string audioClip;
    Button[] buttons;

    private void Start()
    {
        buttons = FindObjectsOfType<Button>();

        foreach (var button in buttons)
        {
            button.onClick.AddListener(PlayAudio);
        }
    }

    private void PlayAudio()
    {
        FindObjectOfType<AudioManager>().SetCurrentSoundFXClip(audioClip);
    }
}
