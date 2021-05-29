namespace SqlStreamStore.FSharp

open FSharp.Prelude
open System.Threading
open SqlStreamStore

[<RequireQualifiedAccess>]
type AppendOption =
    | CancellationToken of CancellationToken
    | ExpectedVersion of int

module Append =
    let streamMessages'
        (messages: NewStreamMessage list)
        (appendOptions: AppendOption list)
        (Stream stream: Stream)
        : AsyncResult<Streams.AppendResult, exn> =

        let mutable expectedVersion = Streams.ExpectedVersion.Any
        let mutable cancellationToken = Unchecked.defaultof<CancellationToken>

        appendOptions
        |> List.iter
            (function
            | AppendOption.CancellationToken token -> cancellationToken <- token
            | AppendOption.ExpectedVersion version -> expectedVersion <- version)

        let messages' =
            List.map NewStreamMessage.toOriginalNewStreamMessage messages

        stream.store.AppendToStream(stream.streamId, expectedVersion, List.toArray messages', cancellationToken)

    let streamMessages (messages: NewStreamMessage list) : Stream -> AsyncResult<Streams.AppendResult, exn> =
        streamMessages' messages []
