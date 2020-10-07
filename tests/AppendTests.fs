module SqlStreamStore.FSharp.Tests.AppendTests

open Expecto
open SqlStreamStore.FSharp
open SqlStreamStore.FSharp.Tests


[<Tests>]
let tests =
    testList
        "Append Tests"
        [ testAsync "Should append one message to stream." {
              let inMemStore = new SqlStreamStore.InMemoryStreamStore()

              let streamName = "test"
              
              let appendVersion = AppendVersion.NoStream

              let msg =
                  { id = StreamMessageId.Auto
                    type_ = "testing"
                    jsonData = "{}"
                    jsonMetadata = "{}" }

              let! appendResult = Append.appendNewMessage inMemStore streamName appendVersion msg

              ExpectExtra.equal 0 appendResult.CurrentVersion
          }

          testAsync "Should append a list of messages to stream." {
              let inMemStore = new SqlStreamStore.InMemoryStreamStore()

              let streamName = "test"
              
              let appendVersion = AppendVersion.NoStream

              let msg1 =
                  { id = StreamMessageId.Auto
                    type_ = "testing"
                    jsonData = "{}"
                    jsonMetadata = "{}" }

              let msg2 =
                  { id = StreamMessageId.Auto
                    type_ = "testing"
                    jsonData = "{}"
                    jsonMetadata = "{}" }

              let msgList = [ msg1; msg2 ]

              let! appendResult = Append.appendNewMessages inMemStore streamName appendVersion msgList
              ExpectExtra.equal 1 appendResult.CurrentVersion
          } ]
