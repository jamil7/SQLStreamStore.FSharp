namespace SqlStreamStore.FSharp

module ExceptionsHandler =
    /// Handles thrown exceptions from an async operation by wrapping them in a Result. 
    let asyncExceptionHandler (asyncOperation: Async<'a>): Async<Result<'a, exn>> =
        asyncOperation
        |> Async.Catch
        |> Async.map (function
            | Choice1Of2 response -> Ok response
            | Choice2Of2 exn -> Error exn)
