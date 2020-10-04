namespace SqlStreamStore.FSharp


[<RequireQualifiedAccessAttribute>]
type Version =
    | None
    | Any
    | EmptyStream
    | NoStream
    | SpecificVersion of int

type StreamDetails =
    { streamName: string
      startPosition: int option
      version: Version }
