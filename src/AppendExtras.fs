namespace SqlStreamStore.FSharp

open SqlStreamStore

module AppendExtras =
    let appendNewMessage (store: IStreamStore)
                         (streamName: StreamName)
                         (appendVersion: AppendVersion)
                         (messageDetails: MessageDetails)
                         : Async<Result<Streams.AppendResult, string>> =
        Append.appendNewMessage store streamName appendVersion messageDetails
        |> Async.Catch
        |> Async.map (function
            | Choice1Of2 response -> Ok response
            | Choice2Of2 exn -> Error exn.Message)

    let appendNewMessages (store: IStreamStore)
                          (streamName: StreamName)
                          (appendVersion: AppendVersion)
                          (messages: MessageDetails list)
                          : Async<Result<Streams.AppendResult, string>> =
        Append.appendNewMessages store streamName appendVersion messages
        |> Async.Catch
        |> Async.map (function
            | Choice1Of2 response -> Ok response
            | Choice2Of2 exn -> Error exn.Message)
