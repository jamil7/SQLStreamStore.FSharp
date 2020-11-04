namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Read =
    /// Read forwards from the all stream.
    let allForwards (store: SqlStreamStore.IStreamStore)
                    (startPositionInclusive: StartPosition)
                    (msgCount: int)
                    : Async<Result<ReadAllPage, exn>> =
        ReadRaw.allForwards store startPositionInclusive msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    /// Read backwards from the all stream.
    let allBackwards (store: SqlStreamStore.IStreamStore)
                     (startPositionInclusive: StartPosition)
                     (msgCount: int)
                     : Async<Result<ReadAllPage, exn>> =
        ReadRaw.allBackwards store startPositionInclusive msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    /// Read forwards from a specific stream.
    let streamForwards (store: SqlStreamStore.IStreamStore)
                       (streamName: string)
                       (readVersion: ReadVersion)
                       (msgCount: int)
                       : Async<Result<ReadStreamPage, exn>> =
        ReadRaw.streamForwards store streamName readVersion msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    /// Read backwards from a specific stream.
    let streamBackwards (store: SqlStreamStore.IStreamStore)
                        (streamName: string)
                        (readVersion: ReadVersion)
                        (msgCount: int)
                        : Async<Result<ReadStreamPage, exn>> =
        ReadRaw.streamBackwards store streamName readVersion msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    /// Read forwards from the all stream, prefetching the messages' jsonData.
    let allForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                            (startPositionInclusive: StartPosition)
                            (msgCount: int)
                            : Async<Result<ReadAllPage, exn>> =
        ReadRaw.allForwardsPrefetch store startPositionInclusive msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    /// Read backwards from the all stream, prefetching the messages' jsonData.
    let allBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                             (startPositionInclusive: StartPosition)
                             (msgCount: int)
                             : Async<Result<ReadAllPage, exn>> =
        ReadRaw.allBackwardsPrefetch store startPositionInclusive msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    /// Read forwards from a specific stream, prefetching the messages' jsonData.
    let streamForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                               (streamName: string)
                               (readVersion: ReadVersion)
                               (msgCount: int)
                               : Async<Result<ReadStreamPage, exn>> =
        ReadRaw.streamForwardsPrefetch store streamName readVersion msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    /// Read backwards from a specific stream, prefetching the messages' jsonData.
    let streamBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                                (streamName: string)
                                (readVersion: ReadVersion)
                                (msgCount: int)
                                : Async<Result<ReadStreamPage, exn>> =
        ReadRaw.streamBackwardsPrefetch store streamName readVersion msgCount
        |> ExceptionsHandler.asyncExceptionHandler
