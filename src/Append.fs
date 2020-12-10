namespace SqlStreamStore.FSharp

open SqlStreamStore
open SqlStreamStore.Streams
open FSharp.Prelude

module Append =
    let private stringIdToGuid: StreamMessageId -> System.Guid =
        function
        | StreamMessageId.Custom guid -> guid
        | StreamMessageId.Auto -> System.Guid.NewGuid()

    let private newStreamMessageFromMessageDetails (msg: MessageDetails): NewStreamMessage =
        match msg.jsonMetadata with
        | ""
        | "{}" -> NewStreamMessage(stringIdToGuid msg.id, msg.type', msg.jsonData)
        | metadata -> NewStreamMessage(stringIdToGuid msg.id, msg.type', msg.jsonData, metadata)

    let private fromAppendVersion: AppendVersion -> int =
        function
        | AppendVersion.Any -> ExpectedVersion.Any
        | AppendVersion.EmptyStream -> ExpectedVersion.EmptyStream
        | AppendVersion.NoStream -> ExpectedVersion.NoStream
        | AppendVersion.SpecificVersion version -> version

    let append (store: IStreamStore) (stream: string) (appendVersion: AppendVersion) (messages: MessageDetails list) =
        asyncResult {
            return! store.AppendToStream
                        (StreamId(stream),
                         fromAppendVersion appendVersion,
                         messages
                         |> List.map newStreamMessageFromMessageDetails
                         |> List.toArray)
        }
