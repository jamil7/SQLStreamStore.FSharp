namespace SqlStreamStore.FSharp

open Prelude.ErrorHandling
open System.Threading
open SqlStreamStore
open SqlStreamStore.Streams

[<RequireQualifiedAccess>]
type ReadPartialOption =
    | CancellationToken of CancellationToken
    | FromVersionInclusive of int
    | MessageCount of int
    | NoPrefetch
    | ReadForwards
    | ReadBackwards

[<RequireQualifiedAccess>]
type ReadEntireOption =
    | CancellationToken of CancellationToken
    | FromVersionInclusive of int
    | NoPrefetch
    | ReadForwards
    | ReadBackwards

[<RequireQualifiedAccess>]
type ReadAllOption =
    | CancellationToken of CancellationToken
    | FromPositionInclusive of int64
    | MessageCount of int
    | NoPrefetch
    | ReadForwards
    | ReadBackwards

module Read =

    let partial' (readOptions: ReadPartialOption list) (stream: Stream) : AsyncResult<ReadStreamPage, exn> =

        let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
        let mutable fromVersionInclusive: int option = None
        let mutable messageCount = 1000
        let mutable prefetch = true
        let mutable readDirection = ReadDirection.Forward

        readOptions
        |> List.iter
            (function
            | ReadPartialOption.CancellationToken token -> cancellationToken <- token
            | ReadPartialOption.FromVersionInclusive version -> fromVersionInclusive <- Some version
            | ReadPartialOption.MessageCount count -> messageCount <- count
            | ReadPartialOption.NoPrefetch -> prefetch <- false
            | ReadPartialOption.ReadForwards -> readDirection <- ReadDirection.Forward
            | ReadPartialOption.ReadBackwards -> readDirection <- ReadDirection.Backward)

        let fromVersionInclusive' =
            match readDirection, fromVersionInclusive with
            | ReadDirection.Backward, None -> StreamVersion.End
            | ReadDirection.Backward, Some index -> index
            | ReadDirection.Forward, None -> StreamVersion.Start
            | ReadDirection.Forward, Some index -> index
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
            stream.store.ReadStreamBackwards(
                stream.streamId,
                fromVersionInclusive',
                messageCount,
                prefetch,
                cancellationToken
            )
        | _ -> failwith "Illegal ReadDirection enum."

    let partial: Stream -> AsyncResult<ReadStreamPage, exn> = partial' []

    let entire' (readOptions: ReadEntireOption list) : Stream -> AsyncResult<ReadStreamPage, exn> =

        let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
        let mutable fromVersionInclusive: int option = None
        let mutable prefetch = true
        let mutable readDirection = ReadDirection.Forward

        readOptions
        |> List.iter
            (function
            | ReadEntireOption.CancellationToken token -> cancellationToken <- token
            | ReadEntireOption.FromVersionInclusive version -> fromVersionInclusive <- Some version
            | ReadEntireOption.NoPrefetch -> prefetch <- false
            | ReadEntireOption.ReadForwards -> readDirection <- ReadDirection.Forward
            | ReadEntireOption.ReadBackwards -> readDirection <- ReadDirection.Backward)

        let options =
            [
                ReadPartialOption.MessageCount System.Int32.MaxValue
                ReadPartialOption.ReadForwards
            ]

        let options' =
            match fromVersionInclusive, prefetch with
            | Some version, true ->
                [
                    ReadPartialOption.FromVersionInclusive version
                ]
            | Some version, false ->
                [
                    ReadPartialOption.FromVersionInclusive version
                    ReadPartialOption.NoPrefetch
                ]
            | None, true -> []
            | None, false -> [ ReadPartialOption.NoPrefetch ]

        partial' (options @ options')

    let entire: Stream -> AsyncResult<ReadStreamPage, exn> = entire' []

    let allStream' (readOptions: ReadAllOption list) : IStreamStore -> AsyncResult<ReadAllPage, exn> =

        let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
        let mutable fromPositionInclusive: int64 option = None
        let mutable messageCount = 1000
        let mutable prefetch = true
        let mutable readDirection = ReadDirection.Forward

        readOptions
        |> List.iter
            (function
            | ReadAllOption.CancellationToken token -> cancellationToken <- token
            | ReadAllOption.FromPositionInclusive position -> fromPositionInclusive <- Some position
            | ReadAllOption.MessageCount count -> messageCount <- count
            | ReadAllOption.NoPrefetch -> prefetch <- false
            | ReadAllOption.ReadForwards -> readDirection <- ReadDirection.Forward
            | ReadAllOption.ReadBackwards -> readDirection <- ReadDirection.Backward)

        let fromPositionInclusive' =
            match readDirection, fromPositionInclusive with
            | ReadDirection.Backward, None -> Position.End
            | ReadDirection.Backward, Some position -> position
            | ReadDirection.Forward, None -> Position.Start
            | ReadDirection.Forward, Some position -> position
            | _ -> failwith "Illegal ReadDirection enum."

        fun store ->
            match readDirection with
            | ReadDirection.Forward ->
                store.ReadAllForwards(
                    fromPositionInclusive = fromPositionInclusive',
                    maxCount = messageCount,
                    prefetch = prefetch,
                    cancellationToken = cancellationToken
                )
            | ReadDirection.Backward ->
                store.ReadAllBackwards(
                    fromPositionInclusive = fromPositionInclusive',
                    maxCount = messageCount,
                    prefetch = prefetch,
                    cancellationToken = cancellationToken
                )
            | _ -> failwith "Illegal ReadDirection enum."

    let allStream: IStreamStore -> AsyncResult<ReadAllPage, exn> = allStream' []
