using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _soundFXSource;
    [SerializeField] private AudioData _audioData;
    [SerializeField] private AudioSetting _audioSetting;
    private bool _isBgmMute = false;
    private bool _isSoundMute;

    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Init();
    }

    public AudioData GetAudioData()
    {
        return _audioData;
    }

    private void FixedUpdate()
    {
        _bgmSource.volume = _audioSetting.Volume;
    }

    private void OnEnable()
    {
        AudioButtonSetting.OnToggleBgmClick += MuteBgm;

    }

    private void OnDisable()
    {
        AudioButtonSetting.OnToggleBgmClick -= MuteBgm;
    }

    private void Init()
    {
        _isBgmMute = _audioSetting.IsBgmMuted;
        _isSoundMute = _audioSetting.IsSoundsMuted;
    }

    public void MuteBgm(bool isBgmMute)
    {
        _isBgmMute = isBgmMute;
        if (_isBgmMute == true)
        {
            _bgmSource.Stop();
        }
        else
        {
            _bgmSource.Play();
        }
    }

    public void MuteSound(bool isSoundMute)
    {
        _isSoundMute = isSoundMute;
        if (_isSoundMute == true)
        {
            _soundFXSource.volume = 0;
        }
        else
        {
            _soundFXSource.volume = 1;
        }
    }

    public void SetCurrentBgmClip(string clip)
    {
        for (int i = 0; i < _audioData.BgmList.Count; i++)
        {
            var soundName = _audioData.BgmList[i].BgmName;
            if (soundName == clip)
            {
                var currentClip = _audioData.BgmList[i].Clip;
                _bgmSource.clip = currentClip;
                if (!_bgmSource.isPlaying && _bgmSource.clip.name != clip)
                {
                    if (!_isBgmMute)
                    {

                        _bgmSource.Play();
                    }
                    else if (_isBgmMute)
                    {
                        _bgmSource.Stop();
                    }
                }
                break;
            }
        }
    }

    public void SetCurrentSoundFXClip(string clip)
    {
        for (int i = 0; i < _audioData.SoundList.Count; i++)
        {
            var soundName = _audioData.SoundList[i].SoundName;
            if (soundName == clip)
            {
                _soundFXSource.clip = _audioData.SoundList[i].Clip;
                _soundFXSource.volume = _audioData.SoundList[i].Volume * _audioSetting.Volume;
                if (!_isSoundMute)
                {
                    _soundFXSource.Play();
                }
                else
                {
                    _soundFXSource.Stop();
                }
            }
        }
    }

   /* //function to play Bgm Source
    private void OnMainMenu()
    {
        SetCurrentBgmClip("Home");
    }

    private void OnGameplay()
    {
        SetCurrentBgmClip("Gameplay");
    }

    //function to play SoundFx Source

    private void OnClick()
    {
        SetCurrentSoundFXClip("ButtonClick");
    }*/

}

