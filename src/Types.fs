namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

[<RequireQualifiedAccessAttribute>]
type Version =
    | Any
    | EmptyStream
    | NoStream
    | SpecificVersion of int

type StreamDetails =
    { streamName: string
      version: Version }

type AllStreamDetails =
    { streamName: string
      startPosition: int }

module Helpers =
    let toVersion: Version -> int =
        function
        | Version.Any -> ExpectedVersion.Any
        | Version.EmptyStream -> ExpectedVersion.EmptyStream
        | Version.NoStream -> ExpectedVersion.NoStream
        | Version.SpecificVersion version -> version
