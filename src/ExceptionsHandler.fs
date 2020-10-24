namespace SqlStreamStore.FSharp

module ExceptionsHandler =
    let asyncExceptionHandler (op: Async<'suc>): Async<Result<'suc, string>> =
        op
        |> Async.Catch
        |> Async.map (function
            | Choice1Of2 response -> Ok response
            | Choice2Of2 exn -> Error exn.Message)
