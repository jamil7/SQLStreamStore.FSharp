namespace SqlStreamStore.FSharp

open System.Threading
open FSharp.Prelude
open SqlStreamStore
open SqlStreamStore.Streams

type private StreamData =
    {
        streamId: string
        store: IStreamStore
    }

type Stream = private Stream of StreamData

type StreamOptions = int

module Stream =
    let mutable readPage : ReadStreamPage option = None

    let private apply f = Option.map f readPage

    let connect streamId store =
        Stream { streamId = streamId; store = store }

    let messages =
        apply (fun page -> page.Messages |> Array.toList)

    let status = apply (fun page -> page.Status)

    let isEnd = apply (fun page -> page.IsEnd)

    let readDirection = apply (fun page -> page.ReadDirection)


[<RequireQualifiedAccess>]
type AppendOption =
    | ExpectedVersion of int
    | CancellationToken of CancellationToken

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
            | AppendOption.ExpectedVersion version -> expectedVersion <- version
            | AppendOption.CancellationToken token -> cancellationToken <- token)

        fun (Stream stream) ->
            stream.store.AppendToStream(stream.streamId, expectedVersion, List.toArray messages, cancellationToken)

    let streamMessages (messages: NewStreamMessage list) : Stream -> AsyncResult<AppendResult, exn> =
        streamMessages' messages []

[<RequireQualifiedAccess>]
type ReadOption =
    | ReadDirection of ReadDirection
    | FromVersionInclusive of uint
    | MessageCount of int
    | NoPrefetch
    | CancellationToken of CancellationToken

module Read =

    let partial' (readOptions: ReadOption list) : Stream -> AsyncResult<ReadStreamPage, exn> =
        fun (Stream stream) ->
            let mutable readDirection = ReadDirection.Forward
            let mutable prefetch = true
            let mutable messageCount = 1000
            let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
            let mutable fromVersionInclusive : int option = None

            let fromVersionInclusive' =
                match readDirection, fromVersionInclusive with
                | ReadDirection.Forward, None -> StreamVersion.Start
                | ReadDirection.Forward, Some index -> int index
                | ReadDirection.Backward, None -> StreamVersion.End
                | ReadDirection.Backward, Some index -> int index
                | _ -> failwith "Illegal ReadDirection enum."

            match readDirection with
            | ReadDirection.Forward ->
                stream.store.ReadStreamForwards(
                    stream.streamId,
                    fromVersionInclusive',
                    messageCount,
                    prefetch,
                    cancellationToken
                )
            | ReadDirection.Backward ->
                store.ReadStreamBackwards(StreamId(name), fromVersionInclusive', messageCount', prefetch')
            | _ -> failwith "Illegal ReadDirection enum."

module Test =
    let foo (store: IStreamStore) =
        store |> Stream.connect "name" |> fun a -> a
