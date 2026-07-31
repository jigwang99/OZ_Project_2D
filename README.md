# 2D RTS Game (Unity)

Unity로 제작한 2D 실시간 전략(RTS) 게임입니다. 자원 채집 → 건물 건설 → 유닛 생산 → 전투로 이어지는 고전 RTS의 핵심 루프를 구현했으며, A\* 길찾기, FSM 기반 유닛 제어, 모듈형 적 AI, 오브젝트 풀링, ScriptableObject 데이터 기반 설계를 중심으로 구성했습니다.

승리 조건은 상대 진영의 성(Castle)을 모두 파괴하는 것이고, 아군 성이 모두 파괴되면 패배합니다.

---

## 주요 기능

- **자원 & 인구 관리** — 나무(Wood)/금(Gold) 두 종류 자원과 인구(Population) 시스템
- **유닛 생산 & 건물 건설** — 명령 카드(Command Card) UI와 단축키 기반 조작, 고스트 프리뷰로 배치
- **드래그/클릭 선택** — 다중 유닛 드래그 선택 및 단일 클릭 선택
- **A\* 경로 탐색** — 그리드 기반 길찾기와 경로 단순화(SimplifyPath)
- **FSM 기반 유닛/건물 제어** — Idle, Attack, Chase, Gather, Build, Heal 등 상태 전환
- **모듈형 적 AI** — 페이즈(Gather → Build → Combat)에 따라 자원/생산/건설/전투 모듈이 자율 판단
- **오브젝트 풀링** — 유닛·건물·자원·투사체 재사용
- **데이터 기반 설계** — 유닛/건물 스탯을 ScriptableObject로 분리

---

## 기술 스택

- **엔진**: Unity (2D)
- **언어**: C#
- **입력**: Unity Input System (New Input System)
- **UI**: TextMeshProUGUI
- **설계 패턴**: FSM(유한 상태 기계), Singleton, Object Pool, Strategy(적 AI 모듈), ScriptableObject

---

## 프로젝트 구조

```
Assets/
├── 01.Scripts/
│   ├── Manager/          # GameManager, GridManager, UIManager, SelectManager 등 전역 매니저
│   ├── Player/           # Player(선택·명령), BuildPlacer(건물 배치)
│   ├── Faction/          # Faction, FactionManager (진영·자원·인구 관리)
│   ├── Units/            # Pawn, Warrior, Archer, Lancer, Monk 및 이동/채집/전투 컴포넌트
│   │   └── Unit/UnitMove/ # PathFinder, Node, Heap (A* 길찾기)
│   ├── Building/         # Castle, House, Barracks, Archery, Monastery, Tower
│   ├── FSM/              # StateMachine, UnitState/, BuildingState/
│   ├── Enemy/            # EnemyCommander + Gather/Production/Construction/Combat 모듈
│   ├── Resource/         # Resource, ResourceManager (나무/금 채집)
│   ├── UI/               # CommandCardUI, SelectionUI, GameEndUI 등
│   └── Types/            # UnitType, BuildingType, ResourceType, FactionType 등 enum
├── 02.Prefabs/           # 유닛·건물·자원 프리팹
└── 03.Data/
    ├── ScriptableObjects/ # UnitData, BuildingData, ResourceData 정의
    └── ObjectData/        # 실제 데이터 에셋
```

---

## 유닛

| 유닛 | 역할 | 특징 |
|------|------|------|
| **Pawn** | 일꾼 | 자원 채집 및 건물 건설 |
| **Warrior** | 근접 전투 | 근접 공격(MeleeAttack) |
| **Archer** | 원거리 전투 | 원거리 공격(RangedAttack) |
| **Lancer** | 근접 전투 | 근접 공격(MeleeAttack) |
| **Monk** | 지원 | 아군 유닛 치유(Heal) |

각 유닛의 체력·공격력·이동속도·생산 비용 등은 `UnitData` (ScriptableObject)에서 관리됩니다.

---

## 건물

| 건물 | 역할 | 체력 | 비용 |
|------|------|------|------|
| **Castle** | 본진 / 일꾼 생산 / 자원 반납 | 500 | 나무 400 |
| **House** | 인구 상한 증가 | 100 | 나무 100 |
| **Barracks** | 근접 유닛 생산 | 150 | 나무 150 |
| **Archery** | 원거리 유닛 생산 | 150 | 나무 150 |
| **Monastery** | Monk 생산 | 200 | 나무 100 + 금 200 |
| **Tower** | 방어 (자동 공격) | 100 | 나무 100 |

건물 스탯은 `BuildingData` (ScriptableObject)에서 관리됩니다.

---

## 핵심 시스템

### A\* 경로 탐색
`GridManager`가 월드 공간을 노드 그리드로 분할하고, `PathFinder`가 이진 힙(`Heap`) 기반 우선순위 큐로 최단 경로를 탐색합니다. 탐색 결과는 `SimplifyPath`로 방향이 바뀌는 지점만 남겨 경유점을 최소화하며, 목표 지점이 장애물일 경우 가장 가까운 이동 가능 노드로 대체합니다. 매 탐색마다 노드를 전부 초기화하지 않고 `searchVersion`으로 유효성을 판단해 재사용합니다.

### FSM (유한 상태 기계)
`StateMachine`을 중심으로 유닛과 건물의 행동을 상태 단위로 분리했습니다.

- **유닛 상태**: Idle → (적/자원/건물 탐지) → Attack / Chase / Gather / Build / Heal
- **건물 상태**: Idle / Builded / Product / Attack(Tower)

각 상태는 `Enter / Update / FixedUpdate / Exit`를 구현하며, 상태 전환 시점에 필요한 로직만 실행합니다.

### 적 AI (모듈형)
`EnemyCommander`가 일정 주기(`thinkInterval`)로 상황을 판단하고, 현재 페이즈에 따라 4개의 모듈을 순차 실행합니다.

- **페이즈 흐름**: `Gather`(자원 확보) → `Build`(건물 건설) → `Combat`(병력 집결 후 공격)
- **모듈**: `EnemyGatherModule`, `EnemyProductionModule`, `EnemyContructionModule`, `EnemyCombatModule`

전투 모듈은 우선순위에 따라 기지 방어 → 병력 집결 후 공격을 판단하며, 스쿼드 단위로 대형(Formation)을 유지하며 이동합니다. 생산·건설 규칙은 페이즈별 우선순위와 최대 개수 기반으로 동작해 인스펙터에서 조정할 수 있습니다.

### 오브젝트 풀링
`ObjectPoolManager`를 통해 유닛·건물·자원·투사체를 재사용합니다. 생성/파괴 대신 풀에서 꺼내고 반납하는 방식입니다.

---

## 조작 방법

| 입력 | 동작 |
|------|------|
| 좌클릭 | 유닛/건물 선택 |
| 좌클릭 드래그 | 다중 유닛 선택 |
| 우클릭 | 이동 / 공격 / 채집 (대상에 따라 자동 판단) |
| A / S / D / F | 선택한 생산 건물의 유닛 생산 (명령 카드 단축키) |
| ESC | 마지막 생산 취소 / 배치 취소 |

---
- 적 AI를 판단(Commander) / 실행(Module)으로 분리하고 페이즈 기반 전략을 구성
- ScriptableObject로 데이터와 로직을 분리하여 코드 수정 없이 밸런싱 가능
- A\* 길찾기에 힙·경로 단순화·노드 버전 재사용 등 성능 최적화 적용
