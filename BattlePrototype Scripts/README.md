# BattlePrototype — Mini Project 01

> CSV 데이터 파이프라인 + 턴제 전투 시스템 프로토타입  
> Unity 6.2 / Core Dive Studio

---

## 목표

이 미니프로젝트는 두 가지 핵심 역량을 내 것으로 만들기 위해 시작했다.

1. **기획 데이터 파이프라인** — 기획자가 스프레드시트로 관리하는 데이터를 코드 수정 없이 게임에 반영하는 구조
2. **전투 시스템 프로토타입** — Fatile(진행 중인 타일 기반 로그라이크) 투입을 전제로 한 실전성 있는 전투 로직

---

## 구현 내용

### CSV → ScriptableObject 파이프라인

구글 시트에서 몬스터 데이터를 작성하고 CSV로 내보낸다. `CSVParser`가 런타임에 이를 읽어 `MonsterData` ScriptableObject 인스턴스로 변환한다.

```
Google Sheets → monsters.csv → CSVParser → MonsterData[] → BattleManager
```

- `Split('\n')` + `Split(',')` 으로 CSV 파싱
- `int.Parse` / `float.Parse` 로 타입 변환
- `Resources.Load<Sprite>` 로 스프라이트 자동 로드
- 몬스터 추가/수정 시 코드 수정 없이 시트만 편집하면 됨

### JSON 파서 (추가 구현)

같은 데이터를 JSON으로도 파싱할 수 있도록 `JSONParser`를 별도로 구현했다. `JsonUtility.FromJson<T>` 의 자동 매핑 원리와 `[System.Serializable]` 어트리뷰트의 역할을 학습했다.

### 타일맵 + 플레이어 이동

Unity Tilemap 시스템으로 맵을 구성하고, 플레이어가 타일 단위로 이동한다. N걸음마다 인카운터가 발생한다.

### 전투 시스템

- 코루틴 기반 턴제 전투 (`TakeTurn` 재귀 코루틴)
- `Mathf.Max(0, attack - defense)` 방어력 적용 데미지 계산
- `Random.value < dropRate` 드랍 확률 처리
- `StringBuilder` 로 전투 로그 누적
- `ScrollRect + ContentSizeFitter + Mask` 로 로그 스크롤 UI 구현

### UI 시스템

- HP바: `Image.fillAmount` + `Mathf.Lerp` 부드러운 감소 연출
- 전투 로그: ScrollRect 자동 스크롤 (Lerp 적용)
- 페이드 아웃/인: 검은 패널 Alpha 코루틴 전환 연출
- 데미지 텍스트: 피격/공격 시 수치 표시

### VFX — 리플 이펙트

화투짝맞추기에서 구현했던 탭 리플 이펙트를 전투용으로 리팩터링했다. 탭 이벤트 대신 `ShowPlayerHit()` / `ShowEnemyHit()` 메서드로 전투 판정에 연동했다. 색상, 크기, 속도, 개수를 Inspector에서 실시간 조절 가능하다.

---

## 트러블슈팅

### CSV와 JSON 파일명 충돌
`Resources.Load<TextAsset>("monsters")` 는 확장자를 구분하지 않는다. `monsters.csv`와 `monsters.json`이 같은 폴더에 있으면 JSON이 우선 로드되어 CSV 파서가 오동작했다. JSON 파일을 별도 폴더로 분리해 해결.

### 몬스터 이미지 타이밍 문제
전투 시작 시 이전 몬스터 이미지가 잠깐 보이거나, 첫 전투에서 이미지가 늦게 나타나는 현상이 있었다. 원인은 `BattlePanel` 비활성화 상태에서 `SetEnemySprite`를 호출하거나, Canvas 렌더링 타이밍이 맞지 않았기 때문이다.

해결: 페이드아웃 직후 (검은 화면일 때) `BattlePanel.SetActive(true)` → `PrepareMonster()` 순서로 처리. 전투 종료 후에는 `ClearEnemy()`로 이미지를 비활성화 후 다음 몬스터를 미리 준비해둔다.

### Input System 충돌
Unity 6에서 New Input System이 기본 활성화되어 있어 `Input.GetKeyDown`이 작동하지 않았다. Project Settings → Player → Active Input Handling을 `Both`로 설정해 해결.

### Awake / Start 실행 순서
`BattleManager.Start`가 `CSVParser.Awake`보다 먼저 실행되어 몬스터 데이터가 비어있는 상태에서 전투를 시작하려 했다. CSVParser를 `Start`에서 `Awake`로 변경해 파싱이 먼저 완료되도록 순서를 조정했다.

---

## 스크립트 구조

| 스크립트 | 역할 |
|---|---|
| `CSVParser` | CSV 읽기 → MonsterData 배열 생성 |
| `JSONParser` | JSON 읽기 → MonsterData 배열 생성 |
| `MonsterData` | 몬스터 데이터 ScriptableObject |
| `PlayerStats` | 플레이어 스탯 |
| `PlayerMovement` | 타일 이동 + 인카운터 발생 + 페이드 전환 |
| `BattleManager` | 전투 로직 (턴 처리, 드랍, 종료) |
| `BattleUIManager` | HP바, 로그, 이미지, 데미지 텍스트 |
| `FadeManager` | 페이드 아웃/인 코루틴 |
| `BattleRippleEffect` | 피격/공격 리플 VFX |

---

## 시연 영상

<!-- 영상 링크 추가 예정 -->

---

## 다음 단계

- JSON 파서 심화 미니프로젝트 (별도 진행 예정)
- Fatile 본 프로젝트에 전투 시스템 투입 시 ScriptableObject → JSON 파이프라인으로 전환
- 몬스터 사망 연출 (RIP 이미지 교체) 추가 예정
