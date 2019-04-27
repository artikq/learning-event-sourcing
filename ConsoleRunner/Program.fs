open Domain
open EventStore
open Projections
open Helpers

module Behaviour =
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

[<EntryPoint>]
let main _ =
  let eventStore : EventStore<Event> = EventStore.create()
  eventStore.evolve (Behaviour.restockFlavour Vanilla 5)
  eventStore.evolve (Behaviour.restockFlavour Strawberry -1)
  eventStore.evolve (Behaviour.sellFlavour Vanilla)
  eventStore.evolve (Behaviour.sellFlavour Vanilla)
  eventStore.evolve (Behaviour.sellFlavour Vanilla)
  eventStore.evolve (Behaviour.sellFlavour Strawberry)
  let events = eventStore.get()
  events |> printEvents
  let sold : Map<Flavour, int> = events |> project soldFlavours
  printSoldFlavour Vanilla sold
  printSoldFlavour Strawberry sold
  printProjection flavoursInStock events "Flavours In stock"
  0 // return an integer exit code
