# Framework.Common

`Framework.Common` is a `.NET 10` common library that provides communication, packet handling, configuration, DTO models, coordinate systems, unit conversion, logging, and thread management utilities.

## Project Structure

```text
Lib.Net10/
|-- Framework.Net10.sln
|-- FrameworkCommon/                    # Common library
|   |-- Bootup/                         # Placeholder for initialization features
|   |-- Buffer/                         # Byte and generic buffers
|   |-- Comm/
|   |   |-- Protocol/                   # Packet abstraction
|   |   |   `-- Beacon/                 # Beacon packets
|   |   |-- Serial/                     # Extension point for serial communication
|   |   `-- Tcp/                        # TCP clients and servers
|   |-- Config/                         # INI configuration read/write
|   |-- Controller/                     # Extension point for remote control
|   |-- Converter/                      # Byte, bit, and string conversion
|   |-- CoordinateSystem/               # 2D/3D coordinates and motion section calculation
|   |-- DataBase/Mdb/                   # Extension point for database support
|   |-- DTO/                            # DTO and limit-value models
|   |-- Enum/                           # Common enumerations
|   |-- Error/                          # Common error models
|   |-- Event/                          # Event argument models
|   |-- File/                           # File existence utility
|   |-- Language/                       # Language pack models
|   |-- Logger/                         # File and remote logging
|   |-- Motion/                         # Speed and velocity calculation
|   |-- Singleton/                      # Singleton base classes
|   |-- ThreadManager/                  # Schedulers, threads, and timers
|   `-- Unit/
|       |-- Range/                      # Range validation
|       `-- SI/                         # SI units and metric prefix calculation
|-- FrameworkCommonTest/                # MSTest unit tests
`-- FrameworkCommonRunningTest/         # Manual runtime tests
```

Build:

```powershell
dotnet build Framework.Net10.sln
dotnet test FrameworkCommonTest/Framework.Test.Unit.Common.csproj
```

## API Overview

The methods in the following tables are organized around constructors and major public methods. Repeated numeric overloads are grouped into single entries.

### Basic Features

| Namespace                    | Class             | Method                                                                             | Description                                                                                       |
| ---------------------------- | ----------------- | ---------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------- |
| `Framework.Common`           | `Bootup`          | `Bootup()`                                                                         | Placeholder for initialization features. Currently throws `NotImplementedException` when created. |
| `Framework.Common`           | `LanguagePack`    | `SetLanguage(string)`                                                              | Sets the language identifier to use.                                                              |
| `Framework.Common`           | `LanguageStorage` | `SetLangFile(string, string)`                                                      | Maps a language identifier to a language file path.                                               |
| `Framework.Common.Buffer`    | `Buffer<T>`       | `Clear()`, `Add(T)`, `AddRange(...)`, `RemoveAt(int)`, `Remove(T)`, `RemoveAll(T)` | Manages a generic `List<T>`-based buffer.                                                         |
| `Framework.Common.Buffer`    | `PacketBuffer`    | `GetBytes()`                                                                       | Returns the contents of the packet buffer as a `byte[]`.                                          |
| `Framework.Common.Error`     | `Error`           | `SetReason(string)`, `Clear()`                                                     | Stores and clears an error reason string.                                                         |
| `Framework.Common.Files`     | `ForcesFileExist` | `ForcesFileExist(string, FileType)`                                                | Creates a specified file if it does not exist, based on the requested file type.                  |
| `Framework.Common.Files`     | `TextFileHandler` | -                                                                                  | Type reserved for extending text file handling.                                                   |
| `Framework.Common.Motion`    | `Motion`          | `SetSpeed(int, int)`, `SetVelocity(int, int)`                                      | Calculates speed or velocity using distance/displacement and time.                                |
| `Framework.Common.Singleton` | `Singleton<T>`    | `Ins`, `Constructor()`                                                             | Provides a lazily initialized singleton base class.                                               |
| `Framework.Common.Singleton` | `ISingleton<T>`   | -                                                                                  | Represents the contract for singleton types.                                                      |

### Conversion

| Namespace                    | Class             | Method                                                                                                                                                                                       | Description                                                                                                |
| ---------------------------- | ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| `Framework.Common.Converter` | `ByteConverter`   | `ToHexString(...)`                                                                                                                                                                           | Converts a byte array or list into a hexadecimal string.                                                   |
| `Framework.Common.Converter` | `ByteConverter`   | `ToBytes(string)`, `GetBytes(string)`                                                                                                                                                        | Converts a string into a UTF-8 byte array.                                                                 |
| `Framework.Common.Converter` | `ByteConverter`   | `ToString(byte[])`                                                                                                                                                                           | Converts a byte array into a UTF-8 string.                                                                 |
| `Framework.Common.Converter` | `ByteConverter`   | `ToBinaryString(byte)`                                                                                                                                                                       | Converts a byte into a binary string.                                                                      |
| `Framework.Common.Converter` | `BitManager`      | `GetBytes(int)`, `SetBitMask(int)`, `Clear()`                                                                                                                                                | Manages an integer storage value as a byte array or bit mask.                                              |
| `Framework.Common.Converter` | `BitManager`      | `PutBool(...)`, `PutInt8(...)`, `PutUInt8(...)`, `PutInt16(...)`, `PutUInt16(...)`, `PutInt32(...)`, `PutUInt32(...)`, `PutInt64(...)`, `PutUInt64(...)`                                     | Writes values into specified bit positions in the storage value.                                           |
| `Framework.Common.Converter` | `BitManager`      | `ExtractBool(...)`, `ExtractInt8(...)`, `ExtractUInt8(...)`, `ExtractInt16(...)`, `ExtractUInt16(...)`, `ExtractInt32(...)`, `ExtractUInt32(...)`, `ExtractInt64(...)`, `ExtractUInt64(...)` | Extracts values from specified bit ranges in the storage value.                                            |
| `Framework.Common.Converter` | `StringToNumbers` | `ToBool(string, ...)`                                                                                                                                                                        | Converts a string into a boolean value and returns a default value on failure.                             |
| `Framework.Common.Converter` | `StringToNumbers` | `ToSByte(...)`, `ToByte(...)`, `ToInt16(...)`, `ToUInt16(...)`, `ToInt32(...)`, `ToUInt32(...)`, `ToInt64(...)`, `ToFloat(...)`, `ToDouble(...)`, `ToDecimal(...)`                           | Converts strings into numeric values. Overloads with range parameters validate minimum and maximum bounds. |

### Configuration and DTO

| Namespace                 | Class                                                                                                                      | Method                                                                                                                                                                       | Description                                                                            |
| ------------------------- | -------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| `Framework.Common.Config` | `INI_UTF8`                                                                                                                 | `SetAutoFile(string)`, `SetAutoFile()`                                                                                                                                       | Stores the UTF-8 INI file path and sets it as the automatic file target.               |
| `Framework.Common.Config` | `INI_UTF8.Item`                                                                                                            | `SetValue(string)`                                                                                                                                                           | Changes the value of an INI item.                                                      |
| `Framework.Common.Config` | `IniConfig`                                                                                                                | `GetBool(...)`, `GetByte(...)`, `GetInt16(...)`, `GetUInt16(...)`, `GetInt32(...)`, `GetUInt32(...)`, `GetFloat(...)`, `GetDouble(...)`, `GetDecimal(...)`, `GetString(...)` | Reads typed values from INI sections and keys.                                         |
| `Framework.Common.Config` | `IniConfig`                                                                                                                | `SetBool(...)`, `SetByte(...)`, `SetInt16(...)`, `SetUInt16(...)`, `SetInt32(...)`, `SetUInt32(...)`, `SetFloat(...)`, `SetDouble(...)`, `SetDecimal(...)`, `SetString(...)` | Writes typed values into INI sections and keys.                                        |
| `Framework.Common.Config` | `IniConfig`                                                                                                                | `WriteFile(...)`, `ReadFile(...)`                                                                                                                                            | Reads and writes between DTOs and INI files.                                           |
| `Framework.Common.Config` | `IniError`                                                                                                                 | `Clear()`                                                                                                                                                                    | Manages INI processing error state.                                                    |
| `Framework.Common.DTO`    | `Dto`                                                                                                                      | `ToJson()`                                                                                                                                                                   | Serializes a DTO into an indented JSON string.                                         |
| `Framework.Common.DTO`    | `StringDto`                                                                                                                | -                                                                                                                                                                            | DTO that stores a string value.                                                        |
| `Framework.Common.DTO`    | `IniDto`                                                                                                                   | -                                                                                                                                                                            | Base class for implementing INI configuration DTOs.                                    |
| `Framework.Common.DTO`    | `Limit<T>`                                                                                                                 | `Set(...)`, `ToString()`                                                                                                                                                     | Manages current value, default value, and valid range for limit-value DTOs.            |
| `Framework.Common.DTO`    | `Limit_Number<T>`                                                                                                          | `SetScale(...)`, `SetLimit(...)`, `SetMax(...)`, `SetMin(...)`, `Set(...)`, `SetUV(...)`, `Clear()`                                                                          | Sets scale and range for numeric limits and clamps input values to the valid range.    |
| `Framework.Common.DTO`    | `Limit_Number<T>`                                                                                                          | `ScaledValue(...)`, `ToScaleString(...)`, `To1000CommaString()`                                                                                                              | Returns scaled values and display strings.                                             |
| `Framework.Common.DTO`    | `Limit_Bool`                                                                                                               | `Set(...)`                                                                                                                                                                   | Manages boolean limit values.                                                          |
| `Framework.Common.DTO`    | `Limit_SByte`, `Limit_Byte`, `Limit_Int16`, `Limit_UInt16`, `Limit_Int32`, `Limit_UInt32`, `Limit_Double`, `Limit_Decimal` | `Set(...)`, `GetBytes()`, `ToString()`                                                                                                                                       | Provides limit values, serialization, and string representation for each numeric type. |
| `Framework.Common.DTO`    | `DTOException`                                                                                                             | Constructor                                                                                                                                                                  | Represents DTO processing errors.                                                      |
| `Framework.Common.DTO`    | `IJson`, `IGetBytes`, `IToHexString`, `ITo1000CommaString`, `ILimitData`, `IDto`, `IGetDTO`, `ISetDTO`                     | Interface members                                                                                                                                                            | Conversion and access contracts for DTO implementations.                               |

### Communication and Packets

| Namespace               | Class                     | Method                                                                                         | Description                                                                                                                                  |
| ----------------------- | ------------------------- | ---------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| `Framework.Common.Comm` | `Packet`                  | `GetBytes()`, `SetReceivedBytes(byte[])`, `Print()`                                            | Base class for packet generation, received-data parsing, and debug output.                                                                   |
| `Framework.Common.Comm` | `PacketError`             | `SetReason(PacketErrorReason)`                                                                 | Records packet error state and reason.                                                                                                       |
| `Framework.Common.Comm` | `PacketAddChecksum`       | -                                                                                              | Base class for implementing packets with checksums.                                                                                          |
| `Framework.Common.Comm` | `PacketOneWay`            | -                                                                                              | Base class for implementing one-way packets.                                                                                                 |
| `Framework.Common.Comm` | `PacketTwoWay`            | `ClearRcv()`, `SetRcvData(...)`, `MatchData()`                                                 | Manages the receive buffer for request-response packet matching.                                                                             |
| `Framework.Common.Comm` | `Framework_Beacon_Packet` | Constructor                                                                                    | Common base class for Beacon protocol packets.                                                                                               |
| `Framework.Common.Comm` | `Beacon_Packet`           | `CheckMagicCode(...)`, `SetReceivedBytes(byte[])`                                              | Performs Beacon magic-code checks, header validation, and body parsing.                                                                      |
| `Framework.Common.Comm` | `TCPClient`               | `DebugModeOn()`, `SetLocalLogging()`, `Open(...)`, `OpenAsync(...)`, `Close()`, `CloseAsync()` | Base TCP client using asynchronous send/receive queues. Synchronous methods are preserved for compatibility with existing callers.           |
| `Framework.Common.Comm` | `TCPClient`               | `SendFirst(byte[])`, `Send(byte[])`, `Send(string)`, `SendAdd(byte[])`                         | Sends data through immediate, priority, and normal send queues.                                                                              |
| `Framework.Common.Comm` | `PowerTCPClient`          | Constructor                                                                                    | `TCPClient` implementation for power-control communication.                                                                                  |
| `Framework.Common.Comm` | `TcpTaskClient`           | `SetClient(...)`, `Send(...)`, `Close()`                                                       | Manages send, receive, and close operations for a `Task`-based TCP connection.                                                               |
| `Framework.Common.Comm` | `TcpSocketServer`         | `Open()`, `Send(...)`, `Close()`                                                               | Manages connection, send/receive, and termination for a single TCP socket.                                                                   |
| `Framework.Common.Comm` | `TcpListenerServer`       | `Open()`, `OpenAsync(...)`, `Send(...)`, `SendAsync(...)`, `Close()`, `CloseAsync()`           | Manages a cancelable asynchronous single-connection TCP listener. Synchronous methods are preserved for compatibility with existing callers. |
| `Framework.Common.Comm` | `TcpListenerServer2`      | `Listen()`, `DequeueRcvBuff()`, `Send(...)`, `Close()`                                         | Manages a single-connection TCP listener and receive buffer.                                                                                 |
| `Framework.Common.Comm` | `TcpSingleSocketServer`   | `Listening()`                                                                                  | Base class for extending single-socket servers.                                                                                              |
| `Framework.Common.Comm` | `TcpMultiServer`          | `Stop()`, `Disconnect(...)`, `Send(...)`, `BroadCast(...)`                                     | Manages multiple TCP client connections and sends data to selected or all clients.                                                           |

### Coordinates and Motion

| Namespace                           | Class                      | Method                                                                                                                           | Description                                                                              |
| ----------------------------------- | -------------------------- | -------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| `Framework.Common.CoordinateSystem` | `CartesianCoordinates`     | `ToPolarCoordinates()`                                                                                                           | Converts 2D Cartesian coordinates into polar coordinates.                                |
| `Framework.Common.CoordinateSystem` | `PolarCoordinates`         | `ToCartesian()`                                                                                                                  | Converts polar coordinates into 2D Cartesian coordinates.                                |
| `Framework.Common.CoordinateSystem` | `Cartesian3DCoordinates`   | `ToCylindrical()`, `ToSpherical()`                                                                                               | Converts 3D Cartesian coordinates into cylindrical or spherical coordinates.             |
| `Framework.Common.CoordinateSystem` | `SphericalCoordinate`      | `ToCylinderical()`, `TooCartesian3D()`                                                                                           | Converts spherical coordinates into cylindrical or 3D Cartesian coordinates.             |
| `Framework.Common.CoordinateSystem` | `CylindricalCoordinate`    | `ToSpherical()`, `TooCartesian3D()`                                                                                              | Converts cylindrical coordinates into spherical or 3D Cartesian coordinates.             |
| `Framework.Common.CoordinateSystem` | `GCS`                      | `SetLatitude(...)`, `SetLongitude(...)`, `SetAltitude(...)`, `ToOCS_3D()`                                                        | Stores latitude, longitude, and altitude and converts them into orthogonal coordinates.  |
| `Framework.Common.CoordinateSystem` | `OCS_3D`                   | Constructor                                                                                                                      | Stores a 3D position coordinate.                                                         |
| `Framework.Common.CoordinateSystem` | `OCS_3D_Velocity`          | Constructor                                                                                                                      | Stores a 3D velocity vector.                                                             |
| `Framework.Common.CoordinateSystem` | `MissileMovingSection`     | `SetAzimuth(...)`, `SetElevation(...)`, `RunningDistance(...)`, `RunningVelocity(...)`                                           | Calculates direction, distance, and velocity for a constant-acceleration motion section. |
| `Framework.Common.CoordinateSystem` | `MissileMovingSection`     | `CurrentLocation3D(...)`, `CurrentVelocity3D(...)`, `CurrentLocation3DmSecond(...)`, `CurrentVelocity3DmSecond(...)`, `Add(...)` | Calculates 3D position, velocity vector, and accumulated sections for a motion segment.  |
| `Framework.Common.CoordinateSystem` | `MissileMovingSectionItem` | Constructor                                                                                                                      | Concrete class representing a motion section item.                                       |
| `Framework.Common.CoordinateSystem` | `Earth`                    | -                                                                                                                                | Type reserved for earth-coordinate extensions.                                           |

### Units and Ranges

| Namespace                  | Class                                                                        | Method                                                                                 | Description                                                                          |
| -------------------------- | ---------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
| `Framework.Common.Unit`    | `Range`                                                                      | `Range(...)`, `ToString()`, `In(double, bool)`                                         | Stores minimum and maximum values and checks whether a value is inside the range.    |
| `Framework.Common.Unit`    | `RangeError`                                                                 | `SetMinError()`, `SetMaxError()`, `Clear()`                                            | Manages minimum and maximum range errors.                                            |
| `Framework.Common.Unit.SI` | `MetricPrefix`                                                               | `SetBase10(int)`, `SetSymbol(...)`, `operator +`, `operator -`                         | Manages base-10 exponent and symbol for SI prefixes and performs prefix operations.  |
| `Framework.Common.Unit.SI` | `SIUnitDefine`                                                               | `SetSymbol(string)`                                                                    | Stores symbol, name, physical quantity, and description for an SI unit.              |
| `Framework.Common.Unit.SI` | `SIUnit`                                                                     | `ExtractUnit(string)`, `ExtractOthers(...)`, `ConvertTimes(...)`, `ConvertDevide(...)` | Provides unit-string extraction and unit conversion for multiplication and division. |
| `Framework.Common.Unit.SI` | `SIUnit`                                                                     | `SecondToHertz()`, `HertzToSecond()`, `Equals(...)`                                    | Converts between seconds and hertz and compares unit equivalence.                    |
| `Framework.Common.Unit.SI` | `SIUnitBase`                                                                 | `FindUnit(string)`                                                                     | Finds a registered SI unit definition by symbol.                                     |
| `Framework.Common.Unit.SI` | `SIValue`                                                                    | `SetPrefix(...)`, `SetSIValue(string)`, `ToString()`                                   | Manages an SI value composed of unit, value, and prefix.                             |
| `Framework.Common.Unit.SI` | `SIValue`                                                                    | `operator +`, `operator -`, `operator *`, `operator /`, comparison operators           | Performs arithmetic and comparison operations on SI values.                          |
| `Framework.Common.Unit.SI` | `SIUnitException`, `SIUnitMismatchException`, `SIUnitCannotConvertException` | Constructor                                                                            | Represents SI unit mismatch and non-convertible unit errors.                         |

### Logging and Threads

| Namespace                        | Class                  | Method                                                                                              | Description                                                                    |
| -------------------------------- | ---------------------- | --------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| `Framework.Common.Logger`        | `Log`                  | `SetLogLevel(...)`, `Connect(...)`, `Stop()`                                                        | Manages log level, remote log server connection, and log-thread shutdown.      |
| `Framework.Common.Logger`        | `Log`                  | `Fatal(...)`, `Exception(...)`, `Error(...)`, `Warning(...)`, `Info(...)`, `Sim(...)`, `Debug(...)` | Enqueues level-based logs and writes them to `Log/SystemLog_yyyyMMdd.log`.     |
| `Framework.Common.Logger`        | `Log`                  | `CheckCreateLogDir(string)`                                                                         | Creates the log directory if it does not exist.                                |
| `Framework.Common.Logger`        | `LogItem`              | Constructor, `GetBytes()`                                                                           | Serializes log messages or deserializes them from byte arrays.                 |
| `Framework.Common.Logger`        | `BasicLogClient`       | Constructor                                                                                         | TCP client for remote log transmission.                                        |
| `Framework.Common.ThreadManager` | `Scheduler`            | `Start()`, `Wait()`, `Suspend()`, `Resume()`, `Stop()`                                              | Abstract scheduler that controls execution cycles.                             |
| `Framework.Common.ThreadManager` | `ManagedThread`        | `Start()`, `Suspend()`, `Resume()`, `Stop()`                                                        | Manages the execution state of a background thread.                            |
| `Framework.Common.ThreadManager` | `ParameterlizedThread` | Constructor                                                                                         | Base class for worker threads with parameters.                                 |
| `Framework.Common.ThreadManager` | `NoneParameterThread`  | Constructor                                                                                         | Base class for worker threads without parameters.                              |
| `Framework.Common.ThreadManager` | `TimerManager`         | `Start()`, `Suspend()`, `Resume()`, `Stop()`                                                        | Manages repeated tasks using `System.Threading.Timer`.                         |
| `Framework.Common.ThreadManager` | `Power3SecReceiver`    | `Store(short)`, `Clear()`, `Max()`                                                                  | Stores power values received over a time window and returns the maximum value. |

### Events and Enumerations

| Namespace                  | Type                                                                                                                                                                         | Description                                                   |
| -------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------- |
| `Framework.Common.Event`   | `ReceivedDataArgs`, `ConnectionStateArgs`, `ServerConnectionArgs`, `EventPingArgs`, `BeaconPacketEventArgs`, `DtoEventArgs`, `ValueChangedEventArgs<T>`, `ProgressEventAgrs` | Data types passed to communication, DTO, and progress events. |
| `Framework.Common.Enum`    | `LogType`                                                                                                                                                                    | Defines log levels.                                           |
| `Framework.Common.Enum`    | `DtoType`                                                                                                                                                                    | Defines DTO types.                                            |
| `Framework.Common.Enum`    | `OperationMode`                                                                                                                                                              | Defines operation modes.                                      |
| `Framework.Common`         | `Language`                                                                                                                                                                   | Defines supported language identifiers.                       |
| `Framework.Common.Comm`    | `PacketErrorReason`                                                                                                                                                          | Defines packet error reasons.                                 |
| `Framework.Common.Files`   | `FileType`                                                                                                                                                                   | Defines file types to create.                                 |
| `Framework.Common.Unit.SI` | `MPS`                                                                                                                                                                        | Defines SI metric prefix symbols.                             |

## Notes

* The target framework is `net10.0`.
* JSON serialization uses `Newtonsoft.Json 13.0.3`.
* `FrameworkCommon/SIAmpere.cs` and `FrameworkCommon/Unit/SI/BaseUnit/*.cs` are legacy SI unit prototypes. They are currently excluded from compilation in the project file.
* `Bootup`, `RemoteController`, `Earth`, `TextFileHandler`, `Serial`, and `DataBase/Mdb` are early structures reserved for future extension.
* `Framework.Common.CsharpCodingConventions.CodeArchitectureSampleClass` is an example of coding style and architecture format. It is not a runtime feature.
