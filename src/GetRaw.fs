namespace SqlStreamStore.FSharp

open SqlStreamStore.Streams

module GetRaw =
    let messagePayload (message: StreamMessage) =
        message.GetJsonData

