namespace SqlStreamStore.FSharp

[<RequireQualifiedAccessAttribute>]
type StreamMessageId =
    | Custom of System.Guid
    | Auto

type MessageDetails =
    { id: StreamMessageId
      type_: string
      jsonData: string
      jsonMetadata: string }


[<RequireQualifiedAccessAttribute>]
type AppendVersion =
    | Any
    | EmptyStream
    | NoStream
    | SpecificVersion of int

type AppendStreamDetails =
    { streamName: string
      version: AppendVersion }

type ReadStreamDetails =
    { streamName: string
      version: ReadVersion }

and ReadVersion = uint

type StartPositionInclusive = int64
type MessageCount = int

[<RequireQualifiedAccessAttribute>]
type AppendException =
    | WrongExpectedVersion of System.Exception
    | Other of System.Exception

[<RequireQualifiedAccessAttribute>]
type ReadingDirection =
    | Forward
    | Backward
