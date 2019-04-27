module Behaviour

open Domain
open Projections

let restockFlavour flavour count _ =
  match count with
  | x when x < 1 -> [ FlavourRestockFailed(flavour, x) ]
  | _ -> [ FlavourRestocked(flavour, count) ]

let sellFlavour flavour events =
  let stock =
    events
    |> project flavoursInStock
    |> getFlavourStock flavour
  match stock with
  | 0 -> [ FlavourWasNotInStock flavour ]
  | 1 ->
    [ FlavourSold flavour
      FlavourWentOutOfStock flavour ]
  | _ -> [ FlavourSold flavour ]
