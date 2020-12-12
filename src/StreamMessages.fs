namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

[<AbstractClass>]
type AbstractStandardStreamMethods() =
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
    inherit AbstractStandardStreamMethods()

    let mapLiftSequence f =
        asyncResult {
            let! messages' = messages

            return! messages'
                    |> List.map f
                    |> List.map AsyncResult.singleton
                    |> AsyncResult.sequence
        }

    let apply f =
        asyncResult {
            let! messages' = messages
            return f messages'
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

    member this.choose(chooser: StreamMessage -> StreamMessage option) =
        StreamMessages(apply (List.choose chooser))

    member this.exists(predicate: StreamMessage -> bool) = apply (List.exists predicate)

    member this.find(predicate: StreamMessage -> bool) = ARMessage(apply (List.find predicate))

    member this.filter(predicate: StreamMessage -> bool) =
        StreamMessages(apply (List.filter predicate))

    member this.fold(folder: StreamMessage list -> StreamMessage -> StreamMessage list, ?state: StreamMessage list) =
        let state' = defaultArg state List.empty
        StreamMessages(apply (List.fold folder state'))

    member this.head() = ARMessage(apply List.head)

    member this.last() = ARMessage(apply List.last)

    member this.length() = apply List.length

    member this.tryFind(predicate: StreamMessage -> bool) =
        AROMessage(apply (List.tryFind predicate))

    member this.tryHead() = AROMessage(apply List.tryHead)

    member this.tryLast() = AROMessage(apply List.tryLast)
