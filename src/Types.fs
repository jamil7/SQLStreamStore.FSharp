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

[<RequireQualifiedAccessAttribute>]
type ReadVersion =
    | Start
    | End
    | SpecificVersion of uint

[<RequireQualifiedAccessAttribute>]
type StartPosition =
    | Start
    | End
    | SpecificPosition of int64

[<RequireQualifiedAccessAttribute>]
type ReadingDirection =
    | Forward
    | Backward
