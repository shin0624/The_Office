using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueEndingTrigger : MonoBehaviour
{
    //���� �ڽ��� ������ ��纸�� �������� True Ending���� �б��ϴ� Ʈ���� ��ũ��Ʈ.
    void Start()
    {
        ScoreManager.Instance.OnTrueEnding += EnterTrueEnding;
    }

    void Update()
    {

    }

    private void EnterTrueEnding()
    {
        
    }
}
