# BattleSystem — Mini Project 02

> 캐릭터 선택 시스템 + 상태 머신 기반 전투 프로토타입
> Unity 6.2 / Core Dive Studio

---

## 목표

BattlePrototype에서 구축한 전투 시스템을 기반으로,
C# 핵심 개념을 실전 구조에 직접 녹여내는 것을 목표로 했다.

1. **캐릭터 선택 시스템** — 생성자로 캐릭터를 초기화하고 out으로 스탯을 읽어 UI에 표시
2. **상태 머신 기반 전투** — enum + switch로 전투 흐름을 구조화
3. **포션 시스템** — ref로 포션 수량과 HP를 동시에 처리
4. **씬 간 데이터 전달** — 선택한 캐릭터를 MainScene으로 넘기는 구조

---

## 구현 내용

### 캐릭터 선택 씬

캐릭터 3종(Warrior, Mage, Healer)을 생성자로 초기화한다.
버튼 클릭 시 `GetStatus(out int hp, out int atk, out int def)`로
스탯을 읽어 StatCard UI에 표시한다.
확정 버튼을 누르면 `GameDataManager.selectedUnit`에 데이터를 저장하고
MainScene으로 전환된다.

선택한 캐릭터의 스프라이트는 `CharacterType` enum으로 인덱스를 넘기고,
MainScene에서 switch 표현식으로 매핑하는 방식을 택했다.
스프라이트 자체를 넘기지 않고 타입만 넘김으로써
데이터와 에셋을 분리하는 구조를 유지했다.

### 상태 머신 기반 전투

`BattleState` enum으로 전투 흐름을 명시적으로 구조화했다.
`PlayerTurn` 상태일 때만 Attack/Potion 버튼이 활성화되며,
버튼 입력에 따라 상태가 전환된다.
이전 BattlePrototype의 자동 진행 구조에서 벗어나
플레이어 입력 대기 구조로 전환한 것이 핵심 변화다.

```csharp
enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose }

switch (currentState)
{
    case BattleState.PlayerTurn:
        uiManager.SetButtonsInteractable(true);
        break;
    case BattleState.EnemyTurn:
        uiManager.SetButtonsInteractable(false);
        StartCoroutine(EnemyTurn());
        break;
}
```

### 포션 시스템

`ref`로 포션 수량과 HP를 단일 메서드에서 동시에 처리한다.
포션 타입은 `switch 표현식`으로 분기하고,
HP 범위별 상태 메시지는 `when` 패턴 매칭으로 출력한다.
마나 포션은 enum으로 정의만 해두고 실제 구현은 다음 프로젝트로 미뤘다.

```csharp
string result = type switch
{
    PotionType.Health => HealPlayer(ref hp),
    PotionType.Mana   => "Mana is already full.",
    _                 => "Unknown potion."
};

string hpStatus = hp switch
{
    var h when h >= maxHp        => $"{playerName} HP is full!",
    var h when h >= maxHp * 0.7f => $"{playerName} is in good condition.",
    var h when h >= maxHp * 0.3f => $"{playerName} needs caution.",
    _                            => $"{playerName} is in danger!"
};
```

### UI 시스템

- HP바: `Image.fillAmount` + `Mathf.Lerp` 부드러운 감소 연출
- 전투 로그: `MoveTowards` 기반 스크롤 (속도 Inspector 조절 가능)
- 페이드 아웃/인: 검은 패널 Alpha 코루틴 전환 연출
- 데미지 텍스트: 피격/공격 시 수치 표시 + 위로 이동하며 페이드 아웃
- 리플 VFX: 피격/공격 시 원형 파동 이펙트

---

## 트러블슈팅

### 스프라이트 전달 방식 설계 결정
씬 전환 시 선택한 캐릭터 스프라이트를 어떻게 넘길지 고민했다.
`GameDataManager`에 Sprite를 직접 담는 방식은 데이터와 에셋이 섞이는 구조라 배제했다.
`CharacterType` enum으로 인덱스만 넘기고 MainScene에서 매핑하는 방식을 택해
데이터와 에셋을 분리했다.

### 전투 로그 스크롤 끊김 현상
새 로그가 들어올 때마다 `StopCoroutine` → `StartCoroutine`을 반복하면서
이전 스크롤 애니메이션이 끊기는 현상이 있었다.
`Lerp` 방식에서 `MoveTowards` 방식으로 교체하고,
코루틴 참조를 변수로 관리해 이어서 진행하도록 수정했다.

### PlayerState vs PlayerStats 네이밍 혼선
`PlayerState`(enum 정의)와 `PlayerStats`(스탯 컴포넌트)의 이름이 비슷해
Unity Inspector에서 잘못 연결되는 실수가 발생했다.
파일명과 클래스명이 정확히 일치해야 한다는 점을 재확인했다.

### Awake / Start 실행 순서
BattlePrototype에서 겪었던 문제와 동일하게,
`CSVParser`를 `Awake`에서 실행해 파싱이 먼저 완료되도록 순서를 유지했다.

---

## 적용 개념

| 개념 | 적용 위치 |
|---|---|
| 생성자 | 캐릭터 3종 초기화 |
| out | 스탯 카드 표시 |
| ref | 포션 수량 + HP 동시 처리 |
| enum + switch | 전투 상태 머신 |
| switch 표현식 | 포션 타입 분기 |
| when | HP 범위별 상태 메시지 |

---

## 스크립트 구조

| 스크립트 | 역할 |
|---|---|
| `UnitData` | 캐릭터 데이터 + 생성자 + GetStatus |
| `GameDataManager` | 씬 간 캐릭터 데이터 전달 |
| `CharacterSelectManager` | 캐릭터 선택 씬 로직 |
| `PlayerState` | 전투 상태 enum 정의 |
| `PlayerStats` | 플레이어 스탯 + 포션 시스템 |
| `BattleManager` | 상태 머신 기반 전투 로직 |
| `BattleUIManager` | HP바, 로그, 데미지 텍스트 |
| `FadeManager` | 씬 전환 페이드 효과 |
| `BattleRippleEffect` | 피격/공격 리플 VFX |

---

## 시연 영상

[![BattleSystem 시연](https://img.youtube.com/vi/LhRqdZj9CHc/0.jpg)](https://youtu.be/LhRqdZj9CHc)

---

## 다음 단계

- 스킬/마나 시스템 미니프로젝트 — 이번 프로젝트의 PotionType.Mana를 실제로 구현
- Fatile 본 프로젝트에 상태 머신 전투 시스템 투입
- New Input System으로 전환 (Fatile 시작 시 적용 예정)
