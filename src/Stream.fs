namespace SqlStreamStore.FSharp

open System
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
    let connect streamId store =
        Stream { streamId = streamId; store = store }


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
type ReadPartialOption =
    | ReadDirection of ReadDirection
    | FromVersionInclusive of int
    | MessageCount of int
    | NoPrefetch
    | CancellationToken of CancellationToken

[<RequireQualifiedAccess>]
type ReadEntireOption =
    | ReadDirection of ReadDirection
    | FromVersionInclusive of int
    | NoPrefetch
    | CancellationToken of CancellationToken

module Read =

    let partial' (readOptions: ReadPartialOption list) : Stream -> AsyncResult<ReadStreamPage, exn> =
        let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
        let mutable fromVersionInclusive : int option = None
        let mutable messageCount = 1000
        let mutable prefetch = true
        let mutable readDirection = ReadDirection.Forward

        readOptions
        |> List.iter
            (function
            | ReadPartialOption.ReadDirection direction -> readDirection <- direction
            | ReadPartialOption.FromVersionInclusive version -> fromVersionInclusive <- Some version
            | ReadPartialOption.MessageCount count -> messageCount <- count
            | ReadPartialOption.NoPrefetch -> prefetch <- false
            | ReadPartialOption.CancellationToken token -> cancellationToken <- token)

        let fromVersionInclusive' =
            match readDirection, fromVersionInclusive with
            | ReadDirection.Forward, None -> StreamVersion.Start
            | ReadDirection.Forward, Some index -> index
            | ReadDirection.Backward, None -> StreamVersion.End
            | ReadDirection.Backward, Some index -> index
            | _ -> failwith "Illegal ReadDirection enum."

        fun (Stream stream) ->
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
                stream.store.ReadStreamBackwards(
                    stream.streamId,
                    fromVersionInclusive',
                    messageCount,
                    prefetch,
                    cancellationToken
                )
            | _ -> failwith "Illegal ReadDirection enum."

    let partial : Stream -> AsyncResult<ReadStreamPage, exn> = partial' []

    let entire' (readOptions: ReadEntireOption list) : Stream -> AsyncResult<ReadStreamPage, exn> =
        let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
        let mutable fromVersionInclusive : int option = None
        let mutable prefetch = true
        let mutable readDirection = ReadDirection.Forward

        readOptions
        |> List.iter
            (function
            | ReadEntireOption.ReadDirection direction -> readDirection <- direction
            | ReadEntireOption.FromVersionInclusive version -> fromVersionInclusive <- Some version
            | ReadEntireOption.NoPrefetch -> prefetch <- false
            | ReadEntireOption.CancellationToken token -> cancellationToken <- token)

        partial' [ ReadPartialOption.MessageCount Int32.MaxValue ]

    let entire : Stream -> AsyncResult<ReadStreamPage, exn> = entire' []

module Get =
    let private apply f (readStreamPage: AsyncResult<ReadStreamPage, exn>) =
        asyncResult {
            let! page = readStreamPage
            return f page
        }

    let messages =
        apply (fun page -> page.Messages |> Array.toList)

    let status = apply (fun page -> page.Status)

    let isEnd = apply (fun page -> page.IsEnd)

    let readDirection = apply (fun page -> page.ReadDirection)

    let streamId = apply (fun page -> page.StreamId)

    let fromStreamVersion =
        apply (fun page -> page.FromStreamVersion)

    let lastStreamPosition =
        apply (fun page -> page.LastStreamPosition)

    let nextStreamVersion =
        apply (fun page -> page.NextStreamVersion)


namespace SqlStreamStore.FSharp.EventSourcing

open FSharp.Prelude
open SqlStreamStore.FSharp
open SqlStreamStore.Streams

module Append =
    let streamEvents'
        (events: NewStreamEvent<'event> list)
        (appendOptions: AppendOption list)
        : Stream -> AsyncResult<AppendResult, exn> =
        Append.streamMessages' (List.map NewStreamEvent.toNewStreamMessage events) appendOptions

    let streamEvents (events: NewStreamEvent<'a> list) : Stream -> AsyncResult<AppendResult, exn> =
        streamEvents' events []

module Get =
    let events<'event> =
        Get.messages
        >> AsyncResult.map (
            List.filter (fun msg -> msg.Type.Contains "Event::")
            >> List.map StreamEvent.ofStreamMessage<'event>
        )
