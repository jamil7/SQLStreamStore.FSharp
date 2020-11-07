namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module GetRaw =
    let messagePayload (message: StreamMessage): Async<string> =
        async {
            return! message.GetJsonData()
                    |> Async.awaitTaskWithInnerException
        }

    let messagesPayload (messages: StreamMessage list): Async<string list> =
        messages
        |> List.map messagePayload
        |> Async.parallelCombine

    