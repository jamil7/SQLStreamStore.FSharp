namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

module Get =

    let private curriedMap : (ReadStreamPage -> 'a) -> AsyncResult<ReadStreamPage, exn> -> AsyncResult<'a, exn> =
        AsyncResult.map

    let messages =
        curriedMap (fun page -> page.Messages |> Array.toList)

    let messagesData =
        messages
        >> AsyncResult.bind (List.traverseAsyncResultM (fun msg -> msg.GetJsonData()))

    let messagesDataAs<'data> =
        messages
        >> AsyncResult.bind (List.traverseAsyncResultM (fun msg -> msg.GetJsonDataAs<'data>()))

    let status = curriedMap (fun page -> page.Status)

    let isEnd = curriedMap (fun page -> page.IsEnd)

    let readDirection =
        curriedMap (fun page -> page.ReadDirection)

    let streamId = curriedMap (fun page -> page.StreamId)

    let fromStreamVersion =
        curriedMap (fun page -> page.FromStreamVersion)

    let lastStreamPosition =
        curriedMap (fun page -> page.LastStreamPosition)

    let nextStreamVersion =
        curriedMap (fun page -> page.NextStreamVersion)


namespace SqlStreamStore.FSharp.EventSourcing

open FSharp.Prelude
open SqlStreamStore.FSharp
open SqlStreamStore.Streams

module Get =

    let events<'event> =
        Get.messages
        >> Async.map (
            Result.bind (
                List.filter (fun msg -> msg.Type.Contains eventPrefix)
                >> List.traverseResultM StreamEvent.ofStreamMessage<'event>
            )
        )

    let eventsData<'event> =
        events<'event>
        >> AsyncResult.bind (List.traverseAsyncResultM (fun event -> event.data))

    let eventsAndEventsData<'event> =
        fun (page: AsyncResult<ReadStreamPage, exn>) ->
            asyncResult {
                let! events' = events<'event> page
                let! data = List.traverseAsyncResultM (fun event -> event.data) events'
                return List.zip events' data
            }

    let eventDataAsString<'event> =
        events<'event>
        >> AsyncResult.bind (List.traverseAsyncResultM (fun event -> event.dataAsString))
