namespace SqlStreamStore.FSharp.Types

[<RequireQualifiedAccess>]
type StreamMessageId =
    | Auto
    | Custom of System.Guid

type MessageDetails =
    { id: StreamMessageId
      type': string
      jsonData: string
      jsonMetadata: string }

[<RequireQualifiedAccess>]
type AppendVersion =
    | Any
    | EmptyStream
    | NoStream
    | SpecificVersion of int

[<RequireQualifiedAccess>]
type ReadVersion =
    | Any
    | SpecificVersion of uint

[<RequireQualifiedAccess>]
type StartPosition =
    | Any
    | SpecificPosition of int64
