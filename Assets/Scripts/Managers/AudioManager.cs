using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    //게임 내 오디오 소스를 관리하는 매니저 스크립트.
    //게임 내 특정 씬 진입 시, 또는 점수 상승/하락 시 등의 상황에서 호출되는 모든 사운드 효과 재생 메서드를 정의한다.
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObj = GameObject.Find("Managers");
                if (managerObj == null)
                {
                    managerObj = new GameObject("Managers");
                    DontDestroyOnLoad(managerObj);
                }
                instance = managerObj.GetComponent<AudioManager>();
                if (instance == null)
                {
                    instance = managerObj.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;//오디오믹서 객체. BGM, SFX를 그룹으로 관리
    [SerializeField] private string bgmVolumeParam = "BGMVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    [Header("Audio Sources(Output : Mixer Groups)")]
    [SerializeField] private AudioSource bgmSource;//Output -> BGM 그룹
    [SerializeField] private AudioSource sfxSource;//Output -> SFX 그룹
    [SerializeField] private int sfxVoices = 4;//동시에 겹치는 SFX를 위한 보조 소스 수
    private List<AudioSource> sfxPool;

    //[Header("BGM Clips")]
    //[SerializeField] private List<BGMClipEntry> bgmClips;//인스펙터에서 등록




    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }


}
