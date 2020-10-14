module SqlStreamStore.FSharp.Tests.AppendTests

open Expecto
open SqlStreamStore.FSharp


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

              Expect.equal appendResult.CurrentVersion 0 "Error: message version doesn't match."
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
              Expect.equal appendResult.CurrentVersion 1 "Error: message version doesn't match."
          } ]
