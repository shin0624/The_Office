# THE OFFICE - 신입사원 살아남기 프로젝트

## 📋 프로젝트 개요

**THE OFFICE - 신입사원 살아남기**는 비주얼 노벨과 캐주얼 게임 요소를 결합한 모바일 게임입니다. Unity UGUI를 활용하여 간단하면서도 효과적인 사용자 경험을 제공하며, JSON 기반 데이터 관리로 확장성을 확보합니다.

## 📋 프로젝트 컨셉

**게임 컨셉**
 - 사회생활을 처음 시작한 신입사원이 되어 상사의 발언에 따라 답변을 선택하고, 사회력 점수를 올려 레벨업
 - 비주얼 노벨 장르에서 선택지에 따라 엔딩이 분기되는 형태를 차용하여, 상사의 말에 대한 답변으로 3개의 선택지가 주어짐. 선택지 중 하나를 택하면 호감도가 상승/유지/하락하며, 호감도가 100이 되면 사회력 점수 획득량이 2배가 됨
 - 사회력 점수는 호감도 상승/유지 시 증가되며, 하락 선택지를 고르면 내려감
 - 호감도 증가량이 사회력 점수 증가량보다 높고, 호감도가 100이 된 상태로 게임을 진행하면 사회력 점수 획득량이 2배인 상태로 진행되므로 쉽게 클리어 가능

## 인게임 스크린샷
### 현재 개발 중인 화면입니다.
<img width="509" height="903" alt="Image" src="https://github.com/user-attachments/assets/698f1a3f-7c48-483b-882a-9d71073ab220" />
<img width="469" height="861" alt="Image" src="https://github.com/user-attachments/assets/cba8fc7b-a50b-4d16-a284-b5a8bb5180ed" />
<img width="457" height="817" alt="Image" src="https://github.com/user-attachments/assets/565c0784-4a2a-424e-b4f2-68b72eb8f54c" />
<img width="458" height="815" alt="Image" src="https://github.com/user-attachments/assets/d6d26b17-084e-4609-9786-d1e1496edad9" />
<img width="460" height="817" alt="Image" src="https://github.com/user-attachments/assets/7f1490ed-cfab-474c-b3bc-3bc8cead9052" />

- **개발 기간**: 2025.07~
- **타겟 플랫폼**: Android (Google Play)
- **개발 엔진**: <img src="https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white" /> 2023.2.16f1 LTS
- **기술 스택**: <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" /> <img src="https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white" />  <img src="https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white" />
- **그래픽 리소스** : Sora AI / Leonardo AI
- **장르**: 캐주얼 80%, 비주얼 노벨 20%


## 🎮 게임 시스템

### 시스템 아키텍처
0. **사용자 입력에 따른 게임 진행 흐름도**
<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/232f51f3-24ee-4a51-a9ca-deeda1cc2dbc" />

1. **전체 데이터 플로우 및 핵심 데이터 순환 구조**
<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/4b2e6806-f06a-47f3-b3ae-8faabdf93fd0" />

2. **상세 데이터 플로우 아키텍처**
<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/e42104c6-bb00-479f-b1cd-dbcec4516272" />
<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/e3eb7989-6c98-442a-ae31-48be935b9846" />


### 핵심 스테이터스

- **사회력 점수**: 1~99999
- **직급**: 인턴 → 사원 → 주임 → 대리 → 과장 → 차장 → 부장 → 이사 → 상무 → 전무 → 부사장 → 사장 (12단계)
- **호감도**: 낮음 → 보통 → 높음 (Red → Blue → Green)

    
## 📅 개발 일정

### 1단계: 프로젝트 초기 설정 (2-3일)

- [ ] Unity 2022.3 LTS 프로젝트 생성
- [ ] Android 플랫폼 설정
- [ ] GitHub 저장소 설정
- [ ] 프로젝트 폴더 구조 설계


### 2단계: 기본 UI 구조 설계 (4-6일)

- [ ] Canvas 및 기본 UI 레이아웃 구성
- [ ] 호감도 Slider UI 구현
- [ ] 선택지 버튼 UI 구현
- [ ] Portrait 화면 분할 (2/3, 1/3)


