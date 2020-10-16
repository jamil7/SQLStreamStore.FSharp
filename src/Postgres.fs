namespace SqlStreamStore.FSharp.Postgres

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string }

type PoolingConfig =
    { minSize: int option
      maxSize: int option
      connectionIdleLifeTime: int option
      connectionPruningInterval: int option }

module Postgres =
    let private storeSettings (config: PostgresConfig): string =
        sprintf
            "Host=%s;Port=%s;User Id=%s;Password=%s;Database=%s"
            config.host
            config.port
            config.username
            config.password
            config.database

    let private poolingSettings (pooling: PoolingConfig): string =
        let minSize = pooling.minSize |> Option.defaultValue 0

        let maxSize =
            pooling.maxSize |> Option.defaultValue 100

        let connectionPruningInterval =
            pooling.connectionPruningInterval
            |> Option.defaultValue 300

        let connectionIdleLifeTime =
            pooling.connectionIdleLifeTime
            |> Option.defaultValue 10

        sprintf
            "Minimum Pool Size=%d;Maximum Pool Size=%d;Connection Idle Lifetime=%d;Connection Pruning Interval%d"
            minSize
            maxSize
            connectionPruningInterval
            connectionIdleLifeTime

    let private storeSettingsWithPooling (config: PostgresConfig) (pooling: PoolingConfig): string =
        sprintf "%s;%s" (storeSettings config) (poolingSettings pooling)

    let createStore (config: PostgresConfig): SqlStreamStore.PostgresStreamStore =
        new SqlStreamStore.PostgresStreamStore(SqlStreamStore.PostgresStreamStoreSettings(storeSettings config))

    let createStoreWithPoolingConfig (config: PostgresConfig) (pooling: PoolingConfig): SqlStreamStore.PostgresStreamStore =
        new SqlStreamStore.PostgresStreamStore(SqlStreamStore.PostgresStreamStoreSettings
                                                   (storeSettingsWithPooling config pooling))

    let createSchema (store: SqlStreamStore.PostgresStreamStore): Async<unit> =
        store.CreateSchemaIfNotExists() |> Async.AwaitTask
