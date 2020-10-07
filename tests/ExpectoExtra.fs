open Expecto

/// Remove the need for message in expectations, as recommended https://github.com/haf/expecto/issues/361
module Expect =

    let inline equal e a = Expect.equal a e "Failed"

    let inline exists a sequence = Expect.exists sequence a "Failed"

    let inline contains a sequence = Expect.contains sequence a "Failed"

    let inline containsAll a sequence = Expect.containsAll sequence a "Failed"

    let inline hasLength expectedLength sequence =
        Expect.hasLength sequence expectedLength "Failed"

    let inline isEmpty a = Expect.isEmpty a "Failed"

    let inline isError a = Expect.isError a "Failed"

    let inline isNonEmpty a = Expect.isNonEmpty a "Failed"

    let inline isNone a = Expect.isNone a "Failed"

    let inline wantOk a = Expect.wantOk a "Failed"

    let inline wantError a = Expect.wantError a "Failed"

    let inline wantSome a = Expect.wantSome a "Failed"
