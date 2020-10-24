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

    let readFromAllStream (store: SqlStreamStore.IStreamStore)
                          (readingDirection: ReadingDirection)
                          (startPositionInclusive: StartPosition)
                          (msgCount: int)
                          : Async<ReadAllPage> =
        async {
            return! match readingDirection with
                    | ReadingDirection.Forward ->
                        store.ReadAllForwards(fromStartPositionInclusive startPositionInclusive, msgCount)
                    | ReadingDirection.Backward ->
                        store.ReadAllBackwards(fromStartPositionInclusive startPositionInclusive, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    let readFromStream (store: SqlStreamStore.IStreamStore)
                       (readingDirection: ReadingDirection)
                       (streamName: string)
                       (readVersion: ReadVersion)
                       (msgCount: int)
                       : Async<ReadStreamPage> =
        async {
            return! match readingDirection with
                    | ReadingDirection.Forward ->
                        store.ReadStreamForwards(StreamId(streamName), fromReadVersion readVersion, msgCount)
                    | ReadingDirection.Backward ->
                        store.ReadStreamBackwards(StreamId(streamName), fromReadVersion readVersion, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    let readFromAllStream' (store: SqlStreamStore.IStreamStore)
                           (readingDirection: ReadingDirection)
                           (startPositionInclusive: StartPosition)
                           (msgCount: int)
                           (prefetchJson: bool)
                           : Async<ReadAllPage> =
        async {
            return! match readingDirection with
                    | ReadingDirection.Forward ->
                        store.ReadAllForwards(fromStartPositionInclusive startPositionInclusive, msgCount, prefetchJson)
                    | ReadingDirection.Backward ->
                        store.ReadAllBackwards
                            (fromStartPositionInclusive startPositionInclusive, msgCount, prefetchJson)
                    |> Async.awaitTaskWithInnerException
        }

    let readFromStream' (store: SqlStreamStore.IStreamStore)
                        (readingDirection: ReadingDirection)
                        (streamName: string)
                        (readVersion: ReadVersion)
                        (msgCount: int)
                        (prefetchJson: bool)
                        : Async<ReadStreamPage> =
        async {
            return! match readingDirection with
                    | ReadingDirection.Forward ->
                        store.ReadStreamForwards
                            (StreamId(streamName), fromReadVersion readVersion, msgCount, prefetchJson)
                    | ReadingDirection.Backward ->
                        store.ReadStreamBackwards
                            (StreamId(streamName), fromReadVersion readVersion, msgCount, prefetchJson)
                    |> Async.awaitTaskWithInnerException
        }

    let readFromAllStream'' (store: SqlStreamStore.IStreamStore)
                            (readingDirection: ReadingDirection)
                            (startPositionInclusive: StartPosition)
                            (msgCount: int)
                            (prefetchJson: bool)
                            (cancellationToken: CancellationToken)
                            : Async<ReadAllPage> =
        async {
            return! match readingDirection with
                    | ReadingDirection.Forward ->
                        store.ReadAllForwards
                            (fromStartPositionInclusive startPositionInclusive,
                             msgCount,
                             prefetchJson,
                             cancellationToken)
                    | ReadingDirection.Backward ->
                        store.ReadAllBackwards
                            (fromStartPositionInclusive startPositionInclusive,
                             msgCount,
                             prefetchJson,
                             cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }

    let readFromStream'' (store: SqlStreamStore.IStreamStore)
                         (readingDirection: ReadingDirection)
                         (streamName: string)
                         (readVersion: ReadVersion)
                         (msgCount: int)
                         (prefetchJson: bool)
                         (cancellationToken: CancellationToken)
                         : Async<ReadStreamPage> =
        async {
            return! match readingDirection with
                    | ReadingDirection.Forward ->
                        store.ReadStreamForwards
                            (StreamId(streamName),
                             fromReadVersion readVersion,
                             msgCount,
                             prefetchJson,
                             cancellationToken)
                    | ReadingDirection.Backward ->
                        store.ReadStreamBackwards
                            (StreamId(streamName),
                             fromReadVersion readVersion,
                             msgCount,
                             prefetchJson,
                             cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }
