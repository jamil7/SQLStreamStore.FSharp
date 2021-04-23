namespace SqlStreamStore.FSharp.EventSourcing

open FSharp.Prelude
open SqlStreamStore.FSharp
open SqlStreamStore.Streams
open System

type private Metadata =
    {
        author : string
        causationId : Guid option
        correlationId : Guid
        meta : string option
        timestamp : DateTimeOffset
    }

type private NewStreamEventInternal<'event> =
    {
        author : string
        causationId : Guid option
        correlationId : Guid
        data : 'event
        id : Guid
        metadata : string option
        timestamp : DateTimeOffset
    }

type NewStreamEvent<'event> = private NewStreamEvent of NewStreamEventInternal<'event>

module NewStreamEvent =

    /// Creates a NewStreamEvent with the following defaults:
    /// id = Guid.NewGuid()
    /// timestamp = DateTimeOffset.Now
    /// correlationId = Guid.NewGuid()
    /// causationId = None
    /// metadata = None
    let create<'event> (author : string) (data : 'event) : NewStreamEvent<'event> =
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

    let withId (id : Guid) : NewStreamEvent<'event> -> NewStreamEvent<'event> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with id = id }

    let withTimestamp (timestamp : DateTimeOffset) : NewStreamEvent<'event> -> NewStreamEvent<'event> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with timestamp = timestamp }

    let withCorrelationId (correlationId : Guid) : NewStreamEvent<'event> -> NewStreamEvent<'event> =
        fun (NewStreamEvent event) ->
            NewStreamEvent
                { event with
                    correlationId = correlationId
                }

    let withCausationId (causationId : Guid) : NewStreamEvent<'event> -> NewStreamEvent<'event> =
        fun (NewStreamEvent event) ->
            NewStreamEvent
                { event with
                    causationId = Some causationId
                }

    let withMetadata (metadata : string) : NewStreamEvent<'event> -> NewStreamEvent<'event> =
        fun (NewStreamEvent event) -> NewStreamEvent { event with metadata = Some metadata }

    let internal toNewStreamMessage : NewStreamEvent<'event> -> Result<NewStreamMessage, exn> =
        fun (NewStreamEvent event) ->
            let metadata : Metadata =
                {
                    author = event.author
                    causationId = event.causationId
                    correlationId = event.correlationId
                    meta = event.metadata
                    timestamp = event.timestamp
                }

            result {
                let! data' = JayJson.encode event.data
                let! metadata' = JayJson.encode metadata
                return NewStreamMessage(event.id, eventPrefix + unionToString event.data, data', metadata')
            }


[<Struct>]
type StreamEvent<'event> =
    {
        author : string
        causationId : Guid option
        correlationId : Guid
        data : AsyncResult<'event, exn>
        dataAsString : AsyncResult<string, exn>
        id : Guid
        metadata : string option
        position : int64
        streamId : string
        streamVersion : int
        timestamp : DateTimeOffset
        typeAsString : string
    }

module StreamEvent =

    let ofStreamMessage<'event> (msg : StreamMessage) : Result<StreamEvent<'event>, exn> =
        result {
            let! meta = JayJson.decode<Metadata> msg.JsonMetadata

            let data' =
                asyncResult {
                    let! json = msg.GetJsonData()
                    return! JayJson.decode<'event> json
                }

            return
                {
                    author = meta.author
                    causationId = meta.causationId
                    correlationId = meta.correlationId
                    data = data'
                    dataAsString = msg.GetJsonData()
                    id = msg.MessageId
                    metadata = meta.meta
                    position = msg.Position
                    streamId = msg.StreamId
                    streamVersion = msg.StreamVersion
                    timestamp = meta.timestamp
                    typeAsString = msg.Type
                }
        }
