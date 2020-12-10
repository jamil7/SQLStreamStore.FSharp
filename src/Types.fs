namespace SqlStreamStore.FSharp

[<RequireQualifiedAccessAttribute>]
type StreamMessageId =
    | Custom of System.Guid
    | Auto

type MessageDetails =
    { id: StreamMessageId
      type': string
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
    | Any
    | SpecificVersion of uint

[<RequireQualifiedAccessAttribute>]
type StartPosition =
    | Any
    | SpecificPosition of int64
