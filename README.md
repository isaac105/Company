# 🏢 Office Revenge Game

**Unity 기반 오피스 복수 턴제 전투 게임**

## 📋 게임 개요

회사원이 각종 사무용품을 무기로 사용하여 직급별 상사들과 전투하는 턴제 RPG 게임입니다. 플레이어는 아이템을 선택하고 공격 타이밍을 맞춰 상사들을 차례로 물리쳐야 합니다.

## 🎮 조작 방법

### 기본 조작

- **Enter/마우스 클릭**: 아이템 선택
- **← →**: 아이템 선택지 이동 (좌우 화살표)
- **스페이스바**: 공격/방어 타이밍 실행
- **R키**: 전투 리셋
- **E키**: 적 상태 확인 (디버그용)

### 게임 흐름

1. **아이템 선택**: 하단 패널에서 마우스 클릭이나 좌우 화살표로 아이템 선택
2. **공격 타이밍**: Enter나 스페이스바로 공격 실행
3. **방어 타이밍**: 적의 공격에 대해 Enter나 스페이스바로 방어
4. **턴 반복**: 적이 쓰러질 때까지 반복

## 🏗️ 프로젝트 구조

### 📁 Assets/Scripts/

#### 🧑‍💼 캐릭터 시스템

- **`PlayerCharacter.cs`**: 플레이어 캐릭터 기본 클래스
  - HP, 공격력, 방어력 관리
  - 아이템 장착 시스템
- **`EnemyCharacter.cs`**: 적 캐릭터 기본 클래스 (추상)
  - 확률적 방어 시스템 (25%~65%)
  - 방어 봉인/감소 효과 처리
  - 턴 종료 시 효과 리셋

#### 👔 상사별 적 클래스 (EnemyCharacter 상속)

| 클래스                  | 직급   | HP  | 데미지 | 방어확률 | 특징                |
| ----------------------- | ------ | --- | ------ | -------- | ------------------- |
| `AssistantManagerEnemy` | 김대리 | 80  | 12     | 25%      | 신입 수준의 약한 적 |
| `ManagerEnemy`          | 박과장 | 120 | 18     | 35%      | 중간 관리자         |
| `DepartmentHeadEnemy`   | 최부장 | 180 | 25     | 45%      | 상급 관리자         |
| `DirectorEnemy`         | 정이사 | 250 | 35     | 55%      | 임원급 강적         |
| `CEOEnemy`              | 김사장 | 400 | 50     | 65%      | 최종 보스           |

#### 🛠️ 아이템 시스템

**기본 클래스**: `Item.cs`

- 통일된 특수효과 시스템
- 전략패턴으로 효과 처리
- UI Button 기반 클릭 감지

**아이템 구현체들**:

| 아이템              | 클래스              | 데미지 | 특수효과                 | 해금조건    |
| ------------------- | ------------------- | ------ | ------------------------ | ----------- |
| 📄 얇은 보고서 뭉치 | `ReportBundle`      | ×1.5   | 없음                     | 기본 제공   |
| ☕ 뜨거운 커피잔    | `HotCoffee`         | ×1.0   | 방어 봉인                | 대리 클리어 |
| 📎 스테이플러       | `Stapler`           | ×1.0   | 2회 연속공격             | 과장 클리어 |
| 💾 대용량 USB       | `LargeUSB`          | ×1.2   | 방어확률 30%↓            | 부장 클리어 |
| 🗂️ 결재 서류철      | `ApprovalDocuments` | ×3.0   | 고데미지 + 방어확률 20%↓ | 이사 클리어 |

#### ⚔️ 전투 시스템

**`CombatManager.cs`**: 전투 흐름 제어

- 상태 기반 전투 시스템 (ItemSelect → AttackTiming → DefenseTiming)
- 연속공격 처리 (`ExecuteMultipleAttacks`)
- UI 패널 관리

**전투 상태**:

```csharp
enum State { ItemSelect, AttackTiming, EnemyAttack, DefenseTiming }
```

#### 🎯 특수효과 시스템

**`ItemEffectType.cs`**: 전략패턴 구현

- `ItemEffectType` enum으로 효과 타입 정의
- `ItemEffectProcessor` 정적 클래스로 효과 처리
- 확장 가능한 구조

