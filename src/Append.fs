namespace SqlStreamStore.FSharp

open FSharp.Prelude
open System.Threading
open SqlStreamStore.Streams

[<RequireQualifiedAccess>]
type AppendOption =
    | CancellationToken of CancellationToken
    | ExpectedVersion of int

module Append =
    let streamMessages'
        (messages: NewStreamMessage list)
        (appendOptions: AppendOption list)
        : Stream -> AsyncResult<AppendResult, exn> =

        let mutable expectedVersion = ExpectedVersion.Any
        let mutable cancellationToken = Unchecked.defaultof<CancellationToken>

        appendOptions
        |> List.iter
            (function
            | AppendOption.CancellationToken token -> cancellationToken <- token
            | AppendOption.ExpectedVersion version -> expectedVersion <- version)

        fun (Stream stream) ->
            stream.store.AppendToStream(stream.streamId, expectedVersion, List.toArray messages, cancellationToken)

    let streamMessages (messages: NewStreamMessage list) : Stream -> AsyncResult<AppendResult, exn> =
        streamMessages' messages []
