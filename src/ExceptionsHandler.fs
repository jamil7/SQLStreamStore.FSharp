namespace SqlStreamStore.FSharp

module Async =
    let map f m = async.Bind(m, (f >> async.Return))

module ExceptionsHandler =
    let simpleExceptionHandler (op: Async<'res>): Async<Result<'res, string>> =
        op
        |> Async.Catch
        |> Async.map (function
            | Choice1Of2 response -> Ok response
            | Choice2Of2 exn -> Error exn.Message)
