namespace SqlStreamStore.FSharp

module Async =

    open System.Threading.Tasks

    let map f m = async.Bind(m, (f >> async.Return))

    let awaitTaskWithInnerException (task: Task<'T>): Async<'T> =
        Async.FromContinuations(fun (success, exception', _cancellationToken) ->
            task.ContinueWith(fun (t: Task<'T>) ->
                if t.IsFaulted then
                    if t.Exception.InnerExceptions.Count = 1
                    then exception' t.Exception.InnerExceptions.[0]
                    else exception' t.Exception
                elif t.IsCanceled then
                    exception' (TaskCanceledException())
                else
                    success t.Result)
            |> ignore)

    let awaitTaskWithInnerException' (task: Task): Async<unit> =
        Async.FromContinuations(fun (success, exception', _cancellationToken) ->
            task.ContinueWith(fun (t: Task) ->
                if t.IsFaulted then
                    if t.Exception.InnerExceptions.Count = 1
                    then exception' t.Exception.InnerExceptions.[0]
                    else exception' t.Exception
                elif t.IsCanceled then
                    exception' (TaskCanceledException())
                else
                    success ())
            |> ignore)
