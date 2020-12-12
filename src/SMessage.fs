namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

[<AbstractClass>]
type AbstractStandardStreamMessageMethods() =
    abstract position: unit -> AsyncResult<int64, exn>
    abstract type': unit -> AsyncResult<string, exn>
    abstract createdUtc: unit -> AsyncResult<System.DateTime, exn>
    abstract jsonMetadata: unit -> AsyncResult<string, exn>
    abstract messageId: unit -> AsyncResult<System.Guid, exn>
    abstract streamId: unit -> AsyncResult<string, exn>
    abstract streamVersion: unit -> AsyncResult<int, exn>
    abstract toString: unit -> AsyncResult<string, exn>
    abstract jsonData: unit -> AsyncResult<string, exn>

type SMessage(message: AsyncResult<StreamMessage, exn>) =
    inherit AbstractStandardStreamMessageMethods()

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

            return! messages'
                    |> (fun msg -> msg.GetJsonData)
                    |> AsyncResult.ofTask
        }
