namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Append =
    let appendNewMessage (store: SqlStreamStore.IStreamStore)
                         (streamName: string)
                         (appendVersion: AppendVersion)
                         (messageDetails: MessageDetails)
                         : Async<Result<AppendResult, string>> =
        AppendRaw.appendNewMessage store streamName appendVersion messageDetails
        |> ExceptionsHandler.asyncExceptionHandler 

    let appendNewMessages (store: SqlStreamStore.IStreamStore)
                          (streamName: string)
                          (appendVersion: AppendVersion)
                          (messages: MessageDetails list)
                          : Async<Result<AppendResult, string>> =
        AppendRaw.appendNewMessages store streamName appendVersion messages
        |> ExceptionsHandler.asyncExceptionHandler 
