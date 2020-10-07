namespace SqlStreamStore.FSharp

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

module Helpers =
    let getVersion: AppendVersion -> int =
        function
        | AppendVersion.Any -> SqlStreamStore.Streams.ExpectedVersion.Any
        | AppendVersion.EmptyStream -> SqlStreamStore.Streams.ExpectedVersion.EmptyStream
        | AppendVersion.NoStream -> SqlStreamStore.Streams.ExpectedVersion.NoStream
        | AppendVersion.SpecificVersion version -> version
