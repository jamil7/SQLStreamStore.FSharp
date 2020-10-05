namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore
open SqlStreamStore.Streams
open Insurello.AsyncExtra

type ReadingDirection =
    | Forward
    | Backward

module Read =
    type private StartPositionInclusive = int64
    let readFromAllStreamAsync: IStreamStore -> ReadingDirection -> StartPositionInclusive -> int -> Async<ReadAllPage> =
        fun store readingDirection startPositionInclusive msgCount ->
            match readingDirection with
            | Forward -> store.ReadAllForwards(startPositionInclusive, msgCount)
            | Backward -> store.ReadAllBackwards(startPositionInclusive, msgCount)
            |> Async.AwaitTask

    let readFromStreamAsync: IStreamStore -> ReadingDirection -> StreamDetails -> int -> Async<ReadStreamPage> =
        fun store readingDirection streamDetails msgCount ->
            match readingDirection with
            | Forward ->
                store.ReadStreamForwards(streamDetails.streamName, Helpers.toVersion streamDetails.version, msgCount)
            | Backward ->
                store.ReadStreamBackwards(streamDetails.streamName, Helpers.toVersion streamDetails.version, msgCount)
            |> Async.AwaitTask

    let readFromStreamAsync': IStreamStore -> ReadingDirection -> StreamDetails -> int -> CancellationToken -> Async<ReadStreamPage> =
        fun store readingDirection streamDetails msgCount cancellationToken ->
            match readingDirection with
            | Forward ->
                store.ReadStreamForwards
                    (streamDetails.streamName, Helpers.toVersion streamDetails.version, msgCount, cancellationToken)
            | Backward ->
                store.ReadStreamBackwards
                    (streamDetails.streamName, Helpers.toVersion streamDetails.version, msgCount, cancellationToken)
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
                            (sprintf "Failed to retrieve all messages. Retrieved messages count: %d" messageList.Length)
                |> AsyncResult.fromResult)
