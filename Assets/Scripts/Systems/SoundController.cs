using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    // 게임 내 소리 설정 UI 제어 스크립트. 슬라이더/음소거 버튼과 오디오 믹서를 연동하고, 소리에 따라 아이콘 자동 전환을 수행.

    [Header("BGM UI")]
    [SerializeField] private Slider bgmSlider;//0~1
    [SerializeField] private Button bgmMuteButton;//bgm음소거 토글 버튼
    [SerializeField] private Image bgmIcon;//bgm 상태 표시
    [SerializeField] private Sprite iconSoundOn;//사운드 켜짐 아이콘
    [SerializeField] private Sprite iconSoundOff;//사운드 꺼짐(음소거) 아이콘

    [Header("SFX UI")]
    [SerializeField] private Slider sfxSlider;//0~1
    [SerializeField] private Button sfxMuteButton;//sfx음소거 토글 버튼
    [SerializeField] private Image sfxIcon;//sfx상태를 표시

    void Start()
    {
        if (AudioManager.Instance == null) return;

        if (bgmSlider != null)//슬라이더 초기값 동기화
        {
            bgmSlider.SetValueWithoutNotify(AudioManager.Instance.GetBgmVolumeLinear());//현재 BGM 볼륨을 슬라이더에 반영
            bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);//슬라이더 값 변경 시 콜백 등록
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSfxVolumeLinear());//현재 SFX 볼륨을 슬라이더에 반영
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);//슬라이더값 변경 시 콜백 등록
        }
        if (bgmMuteButton != null)bgmMuteButton.onClick.AddListener(OnBgmMuteClicked);

        if (sfxMuteButton != null)sfxMuteButton.onClick.AddListener(OnSfxMuteClicked);

        UpdateBgmIcon();
        UpdateSfxIcon();
    }

    private void OnBgmSliderChanged(float v)//BGM 슬라이더 값 변경 시 호출되는 콜백 메서드
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetBgmVolume(v);//SetBgmVolume()으로 변경된 bgm 볼륨 값을 BGM 볼륨 슬라이더에 반영
        UpdateBgmIcon();
    }

    private void OnSfxSliderChanged(float v)//SFX 슬라이더 값 변경 시 호출되는 콜백 메서드
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetSfxVolume(v);//SetSfxVolume()으로 변경된 sfx 볼륨 값읋 sfx 슬라이더에 반영
        UpdateSfxIcon();
    }

    private void OnBgmMuteClicked()//bgm 음소거 버튼 클릭 시 호출되는 메서드
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.ToggleBgmMute();
        UpdateBgmIcon();
    }

    private void OnSfxMuteClicked()// sfx 음소거 버튼 클릭 시 호출되는 메서드
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.ToggleSfxMute();
        UpdateSfxIcon();
    }
    private void UpdateBgmIcon()//현재 bgm 상태에 따라 아이콘을 변경하는 메서드
    {
        if (AudioManager.Instance == null || bgmIcon == null) return;
        bool effectivelyMuted = AudioManager.Instance.IsBgmMuted() || AudioManager.Instance.GetBgmVolumeLinear() <= 0.0001f;//음소거 이벤트 발생 혹은 선형볼륨이 매우 적으면 true
        bgmIcon.sprite = effectivelyMuted ? iconSoundOff : iconSoundOn;// effectivelyMuted이 true이면 사운드오프 아이콘으로 바꾸고, false면 그냥 사운드 아이콘으로 둔다.
    }

    private void UpdateSfxIcon()//현재 sfx 상태에 따라 아이콘을 변경하는 메서드.
    {
        if (AudioManager.Instance == null || sfxIcon == null) return;
        bool effectivelyMuted = AudioManager.Instance.IsSfxMuted() || AudioManager.Instance.GetSfxVolumeLinear() <= 0.0001f;
        sfxIcon.sprite = effectivelyMuted ? iconSoundOff : iconSoundOn;
    }

    void OnDestroy()
    {
        if (bgmSlider != null) bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        if (bgmMuteButton != null) bgmMuteButton.onClick.RemoveListener(OnBgmMuteClicked);
        if (sfxMuteButton != null) sfxMuteButton.onClick.RemoveListener(OnSfxMuteClicked);
    }
    

}
