namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore
open SqlStreamStore.Streams

type NewMessage = NewStreamMessage
type Message = StreamMessage
type Connection = IStreamStore

type ReadingDirection =
    | Forward
    | Backward

type StreamName = string
type StartPosition = int
type MessageCount = int

module Read =
    let readFromStreamAsync: Connection -> StreamName -> ReadingDirection -> StartPosition -> MessageCount -> Async<ReadStreamPage> =
        fun conn stream direction startPos msgCount ->
            match direction with
            | Forward -> conn.ReadStreamForwards(stream, startPos, msgCount)
            | Backward -> conn.ReadStreamBackwards(stream, startPos, msgCount)
            |> Async.AwaitTask

    let readFromStream: Connection -> StreamName -> ReadingDirection -> StartPosition -> MessageCount -> List<Message> =
        fun conn stream direction startPos msgCount ->
            readFromStreamAsync conn stream direction startPos msgCount
            |> Async.RunSynchronously
            |> fun readStreamPage -> readStreamPage.Messages
            |> Seq.toList