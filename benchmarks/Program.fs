namespace SqlStreamStore.FSharp.Benchmarks

open SqlStreamStore
open SqlStreamStore.FSharp
open SqlStreamStore.FSharp.Postgres

module AppendTestMessages =
    let workflow (store: IStreamStore): Async<Streams.AppendResult> =
        [ 0 .. 400000 ]
        |> List.map (fun msg ->
            printfn "Msg #%d" msg
            { id = StreamMessageId.Auto
              type_ = "test-event"
              jsonData = sprintf "%d" msg
              jsonMetadata = "{}" })
        |> AppendRaw.appendNewMessages store "test" AppendVersion.Any

module Main =
    [<EntryPoint>]
    let main _ =

        let config: PostgresConfig =
            { host = "localhost"
              port = "5432"
              username = "test"
              password = "test"
              database = "test"
              schema = None }

        let conn = Postgres.connect config None
        let store = conn :> IStreamStore

        Postgres.createSchemaRaw conn
        |> Async.RunSynchronously

//        printfn "Appending messages..."
//        AppendTestMessages.workflow store
//        |> Async.RunSynchronously
//        |> ignore

        printfn "Fetching no prefetch..."
        let start1 = System.DateTime.Now

        let len1 =
            PrefetchVsNoPrefetch.getMessagesPrefetch store
            |> Async.RunSynchronously
            |> Array.length

        let duration1 = System.DateTime.Now - start1

        printfn "Fetching with prefetch..."
        let start2 = System.DateTime.Now

        let len2 =
            PrefetchVsNoPrefetch.getMessagesNoPrefetch store
            |> Async.RunSynchronously
            |> Array.length

        let duration2 = System.DateTime.Now - start2

//        printfn "No prefetch: %d messages in %A" len1 duration1
        printfn "With prefetch: %d messages in %A" len2 duration2
        0 // return an integer exit code
