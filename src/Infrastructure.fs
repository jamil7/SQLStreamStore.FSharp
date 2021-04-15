namespace SqlStreamStore.FSharp

module SerializationConfig =

    open System.Text.Json
    open System.Text.Json.Serialization

    type SerializerConfig<'a> =
        { serialize: 'a -> string
          deserialize: string -> 'a }

    let private opt =
        JsonSerializerOptions(IgnoreNullValues = true)

    let private converterOpt =
        JsonFSharpConverter(
            unionTagName = "tag",
            unionFieldsName = "value",
            unionEncoding = JsonUnionEncoding.AdjacentTag
        )

    do opt.Converters.Add converterOpt

    let DefaultSerializationConfig : SerializerConfig<'a> =
        { serialize = fun (eventData: 'a) -> JsonSerializer.Serialize<'a>(eventData, opt)
          deserialize = fun (data: string) -> JsonSerializer.Deserialize<'a>(data, opt) }

module internal Serializer =

    open SerializationConfig

    let serialize<'a> : 'a -> string = DefaultSerializationConfig.serialize

    let deserialize<'a> : string -> 'a = DefaultSerializationConfig.deserialize

[<AutoOpen>]
module Helpers =

    open System.Threading

    let internal memoize : ('a -> 'b) -> 'a -> 'b =
        fun f a ->
            let cache =
                System.Collections.Concurrent.ConcurrentDictionary<'a, Lazy<'b>>()

            let getOrAdd (a: 'a) (f: 'a -> 'b) =
                let lazyRes =
                    cache.GetOrAdd(
                        a,
                        (fun a ->
                            Lazy<'b>(valueFactory = (fun _ -> f a), mode = LazyThreadSafetyMode.ExecutionAndPublication))
                    )

                lazyRes.Value

            getOrAdd a f
