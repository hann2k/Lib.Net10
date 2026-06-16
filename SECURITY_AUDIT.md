# FrameworkCommon 보안 취약점 점검표

> 대상: `FrameworkCommon/`
> 분석일: 2026-06-16
> 분석 범위: TCP 통신 · 바이너리 패킷 파싱 · INI 설정 · 로깅
> 사용법: 항목별 체크박스(`- [ ]`)를 수정 완료 시 `- [x]`로 변경하며 하나씩 진행. 처리일시 추가
> 분리 원칙: 하나의 결함이 독립된 여러 문제로 구성되면 묶지 말고 `N-1 / N-2 / …`로 분리해 각각 위치·상태·처리일시를 따로 추적한다.

---

## 우선순위 요약

| #  | 항목                                             | 심각도    | 상태 | 처리일시 |
| -- | ------------------------------------------------ | --------- | ---- | -------- |
| 1-1 | 외부 인터페이스 바인딩 (0.0.0.0) — 루프백으로 제한 | 🔴 High   | [x]  | 2026-06-16 |
| 1-2 | 클라이언트 인증(authN) 부재                      | 🔴 High   | [x] 위험수용 | 2026-06-16 |
| 1-3 | 전송 구간 암호화(TLS) 부재                       | 🔴 High   | [x] 위험수용(향후개선) | 2026-06-16 |
| 2-1 | TcpTaskClient 미사용 수신버퍼 누적 (DoS)        | 🔴 High   | [x]  | 2026-06-16 |
| 2-2 | PacketTwoWay 누적 버퍼 무제한 (상한 필요)        | 🔴 High   | [ ]  |          |
| 3  | Beacon 패킷 크기 검증 비활성화                   | 🔴 High   | [ ]  |          |
| 4  | INI 파싱이 악성/오타 설정에 취약 (크래시)        | 🟠 Medium | [ ]  |          |
| 5-1 | 파일 읽기 경로 무검증 (경로 탐색)               | 🟠 Medium | [ ]  |          |
| 5-2 | 파일 쓰기 경로 무검증 (임의 파일 쓰기)          | 🟠 Medium | [ ]  |          |
| 6  | 레거시 P/Invoke INI + 고정 255 버퍼              | 🟠 Medium | [ ]  |          |
| 7  | Log 클래스 락 불일치 (경합 조건)                 | 🟡 Low    | [ ]  |          |
| 8-1 | TcpMultiServer 핸들러 객체 공유                  | 🟡 Low    | [ ]  |          |
| 8-2 | Clients Dictionary 비동기화 접근 (경합)         | 🟡 Low    | [ ]  |          |
| 9  | ByteConverter 정수 오버플로 가능성               | 🟡 Low    | [ ]  |          |
| 10 | `Encoding.Default` 사용                        | 🟡 Low    | [x]  | 2026-06-16 |
| 11 | Newtonsoft.Json 의존성 제거 → System.Text.Json   | 🟡 Low    | [x]  | 2026-06-16 |

---

## 🔴 High

> 결함 1(평문·무인증 TCP 서버)은 실제로 **바인딩 노출 / 인증 부재 / 암호화 부재** 세 가지 독립 문제로 구성되므로 1-1·1-2·1-3으로 분리한다.

### 1-1. 외부 인터페이스 바인딩 (0.0.0.0) — 루프백으로 제한

- [x] 수정 완료 (2026-06-16)

**위치**

- `FrameworkCommon/Comm/Tcp/TcpMultiServer.cs:37` — `new TcpListener(IPAddress.Any, port)`
- `FrameworkCommon/Comm/Tcp/TcpSingleServer.cs:407-409` — `IPAddress.Any` + `ReuseAddress=true`
- `FrameworkCommon/Comm/Tcp/TcpSingleServer2.cs:72`

**문제**
서버가 모든 인터페이스(`0.0.0.0`)에 바인딩되어 LAN/외부에서 도달 가능하다.

**영향**
무단 접속 경로 노출.

**조치 (적용 완료)**

- `TcpListenerServer` / `TcpListenerServer2`: 기본 `IPAddress.Loopback`, `SetBindAddress(string)` 메서드 추가(수신 시작 전에만 호출 가능, 잘못된 IP는 `ArgumentException`).
- `TcpMultiServer`: 생성자에서 즉시 바인딩하므로 메서드 대신 생성자 인자 `bindIp`(기본 루프백) 추가. `BindAddressText` getter 제공.
- 테스트: `FrameworkCommonTest/TcpServerBindAddressTest.cs` (11종, 전부 통과).

