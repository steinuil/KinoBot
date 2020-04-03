module KinoBot.Commands


open System
open FSharp.Control.Tasks
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes


type ScheduledTime = {
    day : DayOfWeek
    offsetHours : float Option
}


let rootTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time")


let days = [DayOfWeek.Sunday; DayOfWeek.Saturday]


let convertTimeToTimeZone timeZone time =
    let time = DateTime.SpecifyKind(time, DateTimeKind.Unspecified)
    TimeZoneInfo.ConvertTimeFromUtc(time, timeZone)


let daysTilNextDayOfWeek (dayOfWeek : DayOfWeek) (today : DayOfWeek) =
    let dayOfWeek = LanguagePrimitives.EnumToValue dayOfWeek
    let today = LanguagePrimitives.EnumToValue today
    match (7 - today + dayOfWeek) % 7 with 0 -> 7 | n -> n


let nextDayOfWeek day (today : DateTime) =
    let days = daysTilNextDayOfWeek day today.DayOfWeek
    today.AddDays(float days)


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
        let today = DateTime.Today |> convertTimeToTimeZone rootTimeZone
        let now = DateTime.Now |> convertTimeToTimeZone rootTimeZone
        let nextMovie =
            days
            |> List.map (fun day -> nextDayOfWeek day today)
            |> List.min
        let t = nextMovie - now

        printfn "User %s requested movie time" ctx.User.Username

        ctx.RespondAsync("Movie starts in " + formatTimeSpan t) |> Task.unitIgnore
