using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioButtonSetting : MonoBehaviour
{
    public static UnityAction<bool> OnToggleBgmClick;
    public static UnityAction<bool> OnToggleSoundCLick;

    [SerializeField] private Button _bgmButton;
    [SerializeField] private GameObject _bgmControl;
    [SerializeField] private Vector3 _onPos;
    [SerializeField] private Vector3 _offPos;
    [SerializeField] private Sprite OnSprite, OffSprite;
    [SerializeField] private Slider _volumeControl;
    [SerializeField] private AudioData _audioData;
    [SerializeField] private AudioSetting _audioSetting;
    private bool _isBgmMute;

    [SerializeField] private enum _buttonType
    {
        ToggleBgm,
        ToggleSound
    }

    private void Start()
    {
        Init();
        _bgmButton.onClick.AddListener(ToggleBgm);
            
    }

    private void FixedUpdate()
    {
        _audioSetting.Volume = _volumeControl.value;
    }

    private void Init()
    {
        _isBgmMute = _audioSetting.IsBgmMuted;

        if(_isBgmMute)
        {
            _bgmControl.GetComponent<RectTransform>().anchoredPosition = _offPos;
        }
        else
        {
            _bgmControl.GetComponent<RectTransform>().anchoredPosition = _onPos;
        }

        _volumeControl.value = _audioSetting.Volume;
    }

    private void SaveVolumeValue()
    {
        _audioSetting.Volume = _volumeControl.value;
    }

    private void ToggleBgm()
    {
        _isBgmMute = !_isBgmMute;
        _audioSetting.IsBgmMuted = _isBgmMute;
        OnToggleBgmClick?.Invoke(_isBgmMute);
        if (_isBgmMute)
        {
            _bgmControl.GetComponent<RectTransform>().anchoredPosition = _offPos;
            _bgmButton.GetComponent<Image>().sprite = OffSprite;
        }
        else
        {
            _bgmControl.GetComponent<RectTransform>().anchoredPosition = _onPos;
            _bgmButton.GetComponent<Image>().sprite = OnSprite;
        }

    }


}
