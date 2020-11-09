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

    /// Read forwards from the all stream.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let allForwards (store: SqlStreamStore.IStreamStore)
                    (startPositionInclusive: StartPosition)
                    (msgCount: int)
                    : Async<ReadAllPage> =
        async {
            return! store.ReadAllForwards(fromStartPositionInclusiveForwards startPositionInclusive, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read backwards from the all stream.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let allBackwards (store: SqlStreamStore.IStreamStore)
                     (startPositionInclusive: StartPosition)
                     (msgCount: int)
                     : Async<ReadAllPage> =
        async {
            return! store.ReadAllBackwards(fromStartPositionInclusiveBackwards startPositionInclusive, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read forwards from a specific stream.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let streamForwards (store: SqlStreamStore.IStreamStore)
                       (streamName: string)
                       (readVersion: ReadVersion)
                       (msgCount: int)
                       : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamForwards(StreamId(streamName), fromReadVersionForwards readVersion, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read backwards from a specific stream.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let streamBackwards (store: SqlStreamStore.IStreamStore)
                        (streamName: string)
                        (readVersion: ReadVersion)
                        (msgCount: int)
                        : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamBackwards(StreamId(streamName), fromReadVersionBackwards readVersion, msgCount)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read forwards from the all stream, prefetching the messages' jsonData.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let allForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                            (startPositionInclusive: StartPosition)
                            (msgCount: int)
                            : Async<ReadAllPage> =
        async {
            return! store.ReadAllForwards(fromStartPositionInclusiveForwards startPositionInclusive, msgCount, true)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read backwards from the all stream, prefetching the messages' jsonData.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let allBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                             (startPositionInclusive: StartPosition)
                             (msgCount: int)
                             : Async<ReadAllPage> =
        async {
            return! store.ReadAllBackwards(fromStartPositionInclusiveBackwards startPositionInclusive, msgCount, true)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read forwards from a specific stream, prefetching the messages' jsonData.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let streamForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                               (streamName: string)
                               (readVersion: ReadVersion)
                               (msgCount: int)
                               : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamForwards(StreamId(streamName), fromReadVersionForwards readVersion, msgCount, true)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read backwards from a specific stream, prefetching the messages' jsonData.
    /// Can throw exceptions.
    /// Not recommended to use. Refer to Read module's functions.
    let streamBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                                (streamName: string)
                                (readVersion: ReadVersion)
                                (msgCount: int)
                                : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamBackwards
                        (StreamId(streamName), fromReadVersionBackwards readVersion, msgCount, true)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read forwards from the all stream, prefetching the messages' jsonData.
    /// Needs a cancellation token.
    /// Can throw exceptions.
    /// Not recommended to use. 
    let allForwardsPrefetchWithCancellation (store: SqlStreamStore.IStreamStore)
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

    /// Read forwards from the all stream, prefetching the messages' jsonData.
    /// Needs a cancellation token.
    /// Can throw exceptions.
    /// Not recommended to use.
    let allBackwardsPrefetchWithCancellation (store: SqlStreamStore.IStreamStore)
                                             (startPositionInclusive: StartPosition)
                                             (msgCount: int)
                                             (cancellationToken: CancellationToken)
                                             : Async<ReadAllPage> =
        async {
            return! store.ReadAllBackwards
                        (fromStartPositionInclusiveBackwards startPositionInclusive, msgCount, true, cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read forwards from a specific stream, prefetching the messages' jsonData.
    /// Needs a cancellation token.
    /// Can throw exceptions.
    /// Not recommended to use.
    let streamForwardsPrefetchWithCancellation (store: SqlStreamStore.IStreamStore)
                                               (streamName: string)
                                               (readVersion: ReadVersion)
                                               (msgCount: int)
                                               (cancellationToken: CancellationToken)
                                               : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamForwards
                        (StreamId(streamName), fromReadVersionForwards readVersion, msgCount, true, cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }

    /// Read backwards from a specific stream, prefetching the messages' jsonData.
    /// Needs a cancellation token.
    /// Can throw exceptions.
    /// Not recommended to use.
    let streamBackwardsPrefetchWithCancellation (store: SqlStreamStore.IStreamStore)
                                                (streamName: string)
                                                (readVersion: ReadVersion)
                                                (msgCount: int)
                                                (cancellationToken: CancellationToken)
                                                : Async<ReadStreamPage> =
        async {
            return! store.ReadStreamBackwards
                        (StreamId(streamName), fromReadVersionBackwards readVersion, msgCount, true, cancellationToken)
                    |> Async.awaitTaskWithInnerException
        }
