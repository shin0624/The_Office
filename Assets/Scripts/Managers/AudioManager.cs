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
    //게임 내 오디오 소스를 관리하는 매니저 스크립트.
    //게임 내 특정 씬 진입 시, 또는 점수 상승/하락 시 등의 상황에서 호출되는 모든 사운드 효과 재생 메서드를 정의한다.
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

    [Header("Audio Mixer")]//오디오 믹서 관련 설정
    [SerializeField] private AudioMixer audioMixer;//오디오믹서 객체  참조
    [SerializeField] private string bgmVolumeParam = "BGMVolume";//오디오 믹서에서의 BGM 그룹 볼륨 파라미터 이름.Expose이름과 일치해야 함
    [SerializeField] private string sfxVolumeParam = "SFXVolume";//오디오 믹서에서 SFX 그룹 볼륨 파라미터 이름. Expose이름과 일치해야 함

    [Header("Audio Sources(Output : Mixer Groups)")]//오디오 소스 설정 섹션
    [SerializeField] private AudioSource bgmSource;//bgm 재생 전용 소스
    [SerializeField] private AudioSource sfxSource;//SFX 재생 전용 소스
    [SerializeField] private int sfxVoices = 4;//동시에 여러 SFX를 겹쳐 재생하기 위한 보조 오디오 소스 개수
    private List<AudioSource> sfxPool;//동시 재생을 위한 SFX 오디오 소스 풀

    [Header("BGM Clips")]
    [SerializeField] private List<BGMClipEntry> bgmClips;//BGM타입과 클립을 묶은 리스트
    [Header("SFX Clips")]
    [SerializeField] private List<SFXClipEntry> sfxClips;//SFX 타입과 클립을 묶은 리스트

    private Dictionary<BGMType, AudioClip> bgmDict = new();//씬 타입 별 bgm클립을 딕셔너리로 관리.
    private Dictionary<SFXType, AudioClip> sfxDict = new();// 인터랙션 타입 별 sfx 클립을 딕셔너리로 관리.

    private Coroutine bgmFadeCoroutine;// bgm 페이드인/아웃 코루틴 핸들(중복 실행 방지용)
    private float lastBgmLinear = 0.8f;//마지막으로 설정된 BGM 선형 볼륨(0~1)
    private float lastSfxLinear = 0.8f;//마지막으로 설정된 SFX 선형 볼륨(0~1)
    private bool isBgmMuted = false;//BGM 음소거 상태 플래그
    private bool isSfxMuted = false;//SFX 음소거 상태 플래그

    public bool IsBgmMuted() => isBgmMuted;//bgm음소거 여부를 반환하는 단일 표현식 메서드
    public bool IsSfxMuted() => isSfxMuted;//sfx음소거 여부를 반환하는 단일 표현식 메서드
    public float GetBgmVolumeLinear() => lastBgmLinear;// 마지막 bgm 선형 볼륨값 반환
    public float GetSfxVolumeLinear() => lastSfxLinear;// 마지막 sfx 선형 볼륨값 반환

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

        EnsureBgmSource();//오디오 소스 유효성 보장(없으면 생성하도록)

        BuildDictonaries();//인스펙터에서 등록한 리스트를 딕셔너리로 구성
        PrepareSfxVoices();//sfx동시재생을 위한 보조 오디오 소스 준비

        SceneManager.sceneLoaded += OnSceneLoaded;//씬 로드 이벤트에 핸들러 구독 -> 씬 전환 시 자동 bgm 전환 처리

        SetBgmVolume(lastBgmLinear);//초기 bgm 볼륨을 믹서에 반영
        SetSfxVolume(lastSfxLinear);//초기 sfx 볼륨을 믹서에 반영
    }

    private void BuildDictonaries()// 리스트를 딕셔너리로 빌드하는 메서드.
    {
        bgmDict.Clear();
        foreach (var element in bgmClips)//등록된 bgm 리스트 순회
        {
            if (element.clip != null) bgmDict[element.type] = element.clip;//bgmdict의 타입을 키로 하여 해당하는 클립을 찾는다.
        }

        sfxDict.Clear();
        foreach (var element in sfxClips)//등록된 sfx리스트 순회
        {
            if (element.clip != null) sfxDict[element.type] = element.clip;//sfxDict의 타입을 키로 하여 해당하는 클립을 찾는다.
        }
    }

    private void PrepareSfxVoices()//SFX 보조 오디오 소스를 준비하는 메서드.
    {
        sfxPool = new List<AudioSource>();
        if (sfxSource != null) sfxPool.Add(sfxSource);//기본 SFX 소스를 풀의 첫 항목으로 추가
        for (int i = 0; i < sfxVoices; i++)//지정된 보조 소스 개수만큼 추가 생성(기본 소스 제외)
        {
            var go = new GameObject($"SFXVoice_{i}");//보조 소스를 담을 오브젝트 생성
            go.transform.SetParent(transform);//SoundManagers의 자식으로 배치하여 함께 생존/파괴되도록 함.
            var src = go.AddComponent<AudioSource>();//오디오 소스 컴포넌트 추가

            if (sfxSource != null) src.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;//기본 sfx 소스 존재 시 동일한 믹서 그룹에 라우팅
            src.playOnAwake = false;//자동재생 방지
            sfxPool.Add(src);//풀에 보조 소스 추가
        }  
    }

    private bool TryMapSceneToBgm(string sceneName, out BGMType type)// 씬 이름을 BGMType으로 매핑하는 메서드
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)//씬이 변경될 때 BGM도 같이 변경시키도록 OnSceneLoaded()를 씬 변경 이벤트에 추가
    {

        EnsureBgmSource();//씬 전환 직후 소스 유효성 보장
        if (TryMapSceneToBgm(scene.name, out var bgmType))//로드될 씬의 이름을 BGMType으로 매핑
        {
            PlayBGM(bgmType, fadeTime: 0.7f);
        }
    }

    //---------------BGM 메서드

    public void PlayBGM(BGMType type, float fadeTime = 0.0f)//BGM 재생 메서드
    {
        if (!bgmDict.TryGetValue(type, out var clip) || clip == null) return;//값을 찾을 수 없거나 클립이 null이면 종료

        if (bgmFadeCoroutine != null)//페이드 코루틴이 먼저 진행 중일 경우, 스탑.
        {
            StopCoroutine(bgmFadeCoroutine);
            bgmFadeCoroutine = null;
        }

        EnsureBgmSource(); //접근 전에 유효성 보장.

        if (fadeTime > 0.0f && bgmSource.isPlaying)//BGM 페이드인 시작 
        {
            bgmFadeCoroutine = StartCoroutine(FadeBGM(clip, fadeTime));
        }
        else
        {
            bgmSource.clip = clip;//소스에 새 클립 할당
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM(float fadeOut = 0.0f)//BGM 중지 메서드
    {
        if (!bgmSource.isPlaying) return;//실행 중이 아니면 리턴
        if (fadeOut > 0.0f)
        {
            StartCoroutine(FadeOutBGM(fadeOut));//페이드아웃 시작
        }
        else
        {
            bgmSource.Stop();
            bgmSource.clip = null;//레퍼런스 제거
        }
    }

    private IEnumerator FadeBGM(AudioClip next, float time)//bgm을 페이드 아웃 후 새 클립으로 페이드인 하는 메서드.
    {
        float startVol = bgmSource.volume;//현재 볼륨을 시작 값으로 저장
        float t = 0.0f;//시간 누적 변수 t
        while (t < time)//페이드 시간 동안 반복
        {
            t += Time.unscaledDeltaTime;//시간 경과 누적. 타임스케일에 영향을 받지 않도록 unscaledDeltaTime을 사용
            bgmSource.volume = Mathf.Lerp(startVol, 0.0f, t / time);//0까지 선형 보간으로 볼륨을 낮춤
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = next;//새 클립으로 교체
        bgmSource.loop = true;
        bgmSource.Play();

        t = 0.0f;//시간 초기화
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0.0f, 1.0f, t / time);//1까지 선형 보간으로 볼륨을 올린다.
            yield return null;
        }
        bgmSource.volume = 1.0f;//마지막에 볼륨을 정확히 1로 맞춤
        bgmFadeCoroutine = null;//코루틴 핸들 레퍼런스 제거
    }

    private IEnumerator FadeOutBGM(float time)//bgm 페이드아웃 후 정지하는 메서드.
    {
        float startVol = bgmSource.volume;//시작 볼륨 저장
        float t = 0f;//시간 누적 변수 t
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / time);//0까지 선형 보간으로 낮춤
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = null;
        bgmSource.volume = 1f;//다음 재생을 위해 볼륨을 1로 복원
    }


    //-------------SFX 메서드
    public void PlaySFX(SFXType type, float volumeScale = 1.0f, float pitch = 1.0f)//SFX를 재생하는 메서드.
    {
        if (!sfxDict.TryGetValue(type, out var clip) || clip == null) return;

        AudioSource voice = GetIdleSfxVoice();//쉬고 있는 보조 소스를 가져온다.
        if (voice == null) voice = sfxPool[0];//모두 바쁘면 첫 번째 소스를 사용

        voice.pitch = Mathf.Clamp(pitch, 0.5f, 1.5f);//피치를 안전 범위로 제한하여 설정
        voice.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    private AudioSource GetIdleSfxVoice()//보조소스 중 쉬고 있는 소스를 반환하는 메서드.
    {
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying) return src;
        }
        return null;
    }

    //----------Volume, Mute 메서드

    public void SetBgmVolume(float linear)//BGM 볼륨 설정 메서드(0.0 ~ 1.0)
    {
        lastBgmLinear = Mathf.Clamp01(linear);//입력값을 0~1로 보정하여 저장
        float dB = LinearToDB(isBgmMuted ? 0f : lastBgmLinear);//음소거 상태면 0으로 강제하고, 그 외는 데시벨로 변환
        audioMixer.SetFloat(bgmVolumeParam, dB);//오디오믹서 파라미터에 데시벨 적용
    }

    public void SetSfxVolume(float linear) // SFX 볼륨 설정 메서드.(0.0 ~ 1.0)
    {
        lastSfxLinear = Mathf.Clamp01(linear);//입력값을 0~1로 보정하여 저장
        float dB = LinearToDB(isSfxMuted ? 0f : lastSfxLinear);//음소거 상태면 0으로 강제하고, 그 외는 선형값을 데시벨로 변환
        audioMixer.SetFloat(sfxVolumeParam, dB);//오디오 믹서 파라미터에 데시벨 값 적용
    }

    public void ToggleBgmMute()//bgm음소거 상태 토글 메서드
    {
        isBgmMuted = !isBgmMuted;//음소거 플래그 반전
        SetBgmVolume(lastBgmLinear);//현재 선형 볼륨 값을 다시 적용하여 믹서에 반영.
    }

    public void ToggleSfxMute()//sfx음소거 상태를 토글하는 메서드
    {
        isSfxMuted = !isSfxMuted;//음소거 플래그 상태 반전
        SetSfxVolume(lastSfxLinear);//현재 선형 볼륨 값을 다시 적용하여 믹서에 반영
    }

    private float LinearToDB(float linear)//선형 볼륨(0~1)을 데시벨 값으로 변환하는 메서드.
    {
        if (linear <= 0.0001f) return -80.0f;//거의 0에 가까우면 -80(사실상 음소거)로 처리.
        return Mathf.Log10(linear) * 20.0f;//데시벨 표준 변환식 (DB = 20 * log10(linear)) 적용.
    }

    private void EnsureBgmSource()//씬 전환 시 BGMSource 접근 과정에서 NRE 발생이 일어남을 확인하여, BGMSource 유지 보장을 위해 선언
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
    