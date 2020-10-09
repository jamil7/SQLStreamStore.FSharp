module SqlStreamStore.FSharp.Tests.ReadTests

open Expecto
open SqlStreamStore.FSharp
open SqlStreamStore.FSharp.Tests


let newTestMessage: string -> MessageDetails =
    fun guid ->
        { id = StreamMessageId.Custom(System.Guid.Parse(guid))
          type_ = "testing"
          jsonData = "{}"
          jsonMetadata = "{}" }

let guidString1 = "11111111-1111-1111-1111-111111111111"

let guidString2 = "22222222-2222-2222-2222-222222222222"

[<Tests>]
let tests =
    testList
        "Read Tests"
        [ testAsync "Should read forward from specific stream." {
              let inMemStore = new SqlStreamStore.InMemoryStreamStore()

              let streamName = "test"

              let appendVersion = AppendVersion.NoStream

              let msg1 = newTestMessage guidString1

              let msg2 = newTestMessage guidString2

              let msgList = [ msg1; msg2 ]

              do! Append.appendNewMessages inMemStore streamName appendVersion msgList
                  |> Async.Ignore

              let readVersion = ReadVersion.Start

              let! readResult = Read.readFromStream inMemStore ReadingDirection.Forward streamName readVersion 10

              readResult.Messages
              |> Array.sortBy (fun msg -> msg.MessageId)
              |> fun sorted ->
                  ExpectExtra.equal guidString1 (sorted.[0].MessageId.ToString())
                  ExpectExtra.equal guidString2 (sorted.[1].MessageId.ToString())
          }
          testAsync "Should read Backward from specific stream." {
              let inMemStore = new SqlStreamStore.InMemoryStreamStore()

              let streamName = "test"

              let appendVersion = AppendVersion.NoStream

              let msg1 = newTestMessage guidString1

              let msg2 = newTestMessage guidString2

              let msgList = [ msg1; msg2 ]

              do! Append.appendNewMessages inMemStore streamName appendVersion msgList
                  |> Async.Ignore

              let readVersion = ReadVersion.End

              let! readResult = Read.readFromStream inMemStore ReadingDirection.Backward streamName readVersion 10

              readResult.Messages
              |> Array.sortBy (fun msg -> msg.MessageId)
              |> fun sorted ->
                  ExpectExtra.equal guidString2 (sorted.[1].MessageId.ToString())
                  ExpectExtra.equal guidString1 (sorted.[0].MessageId.ToString())
          }

          testAsync "Should read from all streams forward." {
              let inMemStore = new SqlStreamStore.InMemoryStreamStore()

              let stream1 = "test1"

              let stream2 = "test2"

              let appendVersion = AppendVersion.NoStream

              let msg1 = newTestMessage guidString1

              let msg2 = newTestMessage guidString2

              do! Append.appendNewMessage inMemStore stream1 appendVersion msg1
                  |> Async.Ignore

              do! Append.appendNewMessage inMemStore stream2 appendVersion msg2
                  |> Async.Ignore

              let! readResult = Read.readFromAllStream inMemStore ReadingDirection.Forward StartPosition.Start 10

              readResult.Messages
              |> Array.sortBy (fun msg -> msg.MessageId)
              |> fun sorted ->
                  ExpectExtra.equal guidString1 (sorted.[0].MessageId.ToString())
                  ExpectExtra.equal guidString2 (sorted.[1].MessageId.ToString())
          }

          testAsync "Should read from all streams backward." {
              let inMemStore = new SqlStreamStore.InMemoryStreamStore()

              let stream1 = "test1"

              let stream2 = "test2"

              let appendVersion = AppendVersion.NoStream

              let msg1 = newTestMessage guidString1

              let msg2 = newTestMessage guidString2

              do! Append.appendNewMessage inMemStore stream1 appendVersion msg1
                  |> Async.Ignore

              do! Append.appendNewMessage inMemStore stream2 appendVersion msg2
                  |> Async.Ignore

              let! readResult = Read.readFromAllStream inMemStore ReadingDirection.Backward StartPosition.End 10

              readResult.Messages
              |> Array.sortBy (fun msg -> msg.MessageId)
              |> fun sorted ->
                  ExpectExtra.equal guidString2 (sorted.[1].MessageId.ToString())
                  ExpectExtra.equal guidString1 (sorted.[0].MessageId.ToString())
          } ]