**효과 타입들**:

```csharp
public enum ItemEffectType
{
    None,                 // 효과 없음
    BlockNextDefense,     // 다음 방어 봉인
    DoubleAttack,         // 2회 연속공격
    ReduceEnemyDefense,   // 적 방어확률 감소
    HighDamage           // 고데미지 (계수로 처리)
}
```

## 🎲 전투 메커니즘

### 방어 시스템

- 각 적은 고유한 방어 확률을 가짐
- 방어 성공 시 **데미지 0**으로 완전 무효화
- 확률적 판정: `Random.Range(0f, 1f) < defenseChance`

### 연속공격 시스템

1. 스테이플러 선택 시 2회 연속공격 실행
2. 각 공격마다 개별 방어 판정
3. 적 사망 시 연속공격 즉시 중단
4. 성공한 공격 횟수와 총 데미지 표시

### 특수효과 적용 순서

1. **방어 조작 효과** 먼저 적용 (봉인/감소)
2. **연속공격** 확인 후 별도 처리
3. **일반 공격** 실행
4. **턴 종료** 시 일회성 효과 리셋

## 🎨 UI 구조

### Canvas 계층

```
Canvas
├── TopPanel (상단 정보)
│   ├── StageText (전투 상태)
│   ├── PlayerHpText (플레이어 HP)
│   └── EnemyHpText (적 HP)
├── GameArea (게임 영역)
│   ├── OfficeBackground
│   ├── PlayerCharacter
│   └── EnemyCharacter
└── BottomPanel (하단 UI)
    ├── ItemPanel (아이템 선택)
    ├── AttackTimingPanel (공격 타이밍)
    └── DefenseTimingPanel (방어 타이밍)
```

### 아이템 패널 구조

각 아이템은 Button 컴포넌트와 해당 Item 스크립트를 가짐:

- Background 이미지
- 클릭 가능한 아이템 이미지
- OnClick 이벤트로 CombatManager 연동

## 🔧 개발 패턴

### 상속 구조

```
MonoBehaviour
├── Item (추상)
│   ├── ReportBundle
│   ├── HotCoffee
│   ├── Stapler
│   ├── LargeUSB
│   └── ApprovalDocuments
├── EnemyCharacter (기본)
│   ├── AssistantManagerEnemy
│   ├── ManagerEnemy
│   ├── DepartmentHeadEnemy
│   ├── DirectorEnemy
│   └── CEOEnemy
├── PlayerCharacter
└── CombatManager
```

### 디자인 패턴

- **전략 패턴**: 아이템 특수효과 처리
- **상태 패턴**: 전투 흐름 관리
- **템플릿 메서드**: Enemy 초기화 구조
- **옵저버 패턴**: UI 업데이트

## 🚀 확장 가능성

### 새로운 아이템 추가

1. `ItemEffectType` enum에 새 효과 추가
2. `ItemEffectProcessor.ApplyEffect()`에 로직 구현
3. `Item` 상속 클래스 생성
4. UI에 GameObject와 Button 추가

### 새로운 적 추가

1. `EnemyCharacter` 상속 클래스 생성
2. `InitializeEnemy()`에서 스탯 설정
3. 필요시 고유 메서드 오버라이드

### 새로운 특수효과 추가

- 현재 구조로 쉽게 확장 가능
- 턴 기반 효과, 지속 효과 등 다양한 패턴 지원

## 🎯 전략 요소

### 아이템 시너지

- **커피 + 스테이플러**: 방어 봉인 후 2회 연속공격
- **USB + 서류철**: 방어력 대폭 감소 후 고데미지
- **보고서**: 안정적인 기본 데미지

### 적별 대응법

- **대리~과장**: 기본 아이템으로 충분
- **부장~이사**: 방어력 감소 아이템 필수
- **사장**: 최강 무기(서류철) 필요

## 🛠️ 디버그 기능

- **R키**: 전투 즉시 리셋
- **E키**: 적 상태 정보 출력
- **Inspector**: 실시간 HP, 방어확률 확인
- **Console**: 상세한 전투 로그

---

**개발 환경**: Unity 6000.0.45f1
**스크립팅**: C# (.NET Standard 2.1)  
**UI 시스템**: Unity UI (uGUI)
