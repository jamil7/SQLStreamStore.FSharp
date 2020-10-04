namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore
open SqlStreamStore.Streams
open Insurello.AsyncExtra

type ReadingDirection =
    | Forward
    | Backward

module Read =
    let readFromStreamAsync: IStreamStore -> ReadingDirection -> StreamDetails -> int -> Async<ReadStreamPage> =
        fun store direction stream msgCount ->
            match direction with
            | Forward -> store.ReadStreamForwards(stream.streamName, stream.position, msgCount)
            | Backward -> store.ReadStreamBackwards(stream.streamName, stream.position, msgCount)
            |> Async.AwaitTask

    let readFromStreamAsync': IStreamStore -> ReadingDirection -> StreamDetails -> int -> CancellationToken -> Async<ReadStreamPage> =
        fun store direction stream msgCount cancellationToken ->
            match direction with
            | Forward -> store.ReadStreamForwards(stream.streamName, stream.position, msgCount, cancellationToken)
            | Backward -> store.ReadStreamBackwards(stream.streamName, stream.position, msgCount, cancellationToken)
            |> Async.AwaitTask

module ReadExtras =
    let readStreamMessages: IStreamStore -> ReadingDirection -> StreamDetails -> int -> AsyncResult<List<StreamMessage>, string> =
        fun store direction stream msgCount ->
            Read.readFromStreamAsync store direction stream msgCount
            |> Async.bind (fun readStreamPage ->
                readStreamPage.Messages
                |> Seq.toList
                |> fun messageList ->
                    if messageList.Length = msgCount then
                        Ok messageList
                    else
                        Error
                            (sprintf "Failed to retrieve all messages. Messages retrieved count: %d" messageList.Length)
                |> AsyncResult.fromResult)
