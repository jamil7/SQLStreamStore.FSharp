namespace SqlStreamStore.FSharp


open Insurello.AsyncExtra
open SqlStreamStore.Streams

module Append =
    let private stringIdToGuid: Id -> System.Guid =
        function
        | Custom guid -> guid
        | Auto -> System.Guid.NewGuid()

    let private newStreamMessageFromMessageDetails: MessageDetails -> NewStreamMessage =
        fun msg ->
            match msg.jsonMetadata with
            | "" -> NewStreamMessage(stringIdToGuid msg.id, msg.type_, msg.jsonData)
            | metadata -> NewStreamMessage(stringIdToGuid msg.id, msg.type_, msg.jsonData, metadata)

    let private fromAppendVersion: AppendVersion -> int =
        function
        | AppendVersion.Any -> SqlStreamStore.Streams.ExpectedVersion.Any
        | AppendVersion.EmptyStream -> SqlStreamStore.Streams.ExpectedVersion.EmptyStream
        | AppendVersion.NoStream -> SqlStreamStore.Streams.ExpectedVersion.NoStream
        | AppendVersion.SpecificVersion version -> version

    let appendNewMessage: SqlStreamStore.IStreamStore -> AppendStreamDetails -> MessageDetails -> Async<AppendResult> =
        fun store streamDetails messageDetails ->
            store.AppendToStream
                (StreamId(streamDetails.streamName),
                 fromAppendVersion streamDetails.version,
                 [| newStreamMessageFromMessageDetails messageDetails |])
            |> Async.AwaitTask

    let appendNewMessages: SqlStreamStore.IStreamStore -> AppendStreamDetails -> List<MessageDetails> -> Async<AppendResult> =
        fun store streamDetails messages ->
            store.AppendToStream
                (StreamId(streamDetails.streamName),
                 fromAppendVersion streamDetails.version,
                 messages
                 |> List.map newStreamMessageFromMessageDetails
                 |> List.toArray)
            |> Async.AwaitTask
