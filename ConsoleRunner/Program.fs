open Domain
open EventStore
open Projections
open Behaviour
open Helpers

[<EntryPoint>]
let main _ =
  Tests.run()
  let eventStore : EventStore<Event> = EventStore.create()

  let truck1 = System.Guid.NewGuid()
  let truck2 = System.Guid.NewGuid()

  eventStore.evolve truck1 (restockFlavour Vanilla 5)
  eventStore.evolve truck1 (restockFlavour Strawberry -1)
  eventStore.evolve truck1 (sellFlavour Vanilla)
  eventStore.evolve truck1 (sellFlavour Vanilla)
  eventStore.evolve truck1 (sellFlavour Vanilla)
  eventStore.evolve truck1 (sellFlavour Strawberry)

  let eventsForTruck1 = eventStore.getStream truck1
  let eventsForTruck2 = eventStore.getStream truck2

  eventsForTruck1 |> printEvents
  eventsForTruck2 |> printEvents

  let sold : Map<Flavour, int> = eventsForTruck1 |> project soldFlavours

  printSoldFlavour Vanilla sold
  printSoldFlavour Strawberry sold

  printProjection flavoursInStock eventsForTruck1 "Flavours In stock"
  0 // return an integer exit code
