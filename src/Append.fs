namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module Append =
    /// Appends a new message to a given stream.
    let appendNewMessage (store: SqlStreamStore.IStreamStore)
                         (streamName: string)
                         (appendVersion: AppendVersion)
                         (messageDetails: MessageDetails)
                         : Async<Result<AppendResult, exn>> =
        AppendRaw.appendNewMessage store streamName appendVersion messageDetails
        |> ExceptionsHandler.asyncExceptionHandler 

    /// Appends a list of messages to a given stream.
    let appendNewMessages (store: SqlStreamStore.IStreamStore)
                          (streamName: string)
                          (appendVersion: AppendVersion)
                          (messages: MessageDetails list)
                          : Async<Result<AppendResult, exn>> =
        AppendRaw.appendNewMessages store streamName appendVersion messages
        |> ExceptionsHandler.asyncExceptionHandler 
