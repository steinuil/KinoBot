module FSharp.Control.Tasks.Task


open System.Threading.Tasks


let ignore (_ : Task<'a>) = task { () }


let unitIgnore (_ : Task<'a>) = task { () } :> Task
