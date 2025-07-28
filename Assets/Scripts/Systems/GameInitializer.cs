using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour
{
    //���� ���� �� �ʿ��� ��� �Ŵ������� �ʱ�ȭ�ϰ� ������ �����ϴ� ��ũ��Ʈ.
    [Header("�ʱ�ȭ ����")]
    [SerializeField] private bool showDebugLogs = true; //����� �α� ǥ�� ����

    [Header("����� ����")]
    [SerializeField] private KeyCode clearPlayerPrefsDataKey = KeyCode.Delete;//�����Ϳ��� �÷��̾� ������ �����Ͱ� ��� ���̸� �׽�Ʈ�� �Ұ����ϱ⿡, �����Ϳ� ����Ʈ�ɼ� �߰�.

    private GameObject managersObject; //Managers ������Ʈ ����

    void Start()
    {
        InitializeGame();//���� �ʱ�ȭ �޼��� ȣ��
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(clearPlayerPrefsDataKey))
        {
            ClearAllData();
            Debug.Log("[GameInitializer] ����� : ��� ���� ������ �ʱ�ȭ �Ϸ�");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);//���� ���� �� ��ε� �� ��� �ʱ�ȭ Ʈ����� �ݿ�.
        }   
 #endif
    }

    [ContextMenu("PlayerPrefs �ʱ�ȭ")]
    private void ClearAllData()
    {
        PlayerPrefs.DeleteAll();//�÷��̾������� ���� ��� ����.
        PlayerPrefs.Save();
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeleteSaveData();//Save_Data.json �� ������� ������ ��� ����
        }
        Debug.Log("[GameInitializer]��� ������ �ʱ�ȭ �Ϸ�");
    }

    private void InitializeGame()
    {
        if (LoadingUI.Instance != null && LoadingUI.Instance.IsLoading)
        {
            Debug.Log("[GameInitializer] �ε� UI�� �Բ� �ʱ�ȭ ����");
        }

        if (showDebugLogs)
                Debug.Log("[GameInitializer] ���� �ʱ�ȭ ����");

        CreateOrFindManagers();//Managers ������Ʈ ���� �Ǵ� ã��

        //�� �Ŵ��� ���� ����(���� �� �ڵ� ������.)
        var saveLoadManager = SaveLoadManager.Instance; //����/�ε� �Ŵ��� �ν��Ͻ� ��������
        var scoreManager = ScoreManager.Instance; //���� �Ŵ��� �ν��Ͻ� ��������
        if (showDebugLogs)
        {
            Debug.Log("[GameInitializer] SaveLoadManager �ʱ�ȭ �Ϸ�");
            Debug.Log("[GameInitializer] ScoreManager �ʱ�ȭ �Ϸ�");
        }
        InitializeUI();
        StartCoroutine(InitializeAudioAfterDelay());//����� �Ŵ��� �ʱ�ȭ�� ������ �� ����
        if (showDebugLogs)
            Debug.Log("[GameInitializer] ���� �ʱ�ȭ �Ϸ�");
    }

    private void CreateOrFindManagers()//Managers ������Ʈ ���� �Ǵ� ã�� �޼���.
    {
        managersObject = GameObject.Find("Managers");
        if (managersObject == null)
        {
            managersObject = new GameObject("Managers");
            DontDestroyOnLoad(managersObject);
            
            if (showDebugLogs)
                Debug.Log("[GameInitializer] Managers ������Ʈ ����");
        }
        else
        {
            if (showDebugLogs)
                Debug.Log("[GameInitializer] ���� Managers ������Ʈ �߰�");
        }
    }

    private void InitializeUI()//UI �Ŵ��� �ʱ�ȭ �޼���
    {
        
        var uiManager = FindAnyObjectByType<UIManager>();// UI �Ŵ����� �ִٸ� �ʱ�ȭ
        if (uiManager != null)
        {
            if (showDebugLogs)
                Debug.Log("[GameInitializer] UI �Ŵ��� �ʱ�ȭ");
        }
    }

    private IEnumerator InitializeAudioAfterDelay()//����� �Ŵ��� �ʱ�ȭ �޼���
    {
        yield return new WaitForSeconds(1f);// ScoreManager�� ������ �ʱ�ȭ�� ������ ���
        
        var saveData = ScoreManager.Instance.GetCurrentSaveData();
        if (saveData != null)
        {
            AudioListener.volume = saveData.game_settings.sound_volume;
            if (showDebugLogs)
                Debug.Log($"[GameInitializer] ����� ���� ����: {saveData.game_settings.sound_volume}");
        }
    }

}
