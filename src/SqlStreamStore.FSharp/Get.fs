namespace SqlStreamStore.FSharp

open Prelude
open SqlStreamStore.Streams

module Get =

    let messages: AsyncResult<ReadStreamPage, exn> -> AsyncResult<StreamMessage list, exn> =
        AsyncResult.map (fun page -> page.Messages |> Array.toList)

    let messagesData: AsyncResult<ReadStreamPage, exn> -> AsyncResult<string list, exn> =
        messages
        >> AsyncResult.bind (AsyncResult.traverse (fun msg -> msg.GetJsonData()))

    let messagesDataAs<'data> : AsyncResult<ReadStreamPage, exn> -> AsyncResult<'data list, exn> =
        messages
        >> AsyncResult.bind (AsyncResult.traverse (fun msg -> msg.GetJsonDataAs<'data>()))

    let status: AsyncResult<ReadStreamPage, exn> -> AsyncResult<PageReadStatus, exn> =
        AsyncResult.map (fun page -> page.Status)

    let isEnd: AsyncResult<ReadStreamPage, exn> -> AsyncResult<bool, exn> = AsyncResult.map (fun page -> page.IsEnd)

    let readDirection: AsyncResult<ReadStreamPage, exn> -> AsyncResult<ReadDirection, exn> =
        AsyncResult.map (fun page -> page.ReadDirection)

    let streamId: AsyncResult<ReadStreamPage, exn> -> AsyncResult<string, exn> =
        AsyncResult.map (fun page -> page.StreamId)

    let fromStreamVersion: AsyncResult<ReadStreamPage, exn> -> AsyncResult<int, exn> =
        AsyncResult.map (fun page -> page.FromStreamVersion)

    let lastStreamPosition: AsyncResult<ReadStreamPage, exn> -> AsyncResult<int64, exn> =
        AsyncResult.map (fun page -> page.LastStreamPosition)

    let nextStreamVersion: AsyncResult<ReadStreamPage, exn> -> AsyncResult<int, exn> =
        AsyncResult.map (fun page -> page.NextStreamVersion)

    let nextStreamPage: AsyncResult<ReadStreamPage, exn> -> AsyncResult<ReadStreamPage, exn> =
        AsyncResult.bind (fun (page: ReadStreamPage) -> page.ReadNext |> AsyncResult.ofTask)

module GetAll =

    let messages: AsyncResult<ReadAllPage, exn> -> AsyncResult<StreamMessage list, exn> =
        AsyncResult.map (fun page -> page.Messages |> Array.toList)

    let messagesData: AsyncResult<ReadAllPage, exn> -> AsyncResult<string list, exn> =
        messages
        >> AsyncResult.bind (AsyncResult.traverse (fun msg -> msg.GetJsonData()))

    let messagesDataAs<'data> : AsyncResult<ReadAllPage, exn> -> AsyncResult<'data list, exn> =
        messages
        >> AsyncResult.bind (AsyncResult.traverse (fun msg -> msg.GetJsonDataAs<'data>()))

    let direction: AsyncResult<ReadAllPage, exn> -> AsyncResult<ReadDirection, exn> =
        AsyncResult.map (fun page -> page.Direction)

    let fromPosition: AsyncResult<ReadAllPage, exn> -> AsyncResult<int64, exn> =
        AsyncResult.map (fun page -> page.FromPosition)

    let isEnd: AsyncResult<ReadAllPage, exn> -> AsyncResult<bool, exn> = AsyncResult.map (fun page -> page.IsEnd)

    let nextPosition: AsyncResult<ReadAllPage, exn> -> AsyncResult<int64, exn> =
        AsyncResult.map (fun page -> page.NextPosition)

    let nextAllStreamPage: AsyncResult<ReadAllPage, exn> -> AsyncResult<ReadAllPage, exn> =
        AsyncResult.bind (fun (page: ReadAllPage) -> page.ReadNext |> AsyncResult.ofTask)
