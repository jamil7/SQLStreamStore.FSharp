namespace SqlStreamStore.FSharp

open SqlStreamStore.FSharp
open SqlStreamStore.Streams

module AppendRaw =
    let private stringIdToGuid: StreamMessageId -> System.Guid =
        function
        | StreamMessageId.Custom guid -> guid
        | StreamMessageId.Auto -> System.Guid.NewGuid()

    let private newStreamMessageFromMessageDetails: MessageDetails -> NewStreamMessage =
        fun msg ->
            match msg.jsonMetadata with
            | "" -> NewStreamMessage(stringIdToGuid msg.id, msg.type_, msg.jsonData)
            | metadata -> NewStreamMessage(stringIdToGuid msg.id, msg.type_, msg.jsonData, metadata)

    let private fromAppendVersion: AppendVersion -> int =
        function
        | AppendVersion.Any -> ExpectedVersion.Any
        | AppendVersion.EmptyStream -> ExpectedVersion.EmptyStream
        | AppendVersion.NoStream -> ExpectedVersion.NoStream
        | AppendVersion.SpecificVersion version -> version

    let appendNewMessage: SqlStreamStore.IStreamStore -> StreamName -> AppendVersion -> MessageDetails -> Async<AppendResult> =
        fun store streamName appendVersion messageDetails ->
            store.AppendToStream
                (StreamId(streamName),
                 fromAppendVersion appendVersion,
                 [| newStreamMessageFromMessageDetails messageDetails |])
            |> Async.AwaitTask

    let appendNewMessages: SqlStreamStore.IStreamStore -> StreamName -> AppendVersion -> List<MessageDetails> -> Async<AppendResult> =
        fun store streamName appendVersion messages ->
            store.AppendToStream
                (StreamId(streamName),
                 fromAppendVersion appendVersion,
                 messages
                 |> List.map newStreamMessageFromMessageDetails
                 |> List.toArray)
            |> Async.AwaitTask
