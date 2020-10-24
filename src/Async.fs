namespace SqlStreamStore.FSharp

module Async =

    open System.Threading.Tasks

    let map f m = async.Bind(m, (f >> async.Return))

    let awaitTaskWithInnerException (task: Task<'T>): Async<'T> =
        Async.FromContinuations(fun (suc, exn, _cln) ->
            task.ContinueWith(fun (t: Task<'T>) ->
                if t.IsFaulted
                then if t.Exception.InnerExceptions.Count = 1 then
                         exn t.Exception.InnerExceptions.[0]
                     else
                         exn t.Exception
                elif t.IsCanceled
                then exn (TaskCanceledException())
                else suc t.Result)
            |> ignore)

    let awaitTaskWithInnerException' (task: Task): Async<unit> =
        Async.FromContinuations(fun (suc, exn, _cln) ->
            task.ContinueWith(fun (t: Task) ->
                if t.IsFaulted
                then if t.Exception.InnerExceptions.Count = 1 then
                         exn t.Exception.InnerExceptions.[0]
                     else
                         exn t.Exception
                elif t.IsCanceled
                then exn (TaskCanceledException())
                else suc ())
            |> ignore)
