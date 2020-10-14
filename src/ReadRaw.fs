namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore.Streams

module ReadRaw =
    let private fromReadVersion: ReadVersion -> int =
        function
        | ReadVersion.Start -> int (Position.Start)
        | ReadVersion.End -> int (Position.End)
        | ReadVersion.SpecificVersion version -> int (version)

    let private fromStartPositionInclusive: StartPosition -> int64 =
        function
        | StartPosition.Start -> 0L
        | StartPosition.End -> -1L
        | StartPosition.SpecificPosition position -> position

    let readFromAllStream: SqlStreamStore.IStreamStore -> ReadingDirection -> StartPosition -> MessageCount -> Async<ReadAllPage> =
        fun store readingDirection startPositionInclusive msgCount ->
            match readingDirection with
            | ReadingDirection.Forward ->
                store.ReadAllForwards(fromStartPositionInclusive startPositionInclusive, msgCount)
            | ReadingDirection.Backward ->
                store.ReadAllBackwards(fromStartPositionInclusive startPositionInclusive, msgCount)
            |> Async.AwaitTask

    let readFromAllStream': SqlStreamStore.IStreamStore -> ReadingDirection -> StartPosition -> MessageCount -> bool -> CancellationToken -> Async<ReadAllPage> =
        fun store readingDirection startPositionInclusive msgCount prefetchJson cancellationToken ->
            match readingDirection with
            | ReadingDirection.Forward ->
                store.ReadAllForwards
                    (fromStartPositionInclusive startPositionInclusive, msgCount, prefetchJson, cancellationToken)
            | ReadingDirection.Backward ->
                store.ReadAllBackwards
                    (fromStartPositionInclusive startPositionInclusive, msgCount, prefetchJson, cancellationToken)
            |> Async.AwaitTask

    let readFromStream: SqlStreamStore.IStreamStore -> ReadingDirection -> StreamName -> ReadVersion -> MessageCount -> Async<ReadStreamPage> =
        fun store readingDirection streamName readVersion msgCount ->
            match readingDirection with
            | ReadingDirection.Forward ->
                store.ReadStreamForwards(StreamId(streamName), fromReadVersion readVersion, msgCount)
            | ReadingDirection.Backward ->
                store.ReadStreamBackwards(StreamId(streamName), fromReadVersion readVersion, msgCount)
            |> Async.AwaitTask

    let readFromStream': SqlStreamStore.IStreamStore -> ReadingDirection -> StreamName -> ReadVersion -> MessageCount -> bool -> CancellationToken -> Async<ReadStreamPage> =
        fun store readingDirection streamName readVersion msgCount prefetchJson cancellationToken ->
            match readingDirection with
            | ReadingDirection.Forward ->
                store.ReadStreamForwards
                    (StreamId(streamName), fromReadVersion readVersion, msgCount, prefetchJson, cancellationToken)
            | ReadingDirection.Backward ->
                store.ReadStreamBackwards
                    (StreamId(streamName), fromReadVersion readVersion, msgCount, prefetchJson, cancellationToken)
            |> Async.AwaitTask
