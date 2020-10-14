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

type StreamName = string

[<RequireQualifiedAccessAttribute>]
type ReadVersion =
    | Start
    | End
    | SpecificVersion of uint

[<RequireQualifiedAccessAttribute>]
type AppendVersion =
    | Any
    | EmptyStream
    | NoStream
    | SpecificVersion of int

[<RequireQualifiedAccessAttribute>]
type StartPosition =
    | Start
    | End
    | SpecificPosition of int64

type MessageCount = int

[<RequireQualifiedAccessAttribute>]
type ReadingDirection =
    | Forward
    | Backward