---

### 1-2. 클라이언트 인증(authN) 부재

- [x] **위험 수용(Accepted Risk)** 결정 (2026-06-16) — 코드 변경 없음

**위치**

- `FrameworkCommon/Comm/Tcp/TcpMultiServer.cs:51-58` — `AcceptTcpClient()` 후 검증 없이 클라이언트 등록
- `FrameworkCommon/Comm/Tcp/TcpSingleServer.cs:416-420` — `AcceptTcpClientAsync` 후 즉시 연결 처리

**문제**
TCP 핸드셰이크만 성공하면 신원 검증(토큰/인증서/사전공유키) 없이 정식 클라이언트로 인정된다.

**영향**
무단 클라이언트의 명령/패킷 주입.

**결정: 위험 수용 (Accepted Risk)**

- **근거**: 1-1 수정으로 기본 바인딩이 루프백(`127.0.0.1`)으로 제한되어 외부 도달이 차단됨. 운영 전제가 폐쇄형 작업공간이며, 인증 요구사항이 없음. 루프백 전용 운용에서는 인증 부재의 실질 위험이 낮다고 판단.
- **전제 조건**: 서버를 루프백에만 바인딩하여 운용한다.
- **🔁 재검토 트리거**: 다음 중 하나라도 발생하면 이 결정을 무효화하고 **1-2(PSK 또는 mTLS)와 1-3(TLS)를 함께 재검토**한다.
  - `SetBindAddress(...)` / 생성자 `bindIp`로 **루프백이 아닌 주소**(예: `0.0.0.0`, LAN/공인 IP)에 바인딩하는 경우
  - 신뢰할 수 없는 네트워크 구간에 서버를 노출하는 경우
  - 인증 관련 요구사항이 새로 추가되는 경우

**미적용 조치 (재검토 시 채택안)**

- 접속 직후 인증 단계 추가(토큰/인증서/PSK), 실패 시 즉시 연결 종료.
- 1-3(TLS)와 결합 시 상호 TLS(mTLS)로 일원화 가능.

---

### 1-3. 전송 구간 암호화(TLS) 부재

- [x] **위험 수용(Accepted Risk)** 결정 (2026-06-16) — 코드 변경 없음, **향후 기능 추가로 개선 예정**

**위치**

- `FrameworkCommon/Comm/Tcp/TcpTaskClient.cs:38,46,109` — 원시 `NetworkStream` 직접 read/write
- `FrameworkCommon/Comm/Tcp/TCPClient.cs:241,257` — `Nstream` 평문 송수신

**문제**
`SslStream` 등 암호화 계층 없이 평문으로 송수신하여 도청/변조(MITM)에 취약하다.

**영향**
트래픽 도청, 중간자 변조.

**결정: 위험 수용 (Accepted Risk) — 향후 개선 예정**

- **근거**: 현재 전 구간 **폐쇄망(closed network)** 운용이며, 1-1 수정으로 기본 바인딩이 루프백으로 제한됨. 폐쇄망 전제에서는 도청/변조 위험이 낮다고 판단.
- **전제 조건**: 폐쇄망 내에서만 운용한다.
- **개선 계획**: TLS 적용은 **기능 추가 작업으로 별도 진행**한다. → `BACKLOG.md`에 등록.
- **🔁 재검토 트리거**: 다음 중 하나라도 발생하면 이 결정을 무효화하고 TLS 적용을 진행한다.
  - 폐쇄망이 아닌 네트워크 구간에 트래픽이 노출되는 경우
  - 루프백이 아닌 주소에 바인딩하는 경우(1-2와 동일 트리거)
  - 전송 데이터 기밀성/무결성 요구사항이 새로 추가되는 경우

**미적용 조치 (개선 시 채택안)**

- `NetworkStream`을 `SslStream`으로 래핑, 서버 인증서 적용.
- 1-2(인증)와 결합 시 상호 TLS(mTLS)로 일원화 가능.
- `ReuseAddress=true`는 꼭 필요한 경우에만 사용(포트 하이재킹 완화).

---

> 결함 2는 대상 클래스의 성격이 달라(미사용 죽은 버퍼 vs 정상 누적기) 2-1·2-2로 분리한다.

### 2-1. TcpTaskClient 미사용 수신버퍼 누적 (DoS)

- [x] 수정 완료 (2026-06-16)

