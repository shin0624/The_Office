using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    // ���� �� �Ҹ� ���� UI ���� ��ũ��Ʈ. �����̴�/���Ұ� ��ư�� ����� �ͼ��� �����ϰ�, �Ҹ��� ���� ������ �ڵ� ��ȯ�� ����.

    [Header("BGM UI")]
    [SerializeField] private Slider bgmSlider;//0~1
    [SerializeField] private Button bgmMuteButton;//bgm���Ұ� ��� ��ư
    [SerializeField] private Image bgmIcon;//bgm ���� ǥ��
    [SerializeField] private Sprite iconSoundOn;//���� ���� ������
    [SerializeField] private Sprite iconSoundOff;//���� ����(���Ұ�) ������

    [Header("SFX UI")]
    [SerializeField] private Slider sfxSlider;//0~1
    [SerializeField] private Button sfxMuteButton;//sfx���Ұ� ��� ��ư
    [SerializeField] private Image sfxIcon;//sfx���¸� ǥ��

    void Start()
    {
        if (AudioManager.Instance == null) return;

        if (bgmSlider != null)//�����̴� �ʱⰪ ����ȭ
        {
            bgmSlider.SetValueWithoutNotify(AudioManager.Instance.GetBgmVolumeLinear());//���� BGM ������ �����̴��� �ݿ�
            bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);//�����̴� �� ���� �� �ݹ� ���
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSfxVolumeLinear());//���� SFX ������ �����̴��� �ݿ�
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);//�����̴��� ���� �� �ݹ� ���
        }
        if (bgmMuteButton != null)bgmMuteButton.onClick.AddListener(OnBgmMuteClicked);

        if (sfxMuteButton != null)sfxMuteButton.onClick.AddListener(OnSfxMuteClicked);

        UpdateBgmIcon();
        UpdateSfxIcon();
    }

    private void OnBgmSliderChanged(float v)//BGM �����̴� �� ���� �� ȣ��Ǵ� �ݹ� �޼���
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetBgmVolume(v);//SetBgmVolume()���� ����� bgm ���� ���� BGM ���� �����̴��� �ݿ�
        UpdateBgmIcon();
    }

    private void OnSfxSliderChanged(float v)//SFX �����̴� �� ���� �� ȣ��Ǵ� �ݹ� �޼���
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetSfxVolume(v);//SetSfxVolume()���� ����� sfx ���� ���� sfx �����̴��� �ݿ�
        UpdateSfxIcon();
    }

    private void OnBgmMuteClicked()//bgm ���Ұ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.ToggleBgmMute();
        UpdateBgmIcon();
    }

    private void OnSfxMuteClicked()// sfx ���Ұ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.ToggleSfxMute();
        UpdateSfxIcon();
    }
    private void UpdateBgmIcon()//���� bgm ���¿� ���� �������� �����ϴ� �޼���
    {
        if (AudioManager.Instance == null || bgmIcon == null) return;
        bool effectivelyMuted = AudioManager.Instance.IsBgmMuted() || AudioManager.Instance.GetBgmVolumeLinear() <= 0.0001f;//���Ұ� �̺�Ʈ �߻� Ȥ�� ���������� �ſ� ������ true
        bgmIcon.sprite = effectivelyMuted ? iconSoundOff : iconSoundOn;// effectivelyMuted�� true�̸� ������� ���������� �ٲٰ�, false�� �׳� ���� ���������� �д�.
    }

    private void UpdateSfxIcon()//���� sfx ���¿� ���� �������� �����ϴ� �޼���.
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
