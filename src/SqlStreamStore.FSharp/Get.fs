namespace SqlStreamStore.FSharp

open FSharp.Prelude
open SqlStreamStore.Streams

module Get =

    // A function to help wit type inference in this module
    let private curriedMap : (ReadStreamPage -> 'a) -> AsyncResult<ReadStreamPage, exn> -> AsyncResult<'a, exn> =
        AsyncResult.map

    let messages =
        curriedMap (fun page -> page.Messages |> Array.toList)

    let messagesData =
        messages
        >> AsyncResult.bind (AsyncResult.mapM (fun msg -> msg.GetJsonData()))

    let messagesDataAs<'data> =
        messages
        >> AsyncResult.bind (AsyncResult.mapM (fun msg -> msg.GetJsonDataAs<'data>()))

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

    let nextStreamPage =
        AsyncResult.bind (fun (page: ReadStreamPage) -> page.ReadNext |> AsyncResult.ofTask)

module GetAll =

    // A function to help wit type inference in this module
    let private curriedMap : (ReadAllPage -> 'a) -> AsyncResult<ReadAllPage, exn> -> AsyncResult<'a, exn> =
        AsyncResult.map

    let messages =
        curriedMap (fun page -> page.Messages |> Array.toList)

    let messagesData =
        messages
        >> AsyncResult.bind (AsyncResult.mapM (fun msg -> msg.GetJsonData()))

    let messagesDataAs<'data> =
        messages
        >> AsyncResult.bind (AsyncResult.mapM (fun msg -> msg.GetJsonDataAs<'data>()))

    let direction = curriedMap (fun page -> page.Direction)

    let fromPosition =
        curriedMap (fun page -> page.FromPosition)

    let isEnd = curriedMap (fun page -> page.IsEnd)

    let nextPosition =
        curriedMap (fun page -> page.NextPosition)

    let nextAllStreamPage =
        AsyncResult.bind (fun (page: ReadAllPage) -> page.ReadNext |> AsyncResult.ofTask)
