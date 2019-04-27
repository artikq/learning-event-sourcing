open Domain
open EventStore
open Projections
open Behaviour
open Helpers

[<EntryPoint>]
let main _ =
  Tests.run()
  let eventStore : EventStore<Event> = EventStore.create()
  eventStore.evolve (restockFlavour Vanilla 5)
  eventStore.evolve (restockFlavour Strawberry -1)
  eventStore.evolve (sellFlavour Vanilla)
  eventStore.evolve (sellFlavour Vanilla)
  eventStore.evolve (sellFlavour Vanilla)
  eventStore.evolve (sellFlavour Strawberry)
  let events = eventStore.get()
  events |> printEvents
  let sold : Map<Flavour, int> = events |> project soldFlavours
  printSoldFlavour Vanilla sold
  printSoldFlavour Strawberry sold
  printProjection flavoursInStock events "Flavours In stock"
  0 // return an integer exit code
