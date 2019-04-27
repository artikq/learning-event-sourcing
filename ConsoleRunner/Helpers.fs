module Helpers

open Projections

let printList list =
  list |> List.iteri (fun i item -> printfn "  %i: %A" (i + 1) item)

let printEvents events =
  events
  |> List.length
  |> printfn "History (Length: %i)"
  events |> printList

let printSoldFlavour flavour map =
  map
  |> countFlavour flavour
  |> printfn "%A : %i" flavour

let printProjection projection events name =
  events
  |> project projection
  |> printfn "Projection %s is %A" name
