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
        JsonFSharpConverter
            (unionTagName = "tag", unionFieldsName = "value", unionEncoding = JsonUnionEncoding.AdjacentTag)

    opt.Converters.Add converterOpt

    let DefaultSerializationConfig: SerializerConfig<'a> =
        { serialize = fun (eventData: 'a) -> JsonSerializer.Serialize<'a>(eventData, opt)
          deserialize = fun (data: string) -> JsonSerializer.Deserialize<'a>(data, opt) }

module internal Serdes =

    open SerializationConfig

    let serialize<'a> : 'a -> string = DefaultSerializationConfig.serialize

    let deserialize<'a> : string -> 'a = DefaultSerializationConfig.deserialize
