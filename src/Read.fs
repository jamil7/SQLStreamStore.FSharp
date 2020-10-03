namespace SqlStreamStore.FSharp

open SqlStreamStore
open SqlStreamStore.Streams

type StreamMessage = NewStreamMessage

module Read =
    let read _ = failwith "hi"