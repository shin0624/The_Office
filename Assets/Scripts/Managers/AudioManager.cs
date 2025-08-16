using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    //���� �� ����� �ҽ��� �����ϴ� �Ŵ��� ��ũ��Ʈ.
    //���� �� Ư�� �� ���� ��, �Ǵ� ���� ���/�϶� �� ���� ��Ȳ���� ȣ��Ǵ� ��� ���� ȿ�� ��� �޼��带 �����Ѵ�.
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
    [SerializeField] private AudioMixer audioMixer;//������ͼ� ��ü. BGM, SFX�� �׷����� ����
    [SerializeField] private string bgmVolumeParam = "BGMVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    [Header("Audio Sources(Output : Mixer Groups)")]
    [SerializeField] private AudioSource bgmSource;//Output -> BGM �׷�
    [SerializeField] private AudioSource sfxSource;//Output -> SFX �׷�
    [SerializeField] private int sfxVoices = 4;//���ÿ� ��ġ�� SFX�� ���� ���� �ҽ� ��
    private List<AudioSource> sfxPool;

    //[Header("BGM Clips")]
    //[SerializeField] private List<BGMClipEntry> bgmClips;//�ν����Ϳ��� ���




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
