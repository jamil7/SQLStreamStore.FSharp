namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Append =
    let appendNewMessage (store: SqlStreamStore.IStreamStore)
                         (streamName: StreamName)
                         (appendVersion: AppendVersion)
                         (messageDetails: MessageDetails)
                         : Async<Result<AppendResult, string>> =
        AppendRaw.appendNewMessage store streamName appendVersion messageDetails
        |> ExceptionsHandler.simpleExceptionHandler 

    let appendNewMessages (store: SqlStreamStore.IStreamStore)
                          (streamName: StreamName)
                          (appendVersion: AppendVersion)
                          (messages: MessageDetails list)
                          : Async<Result<AppendResult, string>> =
        AppendRaw.appendNewMessages store streamName appendVersion messages
        |> ExceptionsHandler.simpleExceptionHandler 
