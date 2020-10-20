namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Read =
    let readFromAllStream (store: SqlStreamStore.IStreamStore)
                          (readingDirection: ReadingDirection)
                          (startPositionInclusive: StartPosition)
                          (msgCount: int)
                          : Async<Result<ReadAllPage, string>> =
        ReadRaw.readFromAllStream store readingDirection startPositionInclusive msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    let readFromStream (store: SqlStreamStore.IStreamStore)
                       (readingDirection: ReadingDirection)
                       (streamName: string)
                       (readVersion: ReadVersion)
                       (msgCount: int)
                       : Async<Result<ReadStreamPage, string>> =
        ReadRaw.readFromStream store readingDirection streamName readVersion msgCount
        |> ExceptionsHandler.asyncExceptionHandler

    let readFromAllStreamAndPrefetchJsonData (store: SqlStreamStore.IStreamStore)
                                             (readingDirection: ReadingDirection)
                                             (startPositionInclusive: StartPosition)
                                             (msgCount: int)
                                             : Async<Result<ReadAllPage, string>> =
        ReadRaw.readFromAllStream' store readingDirection startPositionInclusive msgCount true
        |> ExceptionsHandler.asyncExceptionHandler

    let readFromStreamAndPrefetchJsonData (store: SqlStreamStore.IStreamStore)
                                          (readingDirection: ReadingDirection)
                                          (streamName: string)
                                          (readVersion: ReadVersion)
                                          (msgCount: int)
                                          : Async<Result<ReadStreamPage, string>> =
        ReadRaw.readFromStream' store readingDirection streamName readVersion msgCount true
        |> ExceptionsHandler.asyncExceptionHandler
