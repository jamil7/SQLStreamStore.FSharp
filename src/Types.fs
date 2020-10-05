namespace SqlStreamStore.FSharp

[<RequireQualifiedAccessAttribute>]
type Version =
    | Any
    | EmptyStream
    | NoStream
    | SpecificVersion of int

type AppendStreamDetails =
    { streamName: string
      version: Version }

type ReadStreamDetails =
    { streamName: string
      startPosition: int }
