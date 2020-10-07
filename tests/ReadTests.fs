module SqlStreamStore.FSharp.Tests.ReadTests

open Expecto
open SqlStreamStore.FSharp
open SqlStreamStore.FSharp.Tests


[<Tests>]
let tests =
    testList
        "Read Tests"
        [ testAsync "Should read forward from specific stream." {
              let inMemStore = new SqlStreamStore.InMemoryStreamStore()

              let appendStreamDetails: AppendStreamDetails =
                  { streamName = "test"
                    version = AppendVersion.NoStream }

              let guidString1 = "11111111-1111-1111-1111-111111111111"

              let guidString2 = "22222222-2222-2222-2222-222222222222"

              let msg1 =
                  { id = StreamMessageId.Custom(System.Guid.Parse(guidString1))
                    type_ = "testing"
                    jsonData = "{}"
                    jsonMetadata = "{}" }

              let msg2 =
                  { id = StreamMessageId.Custom(System.Guid.Parse(guidString2))
                    type_ = "testing"
                    jsonData = "{}"
                    jsonMetadata = "{}" }

              let msgList = [ msg1; msg2 ]

              do! Append.appendNewMessages inMemStore appendStreamDetails msgList
                  |> Async.Ignore

              let readStreamDetails: ReadStreamDetails = { streamName = "test"; version = 0u }

              let! readResult = Read.readFromStreamAsync inMemStore ReadingDirection.Forward readStreamDetails 10

              readResult.Messages
              |> Array.sortBy (fun msg -> msg.MessageId)
              |> fun sorted ->
                  ExpectExtra.equal guidString1 (sorted.[0].MessageId.ToString())
                  ExpectExtra.equal guidString2 (sorted.[1].MessageId.ToString())
          } ]
