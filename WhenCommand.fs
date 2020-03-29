module KinoBot.Commands


open System
open FSharp.Control.Tasks
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes


let nextSundayMidnight (today : DateTime) =
    let daysTilSunday = 7 - LanguagePrimitives.EnumToValue today.DayOfWeek
    today.AddDays(float daysTilSunday)


let s n = if n <> 1 then "s" else ""


let formatTimeSpan (t : TimeSpan) =
    if t.Days > 0 then
        sprintf "%d day%s, %d hour%s and %d minute%s"
            t.Days (s t.Days)
            t.Hours (s t.Hours)
            t.Minutes (s t.Minutes)
    else if t.Hours > 0 then
        sprintf "%d hour%s and %d minute%s"
            t.Hours (s t.Hours)
            t.Minutes (s t.Minutes)
    else if t.Minutes > 0 then
        sprintf "%d minute%s"
            t.Minutes (s t.Minutes)
    else
        sprintf "%d second%s"
            t.Seconds (s t.Seconds)


type WhenCommand () =
    [<Command "when">]
    member _.When(ctx : CommandContext) =
        let sundayMidnight = nextSundayMidnight DateTime.Today
        let t = sundayMidnight - DateTime.Now

        ctx.RespondAsync("Movie starts in " + formatTimeSpan t) |> Task.unitIgnore
