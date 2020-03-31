module KinoBot.VoteCommand


open System
open FSharp.Control.Tasks
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open KinoBot


let defaultReply = "That's not how you use the vote command you triple-aho"


let formatMovieVote (movie : DB.VotesDB.Movie) =
    sprintf "%s %s : %d votes"
        movie.Id
        movie.Title
        movie.Votes.Length


let displayCurrentVotes () =
    let movieVotes = DB.currentMoviesForVote()
                        |> Array.sortByDescending (fun movie -> movie.Votes.Length)
                        |> Seq.map formatMovieVote
                        |> String.concat Environment.NewLine
    "Vote for the next movie: " + Environment.NewLine + movieVotes


let voteForMovie (id : string, user : string) =
    match DB.voteForMovie(id, user) with
    | true -> "Duely noted!"
    | false -> "No you stupid baka, you have already voted"


let addVoteMovie (movieArgs : string list) =
    let id = movieArgs.[0]
    let title = movieArgs |> Seq.skip 1 |> String.concat " "
    match DB.addVoteMovie(title, id) with
    | true -> sprintf "Added the movie %s (%s) to the votes" title id
    | false -> "The movie is already in the list you dismal retard"

(*
    Usage of the vote command:
    !vote => displays the current vote results
    !vote add ⭐ Shrek =>  adds Shrek to the movies to vote for
    !vote ⭐ => votes for Shrek
*)
type VoteCommand () =
    [<Command "vote">]
    member _.Vote(ctx : CommandContext, [<ParamArray>]args : string []) =
        let reply = match Seq.toList args with
                    | [] -> displayCurrentVotes()
                    | [ id ] -> voteForMovie(id, ctx.Member.Username)
                    | "add" :: movieArgs -> addVoteMovie movieArgs
                    | _ -> defaultReply
        ctx.RespondAsync(reply) |> Task.unitIgnore
