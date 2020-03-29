module KinoBot.Program


open Argu
open System.Threading.Tasks
open FSharp.Control.Tasks
open DSharpPlus
open DSharpPlus.CommandsNext


type Arguments =
    | [<Mandatory>] Token of string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Token _ -> "Discord bot token"


let parser = ArgumentParser.Create<Arguments>(programName = "KinoBot")


let mainTask token =
    let conf = DiscordConfiguration()
    conf.set_Token(token)
    conf.set_TokenType(TokenType.Bot)
    conf.set_UseInternalLogHandler(true)
    conf.set_LogLevel(LogLevel.Debug)

    let discord = new DiscordClient(conf)

    let commandsConf = CommandsNextConfiguration()
    commandsConf.set_StringPrefix("!")

    let commands = discord.UseCommandsNext(commandsConf)
    commands.RegisterCommands<Commands.WhenCommand>()

    task {
        do! discord.ConnectAsync()
        do! Task.Delay(-1)
    }


[<EntryPoint>]
let main args =
    try
        let args = parser.Parse args
        (mainTask <| args.GetResult Token).GetAwaiter().GetResult()
        0
    with :? ArguParseException as exn ->
        eprintfn "%s" exn.Message
        1
