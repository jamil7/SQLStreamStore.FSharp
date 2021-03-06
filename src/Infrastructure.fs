namespace SqlStreamStore.FSharp

module SerializationConfig =

    open System.Text.Json
    open System.Text.Json.Serialization

    type SerializerConfig<'a> =
        { encode: 'a -> string
          decode: string -> 'a }

    let private opt =
        JsonSerializerOptions(IgnoreNullValues = true)

    let private converterOpt =
        JsonFSharpConverter
            (unionTagName = "tag", unionFieldsName = "value", unionEncoding = JsonUnionEncoding.AdjacentTag)

    opt.Converters.Add converterOpt

    let DefaultSerializationConfig: SerializerConfig<'a> =
        { encode = fun (eventData: 'a) -> JsonSerializer.Serialize<'a>(eventData, opt)
          decode = fun (data: string) -> JsonSerializer.Deserialize<'a>(data, opt) }

module internal Serdes =

    open SerializationConfig

    let encode<'a> : 'a -> string = DefaultSerializationConfig.encode

    let decode<'a> : string -> 'a = DefaultSerializationConfig.decode
