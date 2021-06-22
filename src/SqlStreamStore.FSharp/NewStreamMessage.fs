namespace SqlStreamStore.FSharp

[<Struct>]
type NewStreamMessageInternal =
    {
        messageId: System.Guid option
        messageType: string
        jsonData: string
        jsonMetadata: string option
    }

[<Struct>]
type NewStreamMessage = private NewStreamMessage of NewStreamMessageInternal

module NewStreamMessage =

    let create (messageType: string) (jsonData: string) : NewStreamMessage =
        NewStreamMessage
            {
                messageId = None
                messageType = messageType
                jsonData = jsonData
                jsonMetadata = None
            }

    let withMessageId (messageId: System.Guid) (NewStreamMessage msg: NewStreamMessage) : NewStreamMessage =
        NewStreamMessage { msg with messageId = Some messageId }

    let withJsonMetadata (jsonMetadata: string) (NewStreamMessage msg: NewStreamMessage) : NewStreamMessage =
        NewStreamMessage
            { msg with
                jsonMetadata = Some jsonMetadata
            }

    let internal toOriginalNewStreamMessage
        (NewStreamMessage msg: NewStreamMessage)
        : SqlStreamStore.Streams.NewStreamMessage =
        let id =
            match msg.messageId with
            | Some id -> id
            | None -> System.Guid.NewGuid()

        match msg.jsonMetadata with
        | Some metadata -> SqlStreamStore.Streams.NewStreamMessage(id, msg.messageType, msg.jsonData, metadata)
        | None -> SqlStreamStore.Streams.NewStreamMessage(id, msg.messageType, msg.jsonData)
