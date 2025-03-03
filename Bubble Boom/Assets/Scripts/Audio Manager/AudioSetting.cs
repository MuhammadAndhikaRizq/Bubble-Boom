using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class AudioSetting : ScriptableObject
{
    public bool IsBgmMuted;
    public bool IsSoundsMuted;
    [Range(0f, 1f)] public float Volume;
}
