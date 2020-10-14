namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Read =
    let readFromAllStream (store: SqlStreamStore.IStreamStore)
                          (readingDirection: ReadingDirection)
                          (startPositionInclusive: StartPosition)
                          (msgCount: int)
                          : Async<Result<ReadAllPage, string>> =
        ReadRaw.readFromAllStream store readingDirection startPositionInclusive msgCount
        |> ExceptionsHandler.simpleExceptionHandler

    let readFromStream (store: SqlStreamStore.IStreamStore)
                       (readingDirection: ReadingDirection)
                       (streamName: string)
                       (readVersion: ReadVersion)
                       (msgCount: int)
                       : Async<Result<ReadStreamPage, string>> =
        ReadRaw.readFromStream store readingDirection streamName readVersion msgCount
        |> ExceptionsHandler.simpleExceptionHandler