**위치**

- `FrameworkCommon/Comm/Tcp/TcpTaskClient.cs` — (구) `RcvBuffer.AddRange(rcvs)` 무제한 누적

**문제**
`RcvBuffer`가 쓰기만 되고 소비처가 없었다(`ParseReceivedDatas`는 private + 호출처 없음 = 죽은 코드). 수신 데이터는 이미 `ReceivedData` 이벤트로 전달되고 있어 버퍼 누적은 순수 메모리 누수였다.

**영향**
원격 메모리 고갈(DoS).

**조치 (적용 완료) — 불필요 코드 제거 + 최소 기능 정리**

- 죽은 `RcvBuffer` 필드와 `ParseReceivedDatas()` 메서드 **제거** → DoS 벡터 자체 소멸.
- 버려지던 `new TcpClient()` 필드 초기화 제거(소켓 누수), 불필요한 `Clone()` 제거, 핫패스 `Console.WriteLine` 제거.
- 수신 루프에 `try/catch/finally` 추가 → 비정상 종료에도 `ClientDisconnected` 이벤트 보장.
- 빈 `Close()`를 실제 종료(스트림/소켓 닫기)로 구현.
- 데이터 전달은 기존대로 `ReceivedData` 이벤트 유지(서버측 인바운드 핸들러 역할 불변).
- 테스트: `FrameworkCommonTest/TcpTaskClientTest.cs` (3종, 전부 통과).
- 참고: 취소토큰·송신큐 등 고급 수명관리는 미적용 — 신규 인바운드 핸들러로 대체 예정(`BACKLOG.md`).

---

### 2-2. PacketTwoWay 누적 버퍼 무제한 (상한 필요)

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Comm/Protocol/PacketTwoWay.cs:34-42` — `SetRcvData` 오버로드가 상한 없이 `AddRange`

**문제**
`RcvBuffer`가 `SetRcvData`/`RcvLength`/`ClearRcv`로 실제 사용되는 정상 누적기다(요청-응답 매칭용). 단, 소비자가 `ClearRcv`를 호출하지 않거나 공격자가 패킷 경계 없이 흘려보내면 무한 증가한다.

**영향**
원격 메모리 고갈(DoS).

**조치 (미적용)**

- 설정 가능한 상한(`MaxReceiveBufferBytes`, 기본 1 MiB) 추가, append 시점에 강제.
- 초과 시 `virtual` 훅(기본: `ClearRcv`로 리셋) — 서브클래스 재정의 가능.
- 2-1과 달리 제거 불가(공개 API + 의도된 설계)이므로 상한 방식으로 처리.

---

### 3. 패킷 크기 검증이 비활성화됨 (Beacon)

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Comm/Protocol/Beacon/BeaconPacket.cs:132-138`
- 최소 길이 가드 부재: `FrameworkCommon/Comm/Protocol/Beacon/BeaconPacket.cs:176-179`

**문제**

```csharp
var size = header[5] + (header[6] * 256) - header.Length;
if (bodyLength != size) {
    base.Error.SetReason(...);
    // return false;   ← 주석 처리되어 검증이 무력화됨
}
```

크기 불일치 시 `return false`가 주석 처리되어, 헤더가 선언한 길이와 실제 바디 길이가 달라도 그대로 `Parse()`로 진행한다. 또한 `SetReceivedBytes`의 `GetRange(0,7)`/`GetRange(7, array.Length-7)`는 7바이트 미만 패킷에서 예외가 난다(현재 try/catch로 무시되지만 사전 길이 검사가 없음).

**영향**
하위 `Parse` 구현이 헤더 길이/인덱스를 신뢰하면 OOB(범위 밖) 읽기, 오동작.

**조치**

- 길이 검증(`return false`) 복원.
- `SetReceivedBytes` 진입 시 최소 길이(7바이트) 가드 추가.

---

## 🟠 Medium

### 4. INI 파싱이 악성/오타 설정에 취약 (크래시)

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Config/INI_UTF8.cs:93-94`

**문제**

```csharp
var items = line.Split('=');
var it = new Item(items[0], items[1], i);  // '=' 없는 줄 → items[1] IndexOutOfRangeException
```

`SetIniFile` 전체에 try/catch가 없어 `=`가 없는 한 줄만 있어도 설정 로드가 통째로 중단된다. 값에 `=`가 포함되면 잘려나가는 문제도 있다.

**영향**
설정 파일 손상/오타로 인한 시작 실패(가용성).

**조치**

- 구분자 부재/형식 오류 줄은 건너뛰고 로깅.
- `Split('=', 2)` 사용으로 값 내 `=` 보존.

---

> 결함 5(파일 경로 무검증)는 **읽기 경로**와 **쓰기 경로**가 노출 지점·영향이 다르므로 5-1·5-2로 분리한다.

### 5-1. 파일 읽기 경로 무검증 (경로 탐색)

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Config/INI_UTF8.cs:51-55` — `SetIniFile` → `File.ReadLines`

