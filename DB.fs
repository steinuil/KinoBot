module KinoBot.DB

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open FSharp.Data
open Microsoft.FSharp

[<Literal>]
let votesFile = "./db/votes.json"
[<Literal>]
let votesSchema = "./db/votesSchema.json"


type VotesDB = JsonProvider<votesSchema, RootName="movie">


let clearFile (filename : string) =
    File.WriteAllText(filename, "[]")


let loadOrCreateFile (filename : string) =
    try
        VotesDB.Load(filename)
    with :? FileNotFoundException ->
        clearFile filename
        [| |]


let currentVotes () =
    loadOrCreateFile votesFile


type NotMovie = {
    Title : String
    Id : String
    Votes : String []
    }


let writeVoteMovies (movies : seq<VotesDB.Movie>) =
    let formattedMovies = movies
                            |> Seq.map (fun movie -> movie.JsonValue.ToString())
                            |> String.concat ","
    let resultString = sprintf "[%s]" formattedMovies
    File.WriteAllText(votesFile, resultString)


(*
    Adds a movie to the list of movies users can vote for
    Returns true if the movie is not already present, otherwise false
*)
let addVoteMovie (title : string, id : string) =
    if currentVotes()
        |> Seq.exists (fun vote -> vote.Id = id) then
        false
    else
        let vote = VotesDB.Movie(title, id, [| |])
        writeVoteMovies(vote :: Array.toList(currentVotes()))
        true


(*
    Adds a vote for a movie
    Users can only vote once so we need the user name to check
    Returns true if the movie is in the list and the user hasn't already voted, otherwise false
*)
let voteForMovie (id : string, user : string) =
    let movie = currentVotes() |> Seq.tryFind (fun v -> v.Id = id)
    if movie.IsNone || Seq.contains user movie.Value.Votes then
        false
    else
        let newMovie = VotesDB.Movie
                        ( title = movie.Value.Title,
                          id = movie.Value.Id,
                          votes = Array.append movie.Value.Votes [|user|] )
        let newMovies = newMovie :: (currentVotes()
                                        |> Seq.filter (fun m -> m.Id <> id)
                                        |> Seq.toList)
        writeVoteMovies newMovies
        true

