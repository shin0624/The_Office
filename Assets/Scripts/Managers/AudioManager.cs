using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static DataStructures;
using static AudioEnums;
using UnityEditor;

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
                GameObject managerObj = GameObject.Find("AudioManagers");
                if (managerObj == null)
                {
                    managerObj = new GameObject("AudioManagers");
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

    [Header("Audio Mixer")]//����� �ͼ� ���� ����
    [SerializeField] private AudioMixer audioMixer;//������ͼ� ��ü  ����
    [SerializeField] private string bgmVolumeParam = "BGMVolume";//����� �ͼ������� BGM �׷� ���� �Ķ���� �̸�.Expose�̸��� ��ġ�ؾ� ��
    [SerializeField] private string sfxVolumeParam = "SFXVolume";//����� �ͼ����� SFX �׷� ���� �Ķ���� �̸�. Expose�̸��� ��ġ�ؾ� ��

    [Header("Audio Sources(Output : Mixer Groups)")]//����� �ҽ� ���� ����
    [SerializeField] private AudioSource bgmSource;//bgm ��� ���� �ҽ�
    [SerializeField] private AudioSource sfxSource;//SFX ��� ���� �ҽ�
    [SerializeField] private int sfxVoices = 4;//���ÿ� ���� SFX�� ���� ����ϱ� ���� ���� ����� �ҽ� ����
    private List<AudioSource> sfxPool;//���� ����� ���� SFX ����� �ҽ� Ǯ

    [Header("BGM Clips")]
    [SerializeField] private List<BGMClipEntry> bgmClips;//BGMŸ�԰� Ŭ���� ���� ����Ʈ
    [Header("SFX Clips")]
    [SerializeField] private List<SFXClipEntry> sfxClips;//SFX Ÿ�԰� Ŭ���� ���� ����Ʈ

    private Dictionary<BGMType, AudioClip> bgmDict = new();//�� Ÿ�� �� bgmŬ���� ��ųʸ��� ����.
    private Dictionary<SFXType, AudioClip> sfxDict = new();// ���ͷ��� Ÿ�� �� sfx Ŭ���� ��ųʸ��� ����.

    private Coroutine bgmFadeCoroutine;// bgm ���̵���/�ƿ� �ڷ�ƾ �ڵ�(�ߺ� ���� ������)
    private float lastBgmLinear = 0.8f;//���������� ������ BGM ���� ����(0~1)
    private float lastSfxLinear = 0.8f;//���������� ������ SFX ���� ����(0~1)
    private bool isBgmMuted = false;//BGM ���Ұ� ���� �÷���
    private bool isSfxMuted = false;//SFX ���Ұ� ���� �÷���

    public bool IsBgmMuted() => isBgmMuted;//bgm���Ұ� ���θ� ��ȯ�ϴ� ���� ǥ���� �޼���
    public bool IsSfxMuted() => isSfxMuted;//sfx���Ұ� ���θ� ��ȯ�ϴ� ���� ǥ���� �޼���
    public float GetBgmVolumeLinear() => lastBgmLinear;// ������ bgm ���� ������ ��ȯ
    public float GetSfxVolumeLinear() => lastSfxLinear;// ������ sfx ���� ������ ��ȯ

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
            return;
        }

        EnsureBgmSource();//����� �ҽ� ��ȿ�� ����(������ �����ϵ���)

        BuildDictonaries();//�ν����Ϳ��� ����� ����Ʈ�� ��ųʸ��� ����
        PrepareSfxVoices();//sfx��������� ���� ���� ����� �ҽ� �غ�

        SceneManager.sceneLoaded += OnSceneLoaded;//�� �ε� �̺�Ʈ�� �ڵ鷯 ���� -> �� ��ȯ �� �ڵ� bgm ��ȯ ó��

        SetBgmVolume(lastBgmLinear);//�ʱ� bgm ������ �ͼ��� �ݿ�
        SetSfxVolume(lastSfxLinear);//�ʱ� sfx ������ �ͼ��� �ݿ�
    }

    private void BuildDictonaries()// ����Ʈ�� ��ųʸ��� �����ϴ� �޼���.
    {
        bgmDict.Clear();
        foreach (var element in bgmClips)//��ϵ� bgm ����Ʈ ��ȸ
        {
            if (element.clip != null) bgmDict[element.type] = element.clip;//bgmdict�� Ÿ���� Ű�� �Ͽ� �ش��ϴ� Ŭ���� ã�´�.
        }

        sfxDict.Clear();
        foreach (var element in sfxClips)//��ϵ� sfx����Ʈ ��ȸ
        {
            if (element.clip != null) sfxDict[element.type] = element.clip;//sfxDict�� Ÿ���� Ű�� �Ͽ� �ش��ϴ� Ŭ���� ã�´�.
        }
    }

    private void PrepareSfxVoices()//SFX ���� ����� �ҽ��� �غ��ϴ� �޼���.
    {
        sfxPool = new List<AudioSource>();
        if (sfxSource != null) sfxPool.Add(sfxSource);//�⺻ SFX �ҽ��� Ǯ�� ù �׸����� �߰�
        for (int i = 0; i < sfxVoices; i++)//������ ���� �ҽ� ������ŭ �߰� ����(�⺻ �ҽ� ����)
        {
            var go = new GameObject($"SFXVoice_{i}");//���� �ҽ��� ���� ������Ʈ ����
            go.transform.SetParent(transform);//SoundManagers�� �ڽ����� ��ġ�Ͽ� �Բ� ����/�ı��ǵ��� ��.
            var src = go.AddComponent<AudioSource>();//����� �ҽ� ������Ʈ �߰�

            if (sfxSource != null) src.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;//�⺻ sfx �ҽ� ���� �� ������ �ͼ� �׷쿡 �����
            src.playOnAwake = false;//�ڵ���� ����
            sfxPool.Add(src);//Ǯ�� ���� �ҽ� �߰�
        }  
    }

    private bool TryMapSceneToBgm(string sceneName, out BGMType type)// �� �̸��� BGMType���� �����ϴ� �޼���
    {
        switch (sceneName)
        {
            case "StartScene":
                type = BGMType.StartScene;
                return true;

            case "SelectScene":
                type = BGMType.SelectScene;
                return true;

            case "MainScene":
                type = BGMType.MainScene;
                return true;

            case "CollectionScene":
                type = BGMType.CollectionScene;
                return true;

            default:
                type = BGMType.StartScene;
                return false; 
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)//���� ����� �� BGM�� ���� �����Ű���� OnSceneLoaded()�� �� ���� �̺�Ʈ�� �߰�
    {

        EnsureBgmSource();//�� ��ȯ ���� �ҽ� ��ȿ�� ����
        if (TryMapSceneToBgm(scene.name, out var bgmType))//�ε�� ���� �̸��� BGMType���� ����
        {
            PlayBGM(bgmType, fadeTime: 0.7f);
        }
    }

    //---------------BGM �޼���

    public void PlayBGM(BGMType type, float fadeTime = 0.0f)//BGM ��� �޼���
    {
        if (!bgmDict.TryGetValue(type, out var clip) || clip == null) return;//���� ã�� �� ���ų� Ŭ���� null�̸� ����

        if (bgmFadeCoroutine != null)//���̵� �ڷ�ƾ�� ���� ���� ���� ���, ��ž.
        {
            StopCoroutine(bgmFadeCoroutine);
            bgmFadeCoroutine = null;
        }

        EnsureBgmSource(); //���� ���� ��ȿ�� ����.

        if (fadeTime > 0.0f && bgmSource.isPlaying)//BGM ���̵��� ���� 
        {
            bgmFadeCoroutine = StartCoroutine(FadeBGM(clip, fadeTime));
        }
        else
        {
            bgmSource.clip = clip;//�ҽ��� �� Ŭ�� �Ҵ�
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM(float fadeOut = 0.0f)//BGM ���� �޼���
    {
        if (!bgmSource.isPlaying) return;//���� ���� �ƴϸ� ����
        if (fadeOut > 0.0f)
        {
            StartCoroutine(FadeOutBGM(fadeOut));//���̵�ƿ� ����
        }
        else
        {
            bgmSource.Stop();
            bgmSource.clip = null;//���۷��� ����
        }
    }

    private IEnumerator FadeBGM(AudioClip next, float time)//bgm�� ���̵� �ƿ� �� �� Ŭ������ ���̵��� �ϴ� �޼���.
    {
        float startVol = bgmSource.volume;//���� ������ ���� ������ ����
        float t = 0.0f;//�ð� ���� ���� t
        while (t < time)//���̵� �ð� ���� �ݺ�
        {
            t += Time.unscaledDeltaTime;//�ð� ��� ����. Ÿ�ӽ����Ͽ� ������ ���� �ʵ��� unscaledDeltaTime�� ���
            bgmSource.volume = Mathf.Lerp(startVol, 0.0f, t / time);//0���� ���� �������� ������ ����
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = next;//�� Ŭ������ ��ü
        bgmSource.loop = true;
        bgmSource.Play();

        t = 0.0f;//�ð� �ʱ�ȭ
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0.0f, 1.0f, t / time);//1���� ���� �������� ������ �ø���.
            yield return null;
        }
        bgmSource.volume = 1.0f;//�������� ������ ��Ȯ�� 1�� ����
        bgmFadeCoroutine = null;//�ڷ�ƾ �ڵ� ���۷��� ����
    }

    private IEnumerator FadeOutBGM(float time)//bgm ���̵�ƿ� �� �����ϴ� �޼���.
    {
        float startVol = bgmSource.volume;//���� ���� ����
        float t = 0f;//�ð� ���� ���� t
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / time);//0���� ���� �������� ����
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = null;
        bgmSource.volume = 1f;//���� ����� ���� ������ 1�� ����
    }


    //-------------SFX �޼���
    public void PlaySFX(SFXType type, float volumeScale = 1.0f, float pitch = 1.0f)//SFX�� ����ϴ� �޼���.
    {
        if (!sfxDict.TryGetValue(type, out var clip) || clip == null) return;

        AudioSource voice = GetIdleSfxVoice();//���� �ִ� ���� �ҽ��� �����´�.
        if (voice == null) voice = sfxPool[0];//��� �ٻڸ� ù ��° �ҽ��� ���

        voice.pitch = Mathf.Clamp(pitch, 0.5f, 1.5f);//��ġ�� ���� ������ �����Ͽ� ����
        voice.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    private AudioSource GetIdleSfxVoice()//�����ҽ� �� ���� �ִ� �ҽ��� ��ȯ�ϴ� �޼���.
    {
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying) return src;
        }
        return null;
    }

    //----------Volume, Mute �޼���

    public void SetBgmVolume(float linear)//BGM ���� ���� �޼���(0.0 ~ 1.0)
    {
        lastBgmLinear = Mathf.Clamp01(linear);//�Է°��� 0~1�� �����Ͽ� ����
        float dB = LinearToDB(isBgmMuted ? 0f : lastBgmLinear);//���Ұ� ���¸� 0���� �����ϰ�, �� �ܴ� ���ú��� ��ȯ
        audioMixer.SetFloat(bgmVolumeParam, dB);//������ͼ� �Ķ���Ϳ� ���ú� ����
    }

    public void SetSfxVolume(float linear) // SFX ���� ���� �޼���.(0.0 ~ 1.0)
    {
        lastSfxLinear = Mathf.Clamp01(linear);//�Է°��� 0~1�� �����Ͽ� ����
        float dB = LinearToDB(isSfxMuted ? 0f : lastSfxLinear);//���Ұ� ���¸� 0���� �����ϰ�, �� �ܴ� �������� ���ú��� ��ȯ
        audioMixer.SetFloat(sfxVolumeParam, dB);//����� �ͼ� �Ķ���Ϳ� ���ú� �� ����
    }

    public void ToggleBgmMute()//bgm���Ұ� ���� ��� �޼���
    {
        isBgmMuted = !isBgmMuted;//���Ұ� �÷��� ����
        SetBgmVolume(lastBgmLinear);//���� ���� ���� ���� �ٽ� �����Ͽ� �ͼ��� �ݿ�.
    }

    public void ToggleSfxMute()//sfx���Ұ� ���¸� ����ϴ� �޼���
    {
        isSfxMuted = !isSfxMuted;//���Ұ� �÷��� ���� ����
        SetSfxVolume(lastSfxLinear);//���� ���� ���� ���� �ٽ� �����Ͽ� �ͼ��� �ݿ�
    }

    private float LinearToDB(float linear)//���� ����(0~1)�� ���ú� ������ ��ȯ�ϴ� �޼���.
    {
        if (linear <= 0.0001f) return -80.0f;//���� 0�� ������ -80(��ǻ� ���Ұ�)�� ó��.
        return Mathf.Log10(linear) * 20.0f;//���ú� ǥ�� ��ȯ�� (DB = 20 * log10(linear)) ����.
    }

    private void EnsureBgmSource()//�� ��ȯ �� BGMSource ���� �������� NRE �߻��� �Ͼ�� Ȯ���Ͽ�, BGMSource ���� ������ ���� ����
    {
        if (bgmSource == null || !bgmSource)
        {
            var go = new GameObject("BGMSource");
            go.transform.SetParent(transform);
            bgmSource = go.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.outputAudioMixerGroup = audioMixer.outputAudioMixerGroup;
        }
    }



    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }


}
    