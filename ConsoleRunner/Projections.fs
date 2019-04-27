module Projections
open Domain

type Projection<'state, 'event> =
  { init : 'state
    update : 'state -> 'event -> 'state }

let project (projection: Projection<_, _>) events =
  events |> List.fold projection.update projection.init


let soldOfFlavour flavour state =
  state
  |> Map.tryFind flavour
  |> Option.defaultValue 0

let updateSoldFlavours state event =
  match event with
  | FlavourSold flavour ->
    state
    |> soldOfFlavour flavour
    |> fun portions -> state |> Map.add flavour (portions + 1)
  | _ -> state

let soldFlavours: Projection<Map<Flavour, int>, Event> =
  {
    init = Map.empty
    update = updateSoldFlavours
  }