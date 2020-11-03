namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Read =
    let allForwards (store: SqlStreamStore.IStreamStore)
                    (startPositionInclusive: StartPosition)
                    (msgCount: int)
                    : Async<Result<ReadAllPage, string>> =
        ReadRaw.allForwards store startPositionInclusive msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    let allBackwards (store: SqlStreamStore.IStreamStore)
                     (startPositionInclusive: StartPosition)
                     (msgCount: int)
                     : Async<Result<ReadAllPage, string>> =
        ReadRaw.allBackwards store startPositionInclusive msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    let streamForwards (store: SqlStreamStore.IStreamStore)
                       (streamName: string)
                       (readVersion: ReadVersion)
                       (msgCount: int)
                       : Async<Result<ReadStreamPage, string>> =
        ReadRaw.streamForwards store streamName readVersion msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    let streamBackwards (store: SqlStreamStore.IStreamStore)
                        (streamName: string)
                        (readVersion: ReadVersion)
                        (msgCount: int)
                        : Async<Result<ReadStreamPage, string>> =
        ReadRaw.streamBackwards store streamName readVersion msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    let allForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                            (startPositionInclusive: StartPosition)
                            (msgCount: int)
                            : Async<Result<ReadAllPage, string>> =
        ReadRaw.allForwardsPrefetch store startPositionInclusive msgCount true
        |> ExceptionsHandler.asyncExceptionHandler

    let allBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                             (startPositionInclusive: StartPosition)
                             (msgCount: int)
                             : Async<Result<ReadAllPage, string>> =
        ReadRaw.allBackwardsPrefetch store startPositionInclusive msgCount true
        |> ExceptionsHandler.asyncExceptionHandler

    let streamForwardsPrefetch (store: SqlStreamStore.IStreamStore)
                               (streamName: string)
                               (readVersion: ReadVersion)
                               (msgCount: int)
                               : Async<Result<ReadStreamPage, string>> =
        ReadRaw.streamForwardsPrefetch store streamName readVersion msgCount true
        |> ExceptionsHandler.asyncExceptionHandler

    let streamBackwardsPrefetch (store: SqlStreamStore.IStreamStore)
                                (streamName: string)
                                (readVersion: ReadVersion)
                                (msgCount: int)
                                : Async<Result<ReadStreamPage, string>> =
        ReadRaw.streamBackwardsPrefetch store streamName readVersion msgCount true
        |> ExceptionsHandler.asyncExceptionHandler
