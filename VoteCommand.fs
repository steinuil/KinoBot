module KinoBot.VoteCommand


open System
open FSharp.Control.Tasks
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes


// TODO: contatenate the movie title which have several words
// e.g : [|"🏙", "Die", "Hard", "⭐", "Shrek", "2"|] -> [|"🏙", "Die Hard", "⭐", "Shrek 2"|]
let parseMovieTitles (args : string[]) =
    args


let formatMovieVote (emoji : string, title : string) =
    let displayEmoji = emoji.[0]
    sprintf "%s %c" title displayEmoji


type VoteCommand () =
    [<Command "vote">]
    member _.Vote(ctx : CommandContext, [<ParamArray>]movies : string[]) =
        let movieVotes = movies
                                |> parseMovieTitles
                                |> Array.pairwise
                                |> Array.map formatMovieVote
                                |> String.concat Environment.NewLine
        ctx.RespondAsync("Vote for the next movie: " + Environment.NewLine + movieVotes)
        |> Task.unitIgnore
