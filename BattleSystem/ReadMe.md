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
버튼 클릭 시 `GetStatus(out int hp, out int atk, out int def)` 로
스탯을 읽어 StatCard UI에 표시한다.
확정 버튼을 누르면 `GameDataManager.selectedUnit`에 데이터를 저장하고
MainScene으로 전환된다.

### 상태 머신 (작업 중)

### 포션 시스템 (작업 중)

---

## 적용 개념

| 개념 | 적용 위치 |
|---|---|
| 생성자 | 캐릭터 3종 초기화 |
| out | 스탯 카드 표시 |
| ref | 포션 시스템 (예정) |
| enum + switch | 전투 상태 머신 (예정) |
| switch 표현식 | 포션 타입 분기 (예정) |
| when | HP 범위별 상태 메시지 (예정) |

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

<!-- 영상 링크 추가 예정 -->