**문제**
읽을 파일 경로를 정규화·검증 없이 그대로 `File.ReadLines`에 전달한다. 경로가 외부 입력(원격 명령, 설정값)에서 오면 `..\..\` 경로 탐색으로 허용 범위 밖 파일을 읽을 수 있다.

**영향**
임의 파일 읽기(정보 노출), 경로 탐색.

**조치**

- `Path.GetFullPath`로 정규화 후 허용 루트(base dir) 하위인지 검증.
- 검증 실패 시 로드 거부 + 로깅.

---

### 5-2. 파일 쓰기 경로 무검증 (임의 파일 쓰기)

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Logger/Log.cs:51-58` — `SetLogDir` → `Directory.CreateDirectory`
- `FrameworkCommon/Config/INI_UTF8.cs:158-170` — `WriteFile` → `File.CreateText`

**문제**
쓰기 대상 경로(로그 디렉터리, INI 파일)를 검증 없이 그대로 파일/디렉터리 API에 전달한다. 경로가 외부 입력에서 오면 임의 위치에 파일·디렉터리를 생성하거나 기존 파일을 덮어쓸 수 있다.

**영향**
임의 파일 쓰기/덮어쓰기, 경로 탐색.

**조치**

- `Path.GetFullPath`로 정규화 후 허용 루트(base dir) 하위인지 검증.
- 검증 실패 시 쓰기 거부 + 로깅.

---

### 6. 레거시 P/Invoke INI + 고정 255 버퍼

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Config/Config.cs:19-24` — `WritePrivateProfileString`/`GetPrivateProfileString`
- `FrameworkCommon/Config/Config.cs:62-63` — `new StringBuilder(255)`

**문제**
255자 고정 버퍼로 값이 조용히 잘린다(보안 설정값이 잘리면 의도치 않은 동작). 레거시 Win32 API 의존.

**영향**
설정값 손실로 인한 오동작.

**조치**

- UTF-8 버전(`INI_UTF8`)으로 일원화하거나 버퍼 크기 동적 확장.

---

## 🟡 Low

### 7. Log 클래스의 락 불일치 (경합 조건)

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Logger/Log.cs:177` — `lock(this.LogBuffer)`
- `FrameworkCommon/Logger/Log.cs:207` — `lock(this.LogBufferLock)`
- `FrameworkCommon/Logger/Log.cs:237` — 락 보유 중 `this.Exception(ex)` 재진입

**문제**
같은 큐(`LogBuffer`)를 서로 다른 락으로 보호하여 Enqueue/Dequeue 동시 실행 시 큐 손상 가능. 또한 락을 잡은 채 `Exception`을 재호출해 자기재귀 위험.

**영향**
로그 큐 손상, 잠재적 데드락.

**조치**

- 큐 보호 락을 하나로 통일.
- catch 블록에서 락 재획득 경로 제거.

---

> 결함 8은 **핸들러 객체 공유**와 **Dictionary 비동기화 접근**이 원인·수정 방식이 다른 독립 버그이므로 8-1·8-2로 분리한다.

### 8-1. TcpMultiServer 핸들러 객체 공유

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Comm/Tcp/TcpMultiServer.cs:52,58` — 모든 접속에 동일한 `ttc` 인스턴스 재사용

**문제**
접속할 때마다 동일한 `ttc` 인스턴스를 `SetClient`로 덮어쓴다. 다중 클라이언트가 하나의 핸들러/소켓 상태를 공유하게 되어 서로의 연결을 망가뜨린다.

**영향**
다중 클라이언트 환경에서 데이터 혼선, 연결 손상.

**조치**

- 접속마다 새 핸들러 인스턴스를 생성해 클라이언트별로 격리.

---

### 8-2. Clients Dictionary 비동기화 접근 (경합)

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Comm/Tcp/TcpMultiServer.cs:82-98` — `Clients` Dictionary 락 없이 접근

