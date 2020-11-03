namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore.Streams

module ReadRaw =
    let private fromReadVersionForwards: ReadVersion -> int =
        function
        | ReadVersion.Any -> int (Position.Start)
        | ReadVersion.SpecificVersion version -> int (version)

    let private fromReadVersionBackwards: ReadVersion -> int =
        function
        | ReadVersion.Any -> int (Position.End)
        | ReadVersion.SpecificVersion version -> int (version)

    let private fromStartPositionInclusiveForwards: StartPosition -> int64 =
        function
        | StartPosition.Any -> 0L
        | StartPosition.SpecificPosition position -> position

    let private fromStartPositionInclusiveBackwards: StartPosition -> int64 =
        function
        | StartPosition.Any -> -1L
        | StartPosition.SpecificPosition position -> position

    let allForwards (store: SqlStreamStore.IStreamStore)
                    (startPositionInclusive: StartPosition)
                    (msgCount: int)
                    : Async<ReadAllPage> =
        async {
            return! store.ReadAllForwards(fromStartPositionInclusiveForwards startPositionInclusive, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    let allBackwards (store: SqlStreamStore.IStreamStore)
                     (startPositionInclusive: StartPosition)
                     (msgCount: int)
                     : Async<ReadAllPage> =
        async {
            return! store.ReadAllBackwards(fromStartPositionInclusiveBackwards startPositionInclusive, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    let streamForwards (store: SqlStreamStore.IStreamStore)
                       (streamName: string)
                       (readVersion: ReadVersion)
                       (msgCount: int)
                       : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamForwards(StreamId(streamName), fromReadVersionForwards readVersion, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    let streamBackwards (store: SqlStreamStore.IStreamStore)
                        (streamName: string)
                        (readVersion: ReadVersion)
                        (msgCount: int)
                        : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamBackwards(StreamId(streamName), fromReadVersionBackwards readVersion, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    let allForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                            (startPositionInclusive: StartPosition)
                            (msgCount: int)
                            (prefetchJson: bool)
                            : Async<ReadAllPage> =
        async {
            return! store.ReadAllForwards
                        (fromStartPositionInclusiveForwards startPositionInclusive, msgCount, prefetchJson)
                    |> Async.awaitTaskWithInnerException
        }

    let allBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                             (startPositionInclusive: StartPosition)
                             (msgCount: int)
                             (prefetchJson: bool)
                             : Async<ReadAllPage> =
        async {
            return! store.ReadAllBackwards
                        (fromStartPositionInclusiveBackwards startPositionInclusive, msgCount, prefetchJson)
                    |> Async.awaitTaskWithInnerException
        }

    let streamForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                               (streamName: string)
                               (readVersion: ReadVersion)
                               (msgCount: int)
                               (prefetchJson: bool)
                               : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamForwards
                        (StreamId(streamName), fromReadVersionForwards readVersion, msgCount, prefetchJson)
                    |> Async.awaitTaskWithInnerException
        }

    let streamBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                                (streamName: string)
                                (readVersion: ReadVersion)
                                (msgCount: int)
                                (prefetchJson: bool)
                                : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamBackwards
                        (StreamId(streamName), fromReadVersionBackwards readVersion, msgCount, prefetchJson)
                    |> Async.awaitTaskWithInnerException
        }

    let allForwards' (store: SqlStreamStore.IStreamStore)
                     (startPositionInclusive: StartPosition)
                     (msgCount: int)
                     (prefetchJson: bool)
                     (cancellationToken: CancellationToken)
                     : Async<ReadAllPage> =
        async {
            return! store.ReadAllForwards
                        (fromStartPositionInclusiveForwards startPositionInclusive,
                         msgCount,
                         prefetchJson,
                         cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }

    let allBackwards' (store: SqlStreamStore.IStreamStore)
                      (startPositionInclusive: StartPosition)
                      (msgCount: int)
                      (prefetchJson: bool)
                      (cancellationToken: CancellationToken)
                      : Async<ReadAllPage> =
        async {
            return! store.ReadAllBackwards
                        (fromStartPositionInclusiveBackwards startPositionInclusive,
                         msgCount,
                         prefetchJson,
                         cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }

    let streamForwards' (store: SqlStreamStore.IStreamStore)
                        (streamName: string)
                        (readVersion: ReadVersion)
                        (msgCount: int)
                        (prefetchJson: bool)
                        (cancellationToken: CancellationToken)
                        : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamForwards
                        (StreamId(streamName),
                         fromReadVersionForwards readVersion,
                         msgCount,
                         prefetchJson,
                         cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }

    let streamBackwards' (store: SqlStreamStore.IStreamStore)
                         (streamName: string)
                         (readVersion: ReadVersion)
                         (msgCount: int)
                         (prefetchJson: bool)
                         (cancellationToken: CancellationToken)
                         : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamBackwards
                        (StreamId(streamName),
                         fromReadVersionBackwards readVersion,
                         msgCount,
                         prefetchJson,
                         cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }
