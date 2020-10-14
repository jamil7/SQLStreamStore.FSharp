namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Read =
    let readFromAllStream (store: SqlStreamStore.IStreamStore)
                          (readingDirection: ReadingDirection)
                          (startPositionInclusive: StartPosition)
                          (msgCount: MessageCount)
                          : Async<Result<ReadAllPage, string>> =
        ReadRaw.readFromAllStream store readingDirection startPositionInclusive msgCount
        |> ExceptionHandler.simpleExceptionHandler 

    let readFromStream store readingDirection streamName readVersion msgCount =
        ReadRaw.readFromStream store readingDirection streamName readVersion msgCount
        |> ExceptionHandler.simpleExceptionHandler 
