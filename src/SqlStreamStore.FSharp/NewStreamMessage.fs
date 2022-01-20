namespace SqlStreamStore.FSharp

[<Struct>]
type NewStreamMessage =
    private
        {
            messageId: System.Guid option
            messageType: string
            jsonData: string
            jsonMetadata: string option
        }

module NewStreamMessage =

    let create (messageType: string) (jsonData: string) : NewStreamMessage =
        {
            messageId = None
            messageType = messageType
            jsonData = jsonData
            jsonMetadata = None
        }

    let withMessageId (messageId: System.Guid) (msg: NewStreamMessage) : NewStreamMessage =
        { msg with messageId = Some messageId }

    let withJsonMetadata (jsonMetadata: string) (msg: NewStreamMessage) : NewStreamMessage =
        { msg with
            jsonMetadata = Some jsonMetadata
        }

    let internal toOriginalNewStreamMessage (msg: NewStreamMessage) : SqlStreamStore.Streams.NewStreamMessage =
        let id =
            match msg.messageId with
            | Some id -> id
            | None -> System.Guid.NewGuid()

        match msg.jsonMetadata with
        | Some metadata -> SqlStreamStore.Streams.NewStreamMessage(id, msg.messageType, msg.jsonData, metadata)
        | None -> SqlStreamStore.Streams.NewStreamMessage(id, msg.messageType, msg.jsonData)
