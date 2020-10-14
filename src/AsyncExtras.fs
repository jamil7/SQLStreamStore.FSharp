module Async

let bind f m = async.Bind(m, f)

let map f m = m |> bind (f >> async.Return)
