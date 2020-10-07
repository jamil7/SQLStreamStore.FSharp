namespace SqlStreamStore.FSharp

open System.Threading
open SqlStreamStore.Streams
open Insurello.AsyncExtra

module Read =
    let private fromReadVersion: uint -> int = fun readVersion -> int (readVersion)

    let readFromAllStreamAsync: SqlStreamStore.IStreamStore -> ReadingDirection -> StartPositionInclusive -> MessageCount -> Async<ReadAllPage> =
        fun store readingDirection startPositionInclusive msgCount ->
            match readingDirection with
            | ReadingDirection.Forward -> store.ReadAllForwards(startPositionInclusive, msgCount)
            | ReadingDirection.Backward -> store.ReadAllBackwards(startPositionInclusive, msgCount)
            |> Async.AwaitTask

    let readFromAllStreamAsync': SqlStreamStore.IStreamStore -> ReadingDirection -> StartPositionInclusive -> MessageCount -> bool -> CancellationToken -> Async<ReadAllPage> =
        fun store readingDirection startPositionInclusive msgCount prefetchJson cancellationToken ->
            match readingDirection with
            | ReadingDirection.Forward ->
                store.ReadAllForwards(startPositionInclusive, msgCount, prefetchJson, cancellationToken)
            | ReadingDirection.Backward ->
                store.ReadAllBackwards(startPositionInclusive, msgCount, prefetchJson, cancellationToken)
            |> Async.AwaitTask

    let readFromStreamAsync: SqlStreamStore.IStreamStore -> ReadingDirection -> ReadStreamDetails -> MessageCount -> Async<ReadStreamPage> =
        fun store readingDirection readStreamDetails msgCount ->
            match readingDirection with
            | ReadingDirection.Forward ->
                store.ReadStreamForwards
                    (StreamId(readStreamDetails.streamName), fromReadVersion readStreamDetails.version, msgCount)
            | ReadingDirection.Backward ->
                store.ReadStreamBackwards
                    (StreamId(readStreamDetails.streamName), fromReadVersion readStreamDetails.version, msgCount)
            |> Async.AwaitTask

    let readFromStreamAsync': SqlStreamStore.IStreamStore -> ReadingDirection -> ReadStreamDetails -> MessageCount -> bool -> CancellationToken -> Async<ReadStreamPage> =
        fun store readingDirection readStreamDetails msgCount prefetchJson cancellationToken ->
            match readingDirection with
            | ReadingDirection.Forward ->
                store.ReadStreamForwards
                    (StreamId(readStreamDetails.streamName),
                     fromReadVersion readStreamDetails.version,
                     msgCount,
                     prefetchJson,
                     cancellationToken)
            | ReadingDirection.Backward ->
                store.ReadStreamBackwards
                    (StreamId(readStreamDetails.streamName),
                     fromReadVersion readStreamDetails.version,
                     msgCount,
                     prefetchJson,
                     cancellationToken)
            |> Async.AwaitTask

module ReadExtras =
    let readAllStreamMessages: SqlStreamStore.IStreamStore -> ReadingDirection -> StartPositionInclusive -> MessageCount -> AsyncResult<List<StreamMessage>, string> =
        fun store readingDirection startPositionInclusive msgCount ->
            Read.readFromAllStreamAsync store readingDirection startPositionInclusive msgCount
            |> Async.bind (fun readAllPage ->
                readAllPage.Messages
                |> Seq.toList
                |> fun messageList ->
                    if messageList.Length = msgCount then
                        Ok messageList
                    else
                        Error
                            (sprintf "Failed to retrieve all messages. Retrieved messages count: %d" messageList.Length)
                |> AsyncResult.fromResult)

    let readStreamMessages: SqlStreamStore.IStreamStore -> ReadingDirection -> ReadStreamDetails -> MessageCount -> AsyncResult<List<StreamMessage>, string> =
        fun store readingDirection readStreamDetails msgCount ->
            Read.readFromStreamAsync store readingDirection readStreamDetails msgCount
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
