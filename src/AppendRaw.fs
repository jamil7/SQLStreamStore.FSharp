namespace SqlStreamStore.FSharp

open SqlStreamStore.FSharp
open SqlStreamStore.Streams

module AppendRaw =
    let private stringIdToGuid: StreamMessageId -> System.Guid =
        function
        | StreamMessageId.Custom guid -> guid
        | StreamMessageId.Auto -> System.Guid.NewGuid()

    let private newStreamMessageFromMessageDetails (msg: MessageDetails): NewStreamMessage =
        match msg.jsonMetadata with
        | ""
        | "{}" -> NewStreamMessage(stringIdToGuid msg.id, msg.type_, msg.jsonData)
        | metadata -> NewStreamMessage(stringIdToGuid msg.id, msg.type_, msg.jsonData, metadata)

    let private fromAppendVersion: AppendVersion -> int =
        function
        | AppendVersion.Any -> ExpectedVersion.Any
        | AppendVersion.EmptyStream -> ExpectedVersion.EmptyStream
        | AppendVersion.NoStream -> ExpectedVersion.NoStream
        | AppendVersion.SpecificVersion version -> version

    /// Appends a new message to a given stream.
    /// Can throw exceptions.
    let appendNewMessage (store: SqlStreamStore.IStreamStore)
                         (streamName: string)
                         (appendVersion: AppendVersion)
                         (messageDetails: MessageDetails)
                         : Async<AppendResult> =
        async {
            return! store.AppendToStream
                        (StreamId(streamName),
                         fromAppendVersion appendVersion,
                         [| newStreamMessageFromMessageDetails messageDetails |])
                    |> Async.awaitTaskWithInnerException
        }

    /// Appends a list of messages to a given stream.
    /// Can throw exceptions.
    let appendNewMessages (store: SqlStreamStore.IStreamStore)
                          (streamName: string)
                          (appendVersion: AppendVersion)
                          (messages: MessageDetails list)
                          : Async<AppendResult> =
        async {
            return! store.AppendToStream
                        (StreamId(streamName),
                         fromAppendVersion appendVersion,
                         messages
                         |> List.map newStreamMessageFromMessageDetails
                         |> List.toArray)
                    |> Async.awaitTaskWithInnerException
        }
