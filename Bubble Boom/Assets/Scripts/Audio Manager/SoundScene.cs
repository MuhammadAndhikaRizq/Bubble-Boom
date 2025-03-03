using UnityEngine;

public class SoundScene : MonoBehaviour
{
    public string soundName;
    void Start()
    {
        FindObjectOfType<AudioManager>().SetCurrentBgmClip(soundName);
    }


}
