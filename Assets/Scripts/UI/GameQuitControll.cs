using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameQuitControll : MonoBehaviour
{
    //�ȵ���̵��� �ڷΰ��� ��ư Ŭ�� �� �̺�Ʈ ��� �����ϴ� Ŭ����. �ڷΰ���� � Scene������ ȣ��� �� �����Ƿ�, static���� ����.

    [SerializeField] private GameObject quitPanelPrefab;//���� ���� �г� ������ 
    [SerializeField] private Transform quitPanelParent;
    private float backButtonDoubleClickTime = 0.5f;//�ڷΰ��� ��ư ����Ŭ�� �ν� �ð�
    private float lastBackButtonClickedTime = 0.0f;
    private GameObject currentQuitPanel;
    private static GameQuitControll instance;
    public static GameQuitControll Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerobj = GameObject.Find("Managers");
                if (managerobj == null)
                {
                    managerobj = new GameObject("Managers");//Managers ������Ʈ�� ������ ����
                    DontDestroyOnLoad(managerobj);//�� ��ȯ �ÿ��� �ı����� �ʵ��� ����
                }
                instance = managerobj.GetComponent<GameQuitControll>();//Managers ������Ʈ���� GameQuitControll ������Ʈ�� ã��
                if (instance == null)
                {
                    instance = managerobj.AddComponent<GameQuitControll>();//������Ʈ�� ������ �߰�
                }
            }
            return instance;
        }
    }

    void Update()
    {
        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     if (Input.GetKeyDown(KeyCode.Escape))
        //     {
        //         HandleAndroidBackButton();
        //     }
        // }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleAndroidBackButton();
            }
        
    }

    private void HandleAndroidBackButton()//�ڷΰ��� ��ư Ŭ�� Ƚ���� ���� ���� UI �̺�Ʈ�� �ٸ��� �����ϴ� �޼���. 
    {
        float currentTime = Time.time;
        float timeSinceLastClick = currentTime - lastBackButtonClickedTime;//������ �ڷΰ��� ��ư Ŭ�� �� ���� �ҿ�� �ð�
        if (timeSinceLastClick <= backButtonDoubleClickTime)//0.5 �� �̳��� �ڷΰ��� �ι� Ŭ�� ��
        {
            if (currentQuitPanel != null)//�ڷΰ��� ����Ŭ�� ���� -> ���� �г� ���� �� ���� �簳
            {
                Destroy(currentQuitPanel);
                currentQuitPanel = null;
            }
        }
        else//ù ��° �ڷΰ��� Ŭ�� ��
        {
            if (currentQuitPanel == null)
            {
                currentQuitPanel = Instantiate(quitPanelPrefab, quitPanelParent);
            }
        }
        lastBackButtonClickedTime = currentTime;
    }

    public void ClearCurrentPanel()// QuitPanel���� ȣ��Ǿ� currentQuitPanel�� null�� �����ϴ� �޼���.
    {
        Debug.Log("[GameQuitControll] QuitPanel ���� ����");
        currentQuitPanel = null;
    }

    public bool IsQuitPanelActive()// ���� QuitPanel�� Ȱ��ȭ�Ǿ� �ִ��� Ȯ���ϴ� �޼���.
    {
        return currentQuitPanel != null;
    }
}
