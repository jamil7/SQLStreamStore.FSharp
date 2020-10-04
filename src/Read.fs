namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore
open SqlStreamStore.Streams
open Insurello.AsyncExtra

type ReadingDirection =
    | Forward
    | Backward

module Read =
    let toStartPosition: int option -> int =
        function
        | Some position -> position
        | None -> 0

    let readFromStreamAsync: IStreamStore -> ReadingDirection -> StreamDetails -> int -> Async<ReadStreamPage> =
        fun store readingDirection streamDetails msgCount ->
            match readingDirection with
            | Forward ->
                store.ReadStreamForwards
                    (streamDetails.streamName, toStartPosition streamDetails.startPosition, msgCount)
            | Backward ->
                store.ReadStreamBackwards
                    (streamDetails.streamName, toStartPosition streamDetails.startPosition, msgCount)
            |> Async.AwaitTask

    let readFromStreamAsync': IStreamStore -> ReadingDirection -> StreamDetails -> int -> CancellationToken -> Async<ReadStreamPage> =
        fun store readingDirection streamDetails msgCount cancellationToken ->
            match readingDirection with
            | Forward ->
                store.ReadStreamForwards
                    (streamDetails.streamName, toStartPosition streamDetails.startPosition, msgCount, cancellationToken)
            | Backward ->
                store.ReadStreamBackwards
                    (streamDetails.streamName, toStartPosition streamDetails.startPosition, msgCount, cancellationToken)
            |> Async.AwaitTask

module ReadExtras =
    let readStreamMessages: IStreamStore -> ReadingDirection -> StreamDetails -> int -> AsyncResult<List<StreamMessage>, string> =
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
                            (sprintf "Failed to retrieve all messages. Messages retrieved count: %d" messageList.Length)
                |> AsyncResult.fromResult)
