# Framework.Common

`Framework.Common`은 통신, 패킷, 설정, DTO, 좌표계, 단위계, 로깅 및 스레드 관리 기능을 모은 `.NET 10` 공통 라이브러리입니다.

## 프로젝트 구성

```text
Lib.Net10/
|-- Framework.Net10.sln
|-- FrameworkCommon/                    # 공통 라이브러리
|   |-- Bootup/                         # 초기화 기능 자리표시자
|   |-- Buffer/                         # 바이트 및 일반 버퍼
|   |-- Comm/
|   |   |-- Protocol/                   # 패킷 추상화
|   |   |   `-- Beacon/                 # Beacon 패킷
|   |   |-- Serial/                     # 직렬 통신 확장 위치
|   |   `-- Tcp/                        # TCP 클라이언트 및 서버
|   |-- Config/                         # INI 설정 읽기 및 쓰기
|   |-- Controller/                     # 원격 제어 확장 위치
|   |-- Converter/                      # 바이트, 비트, 문자열 변환
|   |-- CoordinateSystem/               # 2D, 3D 좌표 및 이동 구간 계산
|   |-- DataBase/Mdb/                   # 데이터베이스 확장 위치
|   |-- DTO/                            # DTO 및 제한값 모델
|   |-- Enum/                           # 공통 열거형
|   |-- Error/                          # 공통 오류 모델
|   |-- Event/                          # 이벤트 인자 모델
|   |-- File/                           # 파일 존재 보장 유틸리티
|   |-- Language/                       # 언어팩 모델
|   |-- Logger/                         # 파일 및 원격 로깅
|   |-- Motion/                         # 속력 및 속도 계산
|   |-- Singleton/                      # 싱글턴 기반 클래스
|   |-- ThreadManager/                  # 스케줄러, 스레드, 타이머
|   `-- Unit/
|       |-- Range/                      # 범위 검사
|       `-- SI/                         # SI 단위 및 접두어 계산
|-- FrameworkCommonTest/                # MSTest 단위 테스트
`-- FrameworkCommonRunningTest/         # 수동 실행 테스트
```

빌드:

```powershell
dotnet build Framework.Net10.sln
dotnet test FrameworkCommonTest/Framework.Test.Unit.Common.csproj
```

## API 목록

표의 메서드는 생성자와 주요 공개 메서드를 기준으로 정리했습니다. 반복되는 숫자형 오버로드는 하나의 항목으로 묶었습니다.

### 기본 기능

| 네임스페이스 | 클래스 | 메서드 | 설명 |
|---|---|---|---|
| `Framework.Common` | `Bootup` | `Bootup()` | 초기화 기능을 위한 자리표시자입니다. 현재 생성 시 `NotImplementedException`이 발생합니다. |
| `Framework.Common` | `LanguagePack` | `SetLanguage(string)` | 사용할 언어 식별자를 설정합니다. |
| `Framework.Common` | `LanguageStorage` | `SetLangFile(string, string)` | 언어 식별자와 언어 파일 경로를 연결합니다. |
| `Framework.Common.Buffer` | `Buffer<T>` | `Clear()`, `Add(T)`, `AddRange(...)`, `RemoveAt(int)`, `Remove(T)`, `RemoveAll(T)` | `List<T>` 기반 범용 버퍼를 관리합니다. |
| `Framework.Common.Buffer` | `PacketBuffer` | `GetBytes()` | 패킷 버퍼 내용을 `byte[]`로 반환합니다. |
| `Framework.Common.Error` | `Error` | `SetReason(string)`, `Clear()` | 오류 원인 문자열을 저장하고 초기화합니다. |
| `Framework.Common.Files` | `ForcesFileExist` | `ForcesFileExist(string, FileType)` | 지정한 파일이 없으면 파일 형식에 맞게 생성합니다. |
| `Framework.Common.Files` | `TextFileHandler` | - | 텍스트 파일 처리를 확장하기 위한 형식입니다. |
| `Framework.Common.Motion` | `Motion` | `SetSpeed(int, int)`, `SetVelocity(int, int)` | 이동 거리 또는 변위와 시간을 이용해 속력과 속도를 계산합니다. |
| `Framework.Common.Singleton` | `Singleton<T>` | `Ins`, `Constructor()` | 지연 생성 싱글턴 인스턴스를 제공하는 기반 클래스입니다. |
| `Framework.Common.Singleton` | `ISingleton<T>` | - | 싱글턴 형식 계약을 나타냅니다. |

### 변환

| 네임스페이스 | 클래스 | 메서드 | 설명 |
|---|---|---|---|
| `Framework.Common.Converter` | `ByteConverter` | `ToHexString(...)` | 바이트 배열 또는 목록을 16진수 문자열로 변환합니다. |
| `Framework.Common.Converter` | `ByteConverter` | `ToBytes(string)`, `GetBytes(string)` | 문자열을 UTF-8 바이트 배열로 변환합니다. |
| `Framework.Common.Converter` | `ByteConverter` | `ToString(byte[])` | 바이트 배열을 UTF-8 문자열로 변환합니다. |
| `Framework.Common.Converter` | `ByteConverter` | `ToBinaryString(byte)` | 바이트를 2진수 문자열로 변환합니다. |
| `Framework.Common.Converter` | `BitManager` | `GetBytes(int)`, `SetBitMask(int)`, `Clear()` | 정수 저장소를 바이트 배열 또는 지정 비트 마스크로 관리합니다. |
| `Framework.Common.Converter` | `BitManager` | `PutBool(...)`, `PutInt8(...)`, `PutUInt8(...)`, `PutInt16(...)`, `PutUInt16(...)`, `PutInt32(...)`, `PutUInt32(...)`, `PutInt64(...)`, `PutUInt64(...)` | 저장소의 지정 비트 위치에 값을 기록합니다. |
| `Framework.Common.Converter` | `BitManager` | `ExtractBool(...)`, `ExtractInt8(...)`, `ExtractUInt8(...)`, `ExtractInt16(...)`, `ExtractUInt16(...)`, `ExtractInt32(...)`, `ExtractUInt32(...)`, `ExtractInt64(...)`, `ExtractUInt64(...)` | 저장소의 지정 비트 구간에서 값을 추출합니다. |
| `Framework.Common.Converter` | `StringToNumbers` | `ToBool(string, ...)` | 문자열을 논리값으로 변환하고 실패 시 기본값을 반환합니다. |
| `Framework.Common.Converter` | `StringToNumbers` | `ToSByte(...)`, `ToByte(...)`, `ToInt16(...)`, `ToUInt16(...)`, `ToInt32(...)`, `ToUInt32(...)`, `ToInt64(...)`, `ToFloat(...)`, `ToDouble(...)`, `ToDecimal(...)` | 문자열을 숫자로 변환합니다. 범위가 포함된 오버로드는 최솟값과 최댓값을 검사합니다. |

### 설정과 DTO

| 네임스페이스 | 클래스 | 메서드 | 설명 |
|---|---|---|---|
| `Framework.Common.Config` | `INI_UTF8` | `SetAutoFile(string)`, `SetAutoFile()` | UTF-8 INI 파일 경로를 저장하고 자동 파일로 설정합니다. |
| `Framework.Common.Config` | `INI_UTF8.Item` | `SetValue(string)` | INI 항목의 값을 변경합니다. |
| `Framework.Common.Config` | `IniConfig` | `GetBool(...)`, `GetByte(...)`, `GetInt16(...)`, `GetUInt16(...)`, `GetInt32(...)`, `GetUInt32(...)`, `GetFloat(...)`, `GetDouble(...)`, `GetDecimal(...)`, `GetString(...)` | INI 섹션과 키에서 형식화된 값을 읽습니다. |
| `Framework.Common.Config` | `IniConfig` | `SetBool(...)`, `SetByte(...)`, `SetInt16(...)`, `SetUInt16(...)`, `SetInt32(...)`, `SetUInt32(...)`, `SetFloat(...)`, `SetDouble(...)`, `SetDecimal(...)`, `SetString(...)` | INI 섹션과 키에 형식화된 값을 기록합니다. |
| `Framework.Common.Config` | `IniConfig` | `WriteFile(...)`, `ReadFile(...)` | DTO와 INI 파일 사이의 읽기 및 쓰기를 수행합니다. |
| `Framework.Common.Config` | `IniError` | `Clear()` | INI 처리 오류 상태를 관리합니다. |
| `Framework.Common.DTO` | `Dto` | `ToJson()` | DTO를 들여쓰기된 JSON 문자열로 직렬화합니다. |
| `Framework.Common.DTO` | `StringDto` | - | 문자열 값을 보관하는 DTO입니다. |
| `Framework.Common.DTO` | `IniDto` | - | INI 설정 DTO를 구현하기 위한 기반 클래스입니다. |
| `Framework.Common.DTO` | `Limit<T>` | `Set(...)`, `ToString()` | 제한값 DTO의 현재값, 기본값 및 유효 범위를 관리합니다. |
| `Framework.Common.DTO` | `Limit_Number<T>` | `SetScale(...)`, `SetLimit(...)`, `SetMax(...)`, `SetMin(...)`, `Set(...)`, `SetUV(...)`, `Clear()` | 숫자형 제한값의 배율과 범위를 설정하고 입력값을 유효 범위로 제한합니다. |
| `Framework.Common.DTO` | `Limit_Number<T>` | `ScaledValue(...)`, `ToScaleString(...)`, `To1000CommaString()` | 배율이 적용된 값과 표시용 문자열을 반환합니다. |
| `Framework.Common.DTO` | `Limit_Bool` | `Set(...)` | 논리형 제한값을 관리합니다. |
| `Framework.Common.DTO` | `Limit_SByte`, `Limit_Byte`, `Limit_Int16`, `Limit_UInt16`, `Limit_Int32`, `Limit_UInt32`, `Limit_Double`, `Limit_Decimal` | `Set(...)`, `GetBytes()`, `ToString()` | 각 숫자형의 제한값, 직렬화 및 문자열 표현을 제공합니다. |
| `Framework.Common.DTO` | `DTOException` | 생성자 | DTO 처리 오류를 나타냅니다. |
| `Framework.Common.DTO` | `IJson`, `IGetBytes`, `IToHexString`, `ITo1000CommaString`, `ILimitData`, `IDto`, `IGetDTO`, `ISetDTO` | 인터페이스 멤버 | DTO 구현에서 사용할 변환 및 접근 계약입니다. |

### 통신과 패킷

| 네임스페이스 | 클래스 | 메서드 | 설명 |
|---|---|---|---|
| `Framework.Common.Comm` | `Packet` | `GetBytes()`, `SetReceivedBytes(byte[])`, `Print()` | 패킷 생성, 수신 데이터 파싱 및 디버그 출력을 위한 기반 클래스입니다. |
| `Framework.Common.Comm` | `PacketError` | `SetReason(PacketErrorReason)` | 패킷 오류 상태와 원인을 기록합니다. |
| `Framework.Common.Comm` | `PacketAddChecksum` | - | 체크섬이 포함된 패킷을 구현하기 위한 기반 클래스입니다. |
| `Framework.Common.Comm` | `PacketOneWay` | - | 단방향 패킷을 구현하기 위한 기반 클래스입니다. |
| `Framework.Common.Comm` | `PacketTwoWay` | `ClearRcv()`, `SetRcvData(...)`, `MatchData()` | 요청과 응답을 연결하는 양방향 패킷 수신 버퍼를 관리합니다. |
| `Framework.Common.Comm` | `Framework_Beacon_Packet` | 생성자 | Beacon 프로토콜 공통 패킷 기반 클래스입니다. |
| `Framework.Common.Comm` | `Beacon_Packet` | `CheckMagicCode(...)`, `SetReceivedBytes(byte[])` | Beacon 매직 코드 검사, 헤더 검증 및 본문 파싱을 수행합니다. |
| `Framework.Common.Comm` | `TCPClient` | `DebugModeOn()`, `SetLocalLogging()`, `Open(...)`, `OpenAsync(...)`, `Close()`, `CloseAsync()` | 비동기 송수신 큐를 사용하는 TCP 클라이언트 기반 클래스입니다. 동기 메서드는 기존 호출부 호환을 위해 유지됩니다. |
| `Framework.Common.Comm` | `TCPClient` | `SendFirst(byte[])`, `Send(byte[])`, `Send(string)`, `SendAdd(byte[])` | 즉시, 우선 및 일반 송신 큐에 데이터를 전달합니다. |
| `Framework.Common.Comm` | `PowerTCPClient` | 생성자 | 전원 제어 통신을 위한 `TCPClient` 구현입니다. |
| `Framework.Common.Comm` | `TcpTaskClient` | `SetClient(...)`, `Send(...)`, `Close()` | `Task` 기반 TCP 연결의 송수신과 종료를 관리합니다. |
| `Framework.Common.Comm` | `TcpSocketServer` | `Open()`, `Send(...)`, `Close()` | 단일 TCP 소켓의 연결, 송수신 및 종료를 관리합니다. |
| `Framework.Common.Comm` | `TcpListenerServer` | `Open()`, `OpenAsync(...)`, `Send(...)`, `SendAsync(...)`, `Close()`, `CloseAsync()` | 취소 가능한 비동기 방식으로 단일 연결 TCP 리스너를 관리합니다. 동기 메서드는 기존 호출부 호환을 위해 유지됩니다. |
| `Framework.Common.Comm` | `TcpListenerServer2` | `Listen()`, `DequeueRcvBuff()`, `Send(...)`, `Close()` | 단일 연결 TCP 리스너와 수신 버퍼를 관리합니다. |
| `Framework.Common.Comm` | `TcpSingleSocketServer` | `Listening()` | 단일 소켓 서버를 확장하기 위한 기반 클래스입니다. |
| `Framework.Common.Comm` | `TcpMultiServer` | `Stop()`, `Disconnect(...)`, `Send(...)`, `BroadCast(...)` | 여러 TCP 클라이언트 연결을 관리하고 지정 또는 전체 클라이언트에 데이터를 전송합니다. |

### 좌표와 이동

| 네임스페이스 | 클래스 | 메서드 | 설명 |
|---|---|---|---|
| `Framework.Common.CoordinateSystem` | `CartesianCoordinates` | `ToPolarCoordinates()` | 2D 직교 좌표를 극좌표로 변환합니다. |
| `Framework.Common.CoordinateSystem` | `PolarCoordinates` | `ToCartesian()` | 극좌표를 2D 직교 좌표로 변환합니다. |
| `Framework.Common.CoordinateSystem` | `Cartesian3DCoordinates` | `ToCylindrical()`, `ToSpherical()` | 3D 직교 좌표를 원통 또는 구면 좌표로 변환합니다. |
| `Framework.Common.CoordinateSystem` | `SphericalCoordinate` | `ToCylinderical()`, `TooCartesian3D()` | 구면 좌표를 원통 또는 3D 직교 좌표로 변환합니다. |
| `Framework.Common.CoordinateSystem` | `CylindricalCoordinate` | `ToSpherical()`, `TooCartesian3D()` | 원통 좌표를 구면 또는 3D 직교 좌표로 변환합니다. |
| `Framework.Common.CoordinateSystem` | `GCS` | `SetLatitude(...)`, `SetLongitude(...)`, `SetAltitude(...)`, `ToOCS_3D()` | 위도, 경도, 고도를 보관하고 직교 좌표로 변환합니다. |
| `Framework.Common.CoordinateSystem` | `OCS_3D` | 생성자 | 3차원 위치 좌표를 보관합니다. |
| `Framework.Common.CoordinateSystem` | `OCS_3D_Velocity` | 생성자 | 3차원 속도 벡터를 보관합니다. |
| `Framework.Common.CoordinateSystem` | `MissileMovingSection` | `SetAzimuth(...)`, `SetElevation(...)`, `RunningDistance(...)`, `RunningVelocity(...)` | 일정 가속도 이동 구간의 방향, 거리 및 속도를 계산합니다. |
| `Framework.Common.CoordinateSystem` | `MissileMovingSection` | `CurrentLocation3D(...)`, `CurrentVelocity3D(...)`, `CurrentLocation3DmSecond(...)`, `CurrentVelocity3DmSecond(...)`, `Add(...)` | 이동 구간의 3D 위치와 속도 벡터 및 누적 구간을 계산합니다. |
| `Framework.Common.CoordinateSystem` | `MissileMovingSectionItem` | 생성자 | 이동 구간 항목을 나타내는 구체 클래스입니다. |
| `Framework.Common.CoordinateSystem` | `Earth` | - | 지구 좌표 관련 확장을 위한 형식입니다. |

### 단위와 범위

| 네임스페이스 | 클래스 | 메서드 | 설명 |
|---|---|---|---|
| `Framework.Common.Unit` | `Range` | `Range(...)`, `ToString()`, `In(double, bool)` | 최솟값과 최댓값을 보관하고 값의 범위 포함 여부를 검사합니다. |
| `Framework.Common.Unit` | `RangeError` | `SetMinError()`, `SetMaxError()`, `Clear()` | 범위 최솟값과 최댓값 오류를 관리합니다. |
| `Framework.Common.Unit.SI` | `MetricPrefix` | `SetBase10(int)`, `SetSymbol(...)`, `operator +`, `operator -` | SI 접두어의 10진 지수와 기호를 관리하고 접두어 연산을 수행합니다. |
| `Framework.Common.Unit.SI` | `SIUnitDefine` | `SetSymbol(string)` | SI 단위의 기호, 이름, 물리량 및 설명을 보관합니다. |
| `Framework.Common.Unit.SI` | `SIUnit` | `ExtractUnit(string)`, `ExtractOthers(...)`, `ConvertTimes(...)`, `ConvertDevide(...)` | 단위 문자열 추출 및 곱셈과 나눗셈 단위 변환을 제공합니다. |
| `Framework.Common.Unit.SI` | `SIUnit` | `SecondToHertz()`, `HertzToSecond()`, `Equals(...)` | 초와 헤르츠 단위 변환 및 단위 동등성 비교를 수행합니다. |
| `Framework.Common.Unit.SI` | `SIUnitBase` | `FindUnit(string)` | 등록된 SI 단위 정의를 기호로 검색합니다. |
| `Framework.Common.Unit.SI` | `SIValue` | `SetPrefix(...)`, `SetSIValue(string)`, `ToString()` | 단위, 값 및 접두어가 결합된 SI 값을 관리합니다. |
| `Framework.Common.Unit.SI` | `SIValue` | `operator +`, `operator -`, `operator *`, `operator /`, 비교 연산자 | SI 값의 산술 연산과 비교를 수행합니다. |
| `Framework.Common.Unit.SI` | `SIUnitException`, `SIUnitMismatchException`, `SIUnitCannotConvertException` | 생성자 | SI 단위 불일치 및 변환 불가 오류를 나타냅니다. |

### 로깅과 스레드

| 네임스페이스 | 클래스 | 메서드 | 설명 |
|---|---|---|---|
| `Framework.Common.Logger` | `Log` | `SetLogLevel(...)`, `Connect(...)`, `Stop()` | 로그 수준, 원격 로그 서버 연결 및 로그 스레드 종료를 관리합니다. |
| `Framework.Common.Logger` | `Log` | `Fatal(...)`, `Exception(...)`, `Error(...)`, `Warning(...)`, `Info(...)`, `Sim(...)`, `Debug(...)` | 수준별 로그를 큐에 기록하고 `Log/SystemLog_yyyyMMdd.log` 파일로 저장합니다. |
| `Framework.Common.Logger` | `Log` | `CheckCreateLogDir(string)` | 로그 디렉터리가 없으면 생성합니다. |
| `Framework.Common.Logger` | `LogItem` | 생성자, `GetBytes()` | 로그 메시지를 직렬화하거나 바이트 배열에서 역직렬화합니다. |
| `Framework.Common.Logger` | `BasicLogClient` | 생성자 | 원격 로그 전송을 위한 TCP 클라이언트입니다. |
| `Framework.Common.ThreadManager` | `Scheduler` | `Start()`, `Wait()`, `Suspend()`, `Resume()`, `Stop()` | 실행 주기를 제어하는 추상 스케줄러입니다. |
| `Framework.Common.ThreadManager` | `ManagedThread` | `Start()`, `Suspend()`, `Resume()`, `Stop()` | 백그라운드 스레드의 실행 상태를 관리합니다. |
| `Framework.Common.ThreadManager` | `ParameterlizedThread` | 생성자 | 매개변수가 있는 작업 스레드 기반 클래스입니다. |
| `Framework.Common.ThreadManager` | `NoneParameterThread` | 생성자 | 매개변수가 없는 작업 스레드 기반 클래스입니다. |
| `Framework.Common.ThreadManager` | `TimerManager` | `Start()`, `Suspend()`, `Resume()`, `Stop()` | `System.Threading.Timer` 기반 반복 작업을 관리합니다. |
| `Framework.Common.ThreadManager` | `Power3SecReceiver` | `Store(short)`, `Clear()`, `Max()` | 일정 시간 동안 수신한 전원 값을 저장하고 최댓값을 반환합니다. |

### 이벤트와 열거형

| 네임스페이스 | 형식 | 설명 |
|---|---|---|
| `Framework.Common.Event` | `ReceivedDataArgs`, `ConnectionStateArgs`, `ServerConnectionArgs`, `EventPingArgs`, `BeaconPacketEventArgs`, `DtoEventArgs`, `ValueChangedEventArgs<T>`, `ProgressEventAgrs` | 통신, DTO, 진행률 이벤트에 전달되는 데이터 형식입니다. |
| `Framework.Common.Enum` | `LogType` | 로그 수준을 정의합니다. |
| `Framework.Common.Enum` | `DtoType` | DTO 종류를 정의합니다. |
| `Framework.Common.Enum` | `OperationMode` | 운용 모드를 정의합니다. |
| `Framework.Common` | `Language` | 지원 언어 식별자를 정의합니다. |
| `Framework.Common.Comm` | `PacketErrorReason` | 패킷 오류 원인을 정의합니다. |
| `Framework.Common.Files` | `FileType` | 생성할 파일 종류를 정의합니다. |
| `Framework.Common.Unit.SI` | `MPS` | SI 접두어 기호를 정의합니다. |

## 참고 사항

- 대상 프레임워크는 `net10.0`입니다.
- JSON 직렬화는 `Newtonsoft.Json 13.0.3`을 사용합니다.
- `FrameworkCommon/SIAmpere.cs`와 `FrameworkCommon/Unit/SI/BaseUnit/*.cs`는 이전 SI 단위 프로토타입입니다. 현재 프로젝트 파일에서 컴파일 대상에서 제외되어 있습니다.
- `Bootup`, `RemoteController`, `Earth`, `TextFileHandler`, `Serial`, `DataBase/Mdb`는 확장을 위한 초기 구조입니다.
- `Framework.Common.CsharpCodingConventions.CodeArchitectureSampleClass`는 코드 작성 형식 예제이며 런타임 기능은 아닙니다.
