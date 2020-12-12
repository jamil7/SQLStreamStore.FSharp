namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

[<AbstractClass>]
type AbstractARMessageMethods() =
    abstract position: unit -> AsyncResult<int64 list, exn>
    abstract type': unit -> AsyncResult<string list, exn>
    abstract createdUtc: unit -> AsyncResult<System.DateTime list, exn>
    abstract jsonMetadata: unit -> AsyncResult<string list, exn>
    abstract messageId: unit -> AsyncResult<System.Guid list, exn>
    abstract streamId: unit -> AsyncResult<string list, exn>
    abstract streamVersion: unit -> AsyncResult<int list, exn>
    abstract toString: unit -> AsyncResult<string list, exn>
    abstract jsonData: unit -> AsyncResult<string list, exn>


type StreamMessages(messages: AsyncResult<StreamMessage list, exn>) =
    inherit AbstractARMessageMethods()

    let mapLiftSequence f =
        asyncResult {
            let! messages' = messages

            return! messages'
                    |> List.map f
                    |> List.map AsyncResult.singleton
                    |> AsyncResult.sequence
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
            let! messages' = messages

            return! messages'
                    |> List.map (fun msg -> msg.GetJsonData)
                    |> List.map AsyncResult.ofTask
                    |> AsyncResult.sequence
        }

    member this.length() =
        asyncResult {
            let! messages' = messages
            return messages'.Length
        }

    member this.filter(predicate: StreamMessage -> bool) =
        let messages' =
            asyncResult {
                let! messages' = messages
                return List.filter predicate messages'
            }

        StreamMessages(messages')

    member this.tryHead() =
        asyncResult {
            let! messages' = messages
            return List.tryHead messages' |> AsyncOption.ofOption
        }
        |> AsyncOption.ofAsyncResult
        |> AsyncOption.bind id
