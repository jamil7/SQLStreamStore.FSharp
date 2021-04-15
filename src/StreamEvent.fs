namespace SqlStreamStore.FSharp.EventSourcing

open FSharp.Prelude
open SqlStreamStore.FSharp
open SqlStreamStore.Streams
open System

type private Metadata =
    {
        author: string
        causationId: Guid option
        correlationId: Guid
        meta: string option
        timestamp: DateTimeOffset
    }

type private NewStreamEventInternal<'a> =
    {
        author: string
        causationId: Guid option
        correlationId: Guid
        data: 'a
        id: Guid
        metadata: string option
        timestamp: DateTimeOffset
    }

type NewStreamEvent<'a> = private NewStreamEvent of NewStreamEventInternal<'a>

module NewStreamEvent =
    
    /// Creates a NewStreamEvent with the following defaults:
    /// id = Guid.NewGuid()
    /// timestamp = DateTimeOffset.Now
    /// correlationId = Guid.NewGuid()
    /// causationId = None
    /// metadata = None
    let create<'a> (author: string) (data: 'a) : NewStreamEvent<'a> =
        NewStreamEvent
            {
                author = author
                causationId = None
                correlationId = Guid.NewGuid()
                data = data
                id = Guid.NewGuid()
                metadata = None
                timestamp = DateTimeOffset.Now
            }

    let withId (id: Guid) : NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with id = id }

    let withTimestamp (timestamp: DateTimeOffset) : NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with timestamp = timestamp }

    let withCorrelationId (correlationId: Guid) : NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) ->
            NewStreamEvent
                { event with
                    correlationId = correlationId
                }

    let withCausationId (causationId: Guid) : NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) ->
            NewStreamEvent
                { event with
                    causationId = Some causationId
                }

    let withMetadata (metadata: string) : NewStreamEvent<'a> -> NewStreamEvent<'a> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with metadata = Some metadata }

    let internal toNewStreamMessage : NewStreamEvent<'a> -> NewStreamMessage =
        fun (NewStreamEvent event) ->
            let metadata : Metadata =
                {
                    author = event.author
                    causationId = event.causationId
                    correlationId = event.correlationId
                    meta = event.metadata
                    timestamp = event.timestamp
                }

            NewStreamMessage(
                event.id,
                "Event::" + unionToString event.data,
                Serializer.serialize event.data,
                Serializer.serialize metadata
            )

[<Struct>]
type StreamEvent<'event> =
    {
        author: string
        causationId: Guid option
        correlationId: Guid
        data: AsyncResult<'event, exn>
        id: Guid
        metadata: string option
        position: int64
        streamId: string
        streamVersion: int
        timestamp: DateTimeOffset
        typeAsString: string
    }

module StreamEvent =

    let ofStreamMessage<'event> (msg: StreamMessage) : StreamEvent<'event> =
        let meta =
            Serializer.deserialize<Metadata> msg.JsonMetadata

        {
            author = meta.author
            causationId = meta.causationId
            correlationId = meta.correlationId
            data =
                asyncResult {
                    let! json = msg.GetJsonData()
                    return Serializer.deserialize<'event> json
                }
            id = msg.MessageId
            metadata = meta.meta
            position = msg.Position
            streamId = msg.StreamId
            streamVersion = msg.StreamVersion
            timestamp = meta.timestamp
            typeAsString = msg.Type
        }
