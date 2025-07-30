using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueEndingTrigger : MonoBehaviour
{
    //현재 자신의 직급이 상사보다 높아지면 True Ending으로 분기하는 트리거 스크립트.
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