**문제**
`Clients` Dictionary를 Accept 스레드(추가)와 `Send`/`Disconnect`(조회/삭제)에서 락 없이 동시 접근한다.

**영향**
경합 조건으로 인한 예외, 컬렉션 상태 손상.

**조치**

- `Clients` 접근을 락으로 보호하거나 `ConcurrentDictionary`로 교체.

---

### 9. ByteConverter 정수 오버플로 가능성

- [ ] 수정 완료

**위치**

- `FrameworkCommon/Converter/ByteConverter.cs:24`

**문제**
`buffer.Length < length + offset` 검사에서 `length + offset`이 큰 값이면 오버플로로 음수가 되어 경계검사를 우회한 뒤 `Array.Copy`가 예외를 던진다.

**영향**
경계검사 우회, 예외 유발.

**조치**

- `length`/`offset` 음수·오버플로 가드 추가 (개별 검사 또는 `long` 비교).

---

### 10. `Encoding.Default` 사용

- [x] 수정 완료 (2026-06-16)

**위치**

- `FrameworkCommon/Converter/ByteConverter.cs` — `ToString(byte[])`

**문제**
`Encoding.Default`는 의도가 불명확하고(플랫폼/런타임 의존처럼 읽힘) 같은 클래스의 `ToBytes`(UTF-8)와 불일치. (참고: .NET10에서는 `Encoding.Default`가 실제로는 UTF-8을 반환하므로 현 런타임 기능 버그는 아니나, 명시성·일관성·보장성을 위해 수정.)

**영향**
의도 혼동, 인코딩 가정 불일치 위험.

**조치 (적용 완료)**

- `Encoding.Default.GetString` → `Encoding.UTF8.GetString` 명시.
- 저장소 기본 인코딩 정책(UTF-8 = MySQL `utf8mb4` 동등)을 `AGENTS.md` 7장 + 루트 `.editorconfig`(`charset = utf-8`, 모든 파일)에 등록.
- 테스트: `FrameworkCommonTest/ByteConverterTest.cs` (3종: 한글 UTF-8 바이트, 한글/ASCII 왕복, 4바이트 이모지 왕복).

---

### 11. Newtonsoft.Json 의존성 제거 → System.Text.Json 이관

- [x] 수정 완료 (2026-06-16) — 외부 의존성 제거

**위치**

- `FrameworkCommon/DTO/Dto.cs` — (구) `JsonConvert.SerializeObject`
- `FrameworkCommon/Framework.Common.csproj` — (구) `PackageReference Newtonsoft.Json`

**문제**
외부 의존성(`Newtonsoft.Json`)은 공급망/버전 관리 부담(예: 구버전 깊은 중첩 JSON DoS CVE-2024-21907)을 동반한다. .NET10 내장 `System.Text.Json`으로 충분히 대체 가능.

**영향**
외부 의존성 유지보수·취약점 노출.

**조치 (적용 완료)**

- `Newtonsoft.Json` PackageReference 제거, `using` 제거, `Release/Newtonsoft.Json.dll` 정리.
- `Dto.ToJson()`을 `System.Text.Json.JsonSerializer`로 이관.
- **기존 출력 호환 유지** (검증 완료):
  - `Encoder = UnsafeRelaxedJsonEscaping` → 한글·`'`·`<`·`>`·`&`를 `\uXXXX`로 이스케이프하지 않고 그대로 출력 (Newtonsoft 동일).
  - `JsonSerializer.Serialize(this, this.GetType(), ...)` → 추상 기반 타입이 아닌 **런타임 실제 타입** 기준 직렬화 (Newtonsoft 동일).
  - `WriteIndented = true` → 2칸 들여쓰기 + CRLF(`\r\n`). STJ .NET10 기본값이 Newtonsoft Windows 출력과 동일함을 테스트로 확인.
  - 큰따옴표 `"`는 양쪽 모두 `\"` 이스케이프(JSON 규약).
- 테스트: `FrameworkCommonTest/DtoJsonTest.cs` (6종, 전부 통과).
- 참고: 역직렬화는 미사용(`IJson.SetJson` 구현체 없음). 추후 구현 시 STJ는 기본 안전(다형 역직렬화 비활성).

---

## 참고

- 가장 시급한 항목은 **1·2·3**번 (네트워크에서 외부 공격자가 인증 없이 직접 트리거 가능).
- 각 항목 수정 시 상단 요약표의 상태 칸과 해당 섹션 체크박스를 함께 갱신할 것.
