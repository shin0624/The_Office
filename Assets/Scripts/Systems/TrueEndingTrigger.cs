using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EndingType { True, Good, Bad}//엔딩 타입 열거체 정의
public class TrueEndingTrigger : MonoBehaviour
{
    // True Ending으로 분기하는 트리거 스크립트.
    // True Ending 분기 조건 : 호감도 100 이하 Bad임계값 이상 유지 && 사장까지 승진
    // 엔딩 도달 시 True Ending 컷신 출력 -> 이후 컬렉션에 추가되고 엔딩 전까지 진행 정보는 초기화됨.
    // ScoreManager.cs의 호감도, 직급 체크 함수에서 각각 조건 만족 시 bool값이 넘어올 것이며, 두 함수 모두 true값이 넘어올 경우 True엔딩으로, 둘 중 하나만 True일 경우 Good, 둘 다 False일 경우 Bad로 진행.
    // 진화형 엔딩 구조
    // 	a. [호감도가 100 이하 && low 임계값 이상을 유지하며 && 플레이어가 사장까지 승진 ->  True엔딩] (True / True)
    // 	b. [상사의 호감도가 100이 된 상태 + 상사보다 직급이 낮은 상태 -> Good 엔딩(신뢰관계 형성 엔딩 등 사장이 되지는 못함)](True / False)
    // 	c. [상사의 호감도가 BAD 임계치 도달 + 상사보다 직급이 낮은 상태 -> Bad 엔딩(구조조정 OR 권고사직 등)](False / False)
    // 	d. [상사의 호감도가 BAD 임계치 도달 + 상사보다 직급이 높은 상태 -> Bad 엔딩(내부고발로 해고 등)](False / True)


    private bool affectionBranch = false;
    private bool rankBranch = false;
    private bool endingTriggered = false;

    void OnEnable()
    {
        var scoreManager = ScoreManager.Instance;
        if (scoreManager != null)
            scoreManager.OnEndingBranchChanged += OnEndingBranchCheck;
    }
    void OnDisable()
    {
        var scoreManager = ScoreManager.Instance;
        if (scoreManager != null)
            scoreManager.OnEndingBranchChanged -= OnEndingBranchCheck;
    }
    void Start()
    {
        Debug.Log($"endingTriggered = {endingTriggered}");
    }

    private void OnEndingBranchCheck(bool value, EndingBranchType which)//ScoreManager에서 넘어온 t/f값과 플래그에 따라 엔딩 분기를 결정하는 메서드.
    {
        if (which == EndingBranchType.Affection) affectionBranch = value;
        if (which == EndingBranchType.Rank) rankBranch = value;

        CheckEndingBranch();
    }

    private void CheckEndingBranch()//각 value 값들로부터 엔딩을 분기하는 메서드.
    {
        if (!ScoreManager.Instance.IsEndingBranchEnabled()) return;//플레이어 직급이 "차장" 미만일 때는 아예 분기하지 않도록 안전장치를 추가하여 게임 시작 직후 bad엔딩에 빠지는 예외를 방지.

        if (endingTriggered) return;//엔딩 트리거 false이면 분기X (엔딩 유효화 플래그는 ScoreManager 내부 분기에서 이미 필터링 됨.)

        if (affectionBranch && rankBranch)//True엔딩 (호감도 true && 직급 true)
        {
            endingTriggered = true;// 분기 결과 어떤 엔딩이라도 하나가 실행되면 바로 endingTringger는 true가 되고, 이후 추가 호출에서는 중복 엔딩 처리 x --> 이는 엔딩 이후 플래그 초기화 필요. 
            TriggerEnding(EndingType.True);
            Debug.Log("True 엔딩 진입");
        }
        else if (affectionBranch && !rankBranch)//Good엔딩 (호감도 true && 직급 false)
        {
            endingTriggered = true;
            TriggerEnding(EndingType.Good);
            Debug.Log("Good 엔딩 진입");
        }
        else if (!affectionBranch)//Bad엔딩 (호감도 false && 직급 false)
        {
            endingTriggered = true;
            TriggerEnding(EndingType.Bad, rankBranch);
            Debug.Log("BAD 엔딩 진입");
        }
    }

    private void TriggerEnding(EndingType type, bool rankBranchValue = false)//실제 분기메서드에 전해진 값에 따라 엔딩이 트리거되는 메서드.
    {
        if (endingTriggered) return;//중복 트리거 방지
        endingTriggered = true; // 엔딩 클리어 이후 다시 false로 변경해야 함

        switch (type)// 엔딩 별로 필요한 연출/초기화/콜렉션 저장 등 호출
        {
            case EndingType.True:
                Debug.Log("★ True Ending! 회사 최고 리더 등극 ★");
                // True 엔딩 컷신/이미지/업적/컬렉션 등 추가
                break;

            case EndingType.Good:
                Debug.Log("☆ Good Ending! 상사와 진한 신뢰관계 형성 ☆");
                // Good엔딩 연출, 등등
                break;

            case EndingType.Bad:
                if (rankBranchValue)
                    Debug.Log("Bad 엔딩(내부고발형 등) -- 직급은 높지만 해고");
                else
                    Debug.Log("Bad 엔딩(권고사직/퇴출) -- 평범하게 망함");
                // 각각 연출/메시지/업적 상이하게 연출
                break;
        }
        // ScoreManager 등에서 점수 리셋 등 초기화
        // 엔딩 후 MainScene/상사선택 등으로 이동 제어
    }

    public void ResetEndingTrigger()// 엔딩 분기 변수 및 트리거 초기화 메서드. 상태 초기화는 ScoreManager.cs의 CreateNewGame()을 사용.
    {
        affectionBranch = false;
        rankBranch = false;
        endingTriggered = false;
    }

    public void OnClickReplayOrNextBoss()//엔딩 이후 재도전 또는 다음 상사 선택 화면으로 넘어가는 등 엔딩 이후에 호출되는 초기화 메서드.
    {
        ScoreManager.Instance?.CreateNewGame();//모든 상태, 저장 데이터, UI 초기화 
        this.ResetEndingTrigger();//TrueEndingTrigger 내부 상태 리셋

        //이후 로직 작성 예정
    }
}
