namespace SqlStreamStore.FSharp

[<AutoOpen>]
module Helpers =

    let internal eventPrefix = "Event::"

    let private memoize (f : 'a -> 'b) (a : 'a) : 'b =
        let cache =
            System.Collections.Concurrent.ConcurrentDictionary<'a, Lazy<'b>>()

        let getOrAdd (a : 'a) (f : 'a -> 'b) =
            let lazyRes =
                cache.GetOrAdd(
                    a,
                    (fun a ->
                        Lazy<'b>(
                            valueFactory = (fun _ -> f a),
                            mode = System.Threading.LazyThreadSafetyMode.ExecutionAndPublication
                        ))
                )

            lazyRes.Value

        getOrAdd a f

    let private unionToString' (a : 'a) : string =
        Reflection.FSharpValue.GetUnionFields(a, typeof<'a>)
        |> fst
        |> fun case -> case.Name

    let internal unionToString<'a> : 'a -> string = memoize unionToString'

    let private getEventUnionCases'<'a> () : string seq =
        Reflection.FSharpType.GetUnionCases typeof<'a>
        |> Seq.map (fun info -> eventPrefix + info.Name)

    let internal getEventUnionCases<'a> = memoize id getEventUnionCases'<'a>

    let internal protect (f : 'a -> 'b) x =
        try
            Ok(f x)
        with e -> Error e

module SerializationConfig =

    open System.Text.Json
    open System.Text.Json.Serialization

    type SerializerConfig<'a> =
        {
            encode : 'a -> Result<string, exn>
            decode : string -> Result<'a, exn>
        }

    let private opt =
        JsonSerializerOptions(IgnoreNullValues = true)

    let private converterOpt =
        JsonFSharpConverter(
            unionTagName = "tag",
            unionFieldsName = "value",
            unionEncoding = JsonUnionEncoding.AdjacentTag
        )

    do opt.Converters.Add converterOpt

    let defaultSerializationConfig<'a> : SerializerConfig<'a> =
        {
            encode =
                protect
                <| fun (eventData : 'a) -> JsonSerializer.Serialize<'a>(eventData, opt)
            decode =
                protect
                <| fun (data : string) -> JsonSerializer.Deserialize<'a>(data, opt)
        }

module internal JayJson =

    open SerializationConfig

    let encode<'a> : 'a -> Result<string, exn> = defaultSerializationConfig<'a>.encode

    let decode<'a> : string -> Result<'a, exn> = defaultSerializationConfig<'a>.decode
