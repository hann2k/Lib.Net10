# BACKLOG — 기능 추가 작업목록

> AGENTS.md 규칙: 요구되는 기능 추가는 이 작업목록에 등록한다. (보안 패치는 `SECURITY_AUDIT.md` 우선)

## 대기 (Pending)

### TLS 전송 암호화 적용
- **출처**: `SECURITY_AUDIT.md` 1-3 (위험 수용 → 향후 개선)
- **내용**: `NetworkStream`을 `SslStream`으로 래핑하여 전송 구간 암호화 적용. 서버 인증서 도입.
- **대상**: `TcpTaskClient`, `TCPClient`, `TcpListenerServer` / `TcpListenerServer2` / `TcpMultiServer`
- **연계**: 1-2(클라이언트 인증)와 결합 시 상호 TLS(mTLS)로 일원화 가능.
- **착수 트리거**: 폐쇄망 밖 노출 / 루프백 아닌 주소 바인딩 / 기밀성·무결성 요구사항 신규 추가.
- **상태**: [ ] 미착수

### 서버측 비동기 인바운드 핸들러 신규 + TcpMultiServer 이관
- **출처**: 결함 2-1 정리 중 결정 (TcpTaskClient는 최소 기능으로 동결)
- **배경**: `TcpTaskClient`(서버용 인바운드 핸들러)는 레거시로 동결. 취소토큰·송신큐·견고한 수명관리가 필요하면 현행 `TCPClient`(클라이언트 전용, 아웃바운드)의 검증된 패턴을 재사용한다.
- **내용**: accept된 소켓을 부착하는 진입점(`AttachAsync(TcpClient)` 등)을 갖춘 서버측 핸들러를 신규 작성하고, `TcpMultiServer`를 이관.
- **연계 해소**: 결함 8-1(핸들러 객체 공유), 8-2(Dictionary 비동기화)도 이관 과정에서 함께 정리.
- **상태**: [ ] 미착수

## 진행 중 (In Progress)

(없음)

## 완료 (Done)

(없음)
