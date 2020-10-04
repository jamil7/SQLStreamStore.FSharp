namespace SqlStreamStore.FSharp

open SqlStreamStore
open SqlStreamStore.Streams

type NewMessage = NewStreamMessage
type Message = StreamMessage

type ReadingDirection =
    | Forward
    | Backward

type StreamName = string
type StartPosition = int
type MessageCount = int

module Read =
    let readFromStreamAsync: IStreamStore -> StreamName -> ReadingDirection -> StartPosition -> MessageCount -> Async<ReadStreamPage> =
        fun conn stream direction startPos msgCount ->
            match direction with
            | Forward -> conn.ReadStreamForwards(stream, startPos, msgCount)
            | Backward -> conn.ReadStreamBackwards(stream, startPos, msgCount)
            |> Async.AwaitTask