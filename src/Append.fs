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
        (messages : NewStreamMessage list)
        (appendOptions : AppendOption list)
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

    let streamMessages (messages : NewStreamMessage list) : Stream -> AsyncResult<AppendResult, exn> =
        streamMessages' messages []

namespace SqlStreamStore.FSharp.EventSourcing

open FSharp.Prelude
open SqlStreamStore.FSharp
open SqlStreamStore.Streams

module Append =
    let streamEvents'
        (events : NewStreamEvent<'event> list)
        (appendOptions : AppendOption list)
        : Stream -> AsyncResult<AppendResult, exn> =

        fun stream ->
            List.traverseResultM NewStreamEvent.toNewStreamMessage events
            |> Async.singleton
            |> AsyncResult.bind (fun messages -> Append.streamMessages' messages appendOptions stream)

    let streamEvents (events : NewStreamEvent<'a> list) : Stream -> AsyncResult<AppendResult, exn> =
        streamEvents' events []
