namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

[<AbstractClass>]
type AbstractARMessage() =
    abstract position: unit -> AsyncResult<int64, exn>
    abstract type': unit -> AsyncResult<string, exn>
    abstract createdUtc: unit -> AsyncResult<System.DateTime, exn>
    abstract jsonMetadata: unit -> AsyncResult<string, exn>
    abstract messageId: unit -> AsyncResult<System.Guid, exn>
    abstract streamId: unit -> AsyncResult<string, exn>
    abstract streamVersion: unit -> AsyncResult<int, exn>
    abstract toString: unit -> AsyncResult<string, exn>
    abstract jsonData: unit -> AsyncResult<string, exn>

type ARMessage(message: AsyncResult<StreamMessage, exn>) =
    inherit AbstractARMessage()

    let mapLiftSequence f =
        asyncResult {
            let! message' = message
            return! AsyncResult.singleton (f message')
        }

    override this.position() =
        mapLiftSequence (fun msg -> msg.Position)

    override this.type'() = mapLiftSequence (fun msg -> msg.Type)

    override this.createdUtc() =
        mapLiftSequence (fun msg -> msg.CreatedUtc)

    override this.jsonMetadata() =
        mapLiftSequence (fun msg -> msg.JsonMetadata)

    override this.messageId() =
        mapLiftSequence (fun msg -> msg.MessageId)

    override this.streamId() =
        mapLiftSequence (fun msg -> msg.StreamId)

    override this.streamVersion() =
        mapLiftSequence (fun msg -> msg.StreamVersion)

    override this.toString() =
        mapLiftSequence (fun msg -> msg.ToString())

    override this.jsonData() =
        asyncResult {
            let! messages' = message
            return! AsyncResult.ofTask messages'.GetJsonData
        }

[<AbstractClass>]
type AbstractAROMessage() =
    abstract position: unit -> AsyncResultOption<int64, exn>
    abstract type': unit -> AsyncResultOption<string, exn>
    abstract createdUtc: unit -> AsyncResultOption<System.DateTime, exn>
    abstract jsonMetadata: unit -> AsyncResultOption<string, exn>
    abstract messageId: unit -> AsyncResultOption<System.Guid, exn>
    abstract streamId: unit -> AsyncResultOption<string, exn>
    abstract streamVersion: unit -> AsyncResultOption<int, exn>
    abstract toString: unit -> AsyncResultOption<string, exn>
    abstract jsonData: unit -> AsyncResultOption<string, exn>

type AROMessage(message: AsyncResultOption<StreamMessage, exn>) =
    inherit AbstractAROMessage()

    let mapLiftSequence f =
        asyncResultOption {
            let! message' = message
            return! AsyncResultOption.singleton (f message')
        }

    override this.position() =
        mapLiftSequence (fun msg -> msg.Position)

    override this.type'() = mapLiftSequence (fun msg -> msg.Type)

    override this.createdUtc() =
        mapLiftSequence (fun msg -> msg.CreatedUtc)

    override this.jsonMetadata() =
        mapLiftSequence (fun msg -> msg.JsonMetadata)

    override this.messageId() =
        mapLiftSequence (fun msg -> msg.MessageId)

    override this.streamId() =
        mapLiftSequence (fun msg -> msg.StreamId)

    override this.streamVersion() =
        mapLiftSequence (fun msg -> msg.StreamVersion)

    override this.toString() =
        mapLiftSequence (fun msg -> msg.ToString())


    override this.jsonData() =
        asyncResultOption {
            let! message' = message
            return! AsyncResultOption.ofTask message'.GetJsonData
        }