### 3단계: 데이터 관리 시스템 (7일)

- [ ] JSON 데이터 구조 설계
- [ ] 게임 진행 데이터 저장/로드 시스템
- [ ] 대화 스크립트 관리 시스템
- [ ] 암호화 시스템 구현


### 4단계: 게임 로직 구현 (7-10일)

- [ ] 상사 선택 시스템 (3가지 타입)
- [ ] 대화 진행 시스템
- [ ] 호감도/사회력 점수 계산 로직
- [ ] 직급 승진 시스템


### 5단계: 컨텐츠 제작 (9-12일)

- [ ] 인트로 애니메이션 제작
- [ ] 캐릭터 이미지 및 애니메이션
- [ ] 대화 스크립트 작성 및 적용
- [ ] 굽신거리는 애니메이션 구현


### 6단계: 최적화 및 테스트 (7-10일)

- [ ] 메모리 최적화
- [ ] 모바일 성능 테스트
- [ ] 버그 수정 및 QA
- [ ] 사용자 경험 개선


### 7단계: 빌드 및 출시 준비 (4-5일)

- [ ] Android 빌드 설정 (API Level 34+)
- [ ] Google Play Console 설정
- [ ] 스토어 등록정보 작성


## 🛠️ 기술 스택

### 개발 환경

- **Unity**: 2023.2.16f1 LTS
- **IDE**: Visual Studio Code
- **버전 관리**: Git + GitHub
- **플랫폼**: Android


### 필수 패키지

- TextMeshPro
- Input System
- DOTween(3rd Party)
- JsonUtility (내장)
- Firebase 또는 The Backend 활용 방안 검토중


### 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Managers/
│   ├── Systems/
│   ├── UI/
│   └── Data/
├── Resources/
│   ├── Images/
│   ├── Audio/
│   └── Data/
├── Scenes/
├── Prefabs/
└── Materials/
```


## 💡 핵심 개발 포인트

### UI/UX 최적화

- **Portrait 화면 분할**: 2/3 메인 화면과 1/3 선택지 화면
- **호감도 시각화**: Slider 색상 변화 (빨강→파랑→초록)
- **터치 친화적 인터페이스**: 모바일 최적화된 버튼 크기


### 데이터 관리

- **JSON 기반 구조**: 확장성과 유지보수성 고려
- **암호화된 저장**: 게임 진행 상황 보안 강화
- **모듈화된 스크립트**: 상사별 독립적 컨텐츠 관리


### 성능 최적화

- **메모리 관리**: UI 요소 효율적 풀링
- **배칭 최적화**: Canvas 분리를 통한 렌더링 성능 향상
- **모바일 특화**: 배터리 소모 최소화


## 🎯 성공 기준

### 기능적 목표

- [ ] 모든 기획 기능의 정상 동작
- [ ] 3가지 상사 타입별 차별화된 게임플레이
- [ ] 안정적인 데이터 저장/로드 시스템


### 성능 목표

- [ ] 다양한 Android 기기에서 안정적 구동
- [ ] 60fps 유지 (최소 30fps)
- [ ] 메모리 사용량 최적화


### 사용자 경험

- [ ] 직관적이고 재미있는 게임플레이
- [ ] 빠른 로딩 시간 (3초 이내)
- [ ] 버그 없는 안정적인 플레이


### 출시 목표

- [ ] Google Play 성공적 등록 및 배포
- [ ] 스토어 정책 준수
- [ ] 사용자 리뷰 4.0+ 달성


## 📝 개발 노트

### 중요 구현 사항

1. **호감도 시스템**: FillAmount 기반 실시간 업데이트
2. **선택지 시스템**: 3가지 선택지는 각각 호감도 상승/유지/하락의 결과로 귀결
3. **승진 시스템**: 사회력 점수 기반 12단계 직급
4. **엔딩 분기**: 호감도 0 (Bad) / 100 (True)

**개발 시작일**: 2025년 7월 5일
**목표 출시일**: 2025년 8월 말 ~ 9월 초
