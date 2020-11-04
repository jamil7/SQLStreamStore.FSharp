namespace SqlStreamStore.FSharp.Postgres

open SqlStreamStore.FSharp

type PostgresConfig =
    { host: string
      port: string
      username: string
      password: string
      database: string
      schema: string option }

module Postgres =
    let private settingsStringFromConfig (config: PostgresConfig): string =
        sprintf
            "Host=%s;Port=%s;User Id=%s;Password=%s;Database=%s"
            config.host
            config.port
            config.username
            config.password
            config.database

    let private setSchema (schema: string option)
                          (settings: SqlStreamStore.PostgresStreamStoreSettings)
                          : SqlStreamStore.PostgresStreamStoreSettings =
        match schema with
        | None -> ()
        | Some schema -> settings.Schema <- schema

        settings

    /// Connects to a postgres database given a configuration record and an optional schema.
    /// If no schema is provided the tables will be created directly in the public one.
    let connect (config: PostgresConfig) (schema: string option): SqlStreamStore.PostgresStreamStore =
        let storeSettings =
            SqlStreamStore.PostgresStreamStoreSettings(settingsStringFromConfig config)
            |> setSchema schema

        new SqlStreamStore.PostgresStreamStore(storeSettings)

    /// Connects to a postgres database given a Npgsql configuration string and an optional schema.
    /// If no schema is provided the tables will be created directly in the public one.
    let createStoreWithConfigString (config: string) (schema: string option): SqlStreamStore.PostgresStreamStore =
        let storeSettings =
            SqlStreamStore.PostgresStreamStoreSettings(config)
            |> setSchema schema

        new SqlStreamStore.PostgresStreamStore(storeSettings)

    /// Creates messages, and streams tables that house the data.
    /// Can throw exceptions.
    let createSchemaRaw (store: SqlStreamStore.PostgresStreamStore): Async<unit> =
        async {
            return! store.CreateSchemaIfNotExists()
                    |> Async.awaitTaskWithInnerException'
        }

    /// Creates messages, and streams tables that house the data.
    let createSchema (store: SqlStreamStore.PostgresStreamStore): Async<Result<unit, exn>> =
        createSchemaRaw store
        |> ExceptionsHandler.asyncExceptionHandler
