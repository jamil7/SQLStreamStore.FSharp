namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore
open SqlStreamStore.Streams
open Insurello.AsyncExtra

type ReadingDirection =
    | Forward
    | Backward

module Read =
    let readFromStreamAsync: IStreamStore -> ReadingDirection -> ReadStreamDetails -> int -> Async<ReadStreamPage> =
        fun store readingDirection streamDetails msgCount ->
            match readingDirection with
            | Forward -> store.ReadStreamForwards(streamDetails.streamName, streamDetails.startPosition, msgCount)
            | Backward -> store.ReadStreamBackwards(streamDetails.streamName, streamDetails.startPosition, msgCount)
            |> Async.AwaitTask

    let readFromStreamAsync': IStreamStore -> ReadingDirection -> ReadStreamDetails -> int -> CancellationToken -> Async<ReadStreamPage> =
        fun store readingDirection streamDetails msgCount cancellationToken ->
            match readingDirection with
            | Forward ->
                store.ReadStreamForwards
                    (streamDetails.streamName, streamDetails.startPosition, msgCount, cancellationToken)
            | Backward ->
                store.ReadStreamBackwards
                    (streamDetails.streamName, streamDetails.startPosition, msgCount, cancellationToken)
            |> Async.AwaitTask

module ReadExtras =
    let readStreamMessages: IStreamStore -> ReadingDirection -> ReadStreamDetails -> int -> AsyncResult<List<StreamMessage>, string> =
        fun store readingDirection streamDetails msgCount ->
            Read.readFromStreamAsync store readingDirection streamDetails msgCount
            |> Async.bind (fun readStreamPage ->
                readStreamPage.Messages
                |> Seq.toList
                |> fun messageList ->
                    if messageList.Length = msgCount then
                        Ok messageList
                    else
                        Error
                            (sprintf "Failed to retrieve all messages. Retrieved messages count: %d" messageList.Length)
                |> AsyncResult.fromResult)
