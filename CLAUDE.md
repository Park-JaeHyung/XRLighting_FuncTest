# XRLighting_FuncTest

조명 감독 직업체험 XR 콘텐츠의 PC 기능 검증 프로젝트.
XR 이식 전 핵심 루프(배치→제어→큐→큐시트→재생)를 Unity PC 환경에서 먼저 검증한다.

## 기술 스택

- Unity 6000.0.66f2
- URP (Universal Render Pipeline)
- Unity Input System
- UI Toolkit

## 구조

```
Assets/
  Scripts/
    Domain/           # 순수 데이터 클래스 (MonoBehaviour 없음)
      LightParameters.cs
      FixtureProfile.cs     # ScriptableObject
      Cue.cs
      CueSheet.cs
    Interfaces/
      ILightControllable.cs
      IPlacementInput.cs
    Runtime/          # MonoBehaviour 컴포넌트
      FixtureInstance.cs
      FixtureManager.cs
      CueManager.cs
      CueInterpolator.cs
      PCPlacementInput.cs
    UI/               # UI Toolkit 바인딩
      SpaceSelectionUI.cs
      FixturePlacementUI.cs
      ControlPanelUI.cs
      CueSheetEditorUI.cs
  ScriptableObjects/  # FixtureProfile 에셋 (무빙헤드/PAR/스팟 등)
  Scenes/
    Main.unity        # 단일 진입 씬
  UI/                 # UXML / USS 파일
Settings/             # URP 렌더 설정
```

## 도메인 모델

| 클래스 | 역할 |
|---|---|
| `LightParameters` | Color, Intensity, Pan, Tilt, BeamAngle 등 한 시점의 조명 상태 |
| `FixtureProfile` | ScriptableObject. 조명 종류 정의 + 프리팹 참조 |
| `FixtureInstance` | 씬에 배치된 조명. Unity Light 래핑, 파라미터 적용 |
| `Cue` | 전체 조명 스냅샷 + CueNumber, Label, FadeTime |
| `CueSheet` | Cue 의 순서열. 재생 타이밍 관리 |

## 아키텍처

```
Control Panel UI
      ↓ 이벤트 기반 (UI가 Fixture를 직접 참조하지 않음)
Fixture Layer  (ILightControllable)
      ↓ Record → 스냅샷
Cue System  (CueManager)
      ↓
Cue Sheet Editor UI
```

- 레이어 간 통신은 이벤트/콜백으로 분리
- 조명 제어는 `ILightControllable` 로 추상화
- 배치 입력은 `IPlacementInput` 으로 추상화 (추후 XR 컨트롤러 교체 지점)
- 보간은 파라미터 타입별로 분리: Color→HSV, Pan/Tilt→최단각, Intensity→선형

## 개발 규칙

### 코드 작성

- MVP 단계에서는 프리미티브 도형 + Unity 기본 UI로 전체 루프를 우선 완성한다
- 실제 에셋/스킨은 기능 검증 이후 적용
- Domain 폴더의 클래스는 UnityEngine 의존을 최소화 (ScriptableObject 제외)
- `ILightControllable`, `IPlacementInput` 인터페이스를 통해서만 하위 레이어에 접근
- 주석은 WHY가 불명확한 곳에만 작성. 코드가 설명하는 내용은 주석 금지

### 새 Fixture 종류 추가 시

1. `Assets/ScriptableObjects/` 에 `FixtureProfile` 에셋 생성
2. 지원 파라미터 목록 설정
3. 프리미티브 프리팹 연결 (MVP) 또는 실제 모델 프리팹 연결 (이후)
4. `CueInterpolator` 에 해당 파라미터 보간 처리 여부 확인

### UI 추가/수정 시

- UXML/USS 는 `Assets/UI/` 에 배치
- 코드비하인드(`.cs`)는 `Assets/Scripts/UI/` 에 배치
- UI 이벤트에서 직접 Fixture/Cue 를 조작하지 말고 이벤트/Action 으로 위임

## 검증

씬을 Play 해서 아래 전체 루프가 막힘 없이 동작하는지 확인한다:

```
공간 선택 → 조명 배치 → 파라미터 조정 → 큐 저장 → 큐시트 저장 → 자동 재생
```

각 단계 완료 후 확인 포인트:

- **Fixture 시스템**: 클릭으로 조명 생성, 파라미터 변경 시 Light 컴포넌트에 즉시 반영
- **Cue 시스템**: 저장 버튼 → CueSheet 에 큐 추가, 재생 시 크로스페이드 동작
- **UI**: 모든 버튼/슬라이더가 해당 기능과 연결됨

## 작업 마무리 시

- 전체 루프 Play 테스트 실행
- 테스트용 임시 GameObject, Debug.Log 정리
- 관련 없는 변경은 별도 커밋으로 분리
- `.cs` 파일과 `.cs.meta` 파일은 항상 함께 커밋

## Git

- 브랜치: `pc` (PC MVP 개발), `master` (안정 버전)
- 단계별(도메인→Fixture→Cue→UI) 완료 시 커밋
- 커밋 메시지는 변경 목적 위주로 작성

## MVP 구현 단계

1. **도메인 데이터** — LightParameters, FixtureProfile, Cue, CueSheet, 인터페이스
2. **Fixture 시스템** — FixtureInstance, FixtureManager, PCPlacementInput, 프로필 에셋
3. **Cue 시스템** — CueManager, CueInterpolator
4. **UI** — SpaceSelectionUI → FixturePlacementUI → ControlPanelUI → CueSheetEditorUI
5. **씬 연결 및 전체 루프 검증**
