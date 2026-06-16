# AGENTS.md — 프로젝트 에이전트 가이드

## 1. 언어

- 기본 언어는 **한국어**다.
- 영어를 병행해도 되나, 영어 사용 전 한국어를 반드시 먼저 쓰고 영어를 사용한다.

## 2. 프로젝트 개요

- .NET 10 기반 툴 모음.

## 3. 작업 우선순위

- `SECURITY_AUDIT.md` 파일의 패치를 **최우선**으로 처리한다.
- 모든 기능 개선은 **보안성 패치 이후**에 작업한다.
- 기능 추가가 요구되면 작업목록에 등록해 둔다.

## 4. 역할

| 구분 | 담당 |
| ---- | ---- |
| 개발 감독 및 승인 | 사용자 |
| 개발 및 커밋 | 에이전트 — Codex, Claude Code |

## 5. 작업 규칙

- 모든 수정은 **`FrameworkCommonTest`에 테스트케이스를 작성**한다.
- 개발 진행 중 정보가 부족하면 **사용자에게 반드시 문의**한다.
- 사용자가 코드나 md 파일을 직접 수정할 때가 있다. 에이전트는 이것이 발견되어도 묻지 말고 함께 커밋한다. (폐쇄형 작업공간이라 위험한 injection은 일어나지 않는다.)

## 6. 명령어

| 용도 | 명령 |
| ---- | ---- |
| 빌드 | `dotnet build Framework.Net10.sln` |
| 테스트 | `dotnet test FrameworkCommonTest/Framework.Test.Unit.Common.csproj` |
| 커밋 | `git add -A && git commit -m "<메시지>"` |
| 푸시 | `git push origin main` |

## 7. 인코딩

- 모든 파일의 **기본 인코딩은 UTF-8 (BOM 없음)** 이다.
  - MySQL의 `utf8mb4`와 동일한 **완전 UTF-8**(4바이트 문자·이모지 포함 전 유니코드 지원)을 의미한다.
  - MySQL의 `utf8`(=`utf8mb3`, 3바이트)은 사용하지 않는다.
- 저장소 차원 적용: 루트 `.editorconfig`의 `charset = utf-8` (`[*]` — 모든 파일).
- 코드에서 바이트↔문자열 변환 시 `Encoding.Default`(런타임/플랫폼 의존)를 쓰지 말고 **`Encoding.UTF8`을 명시**한다.
- DB 연동 시 문자셋은 `utf8mb4`를 사용한다.
