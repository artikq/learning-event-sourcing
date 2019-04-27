open Domain
open EventStore
open Projections
open Helpers

[<EntryPoint>]
let main _ =
  let eventStore : EventStore<Event> = EventStore.create()
  eventStore.append [ FlavourRestocked(Vanilla, 3) ]
  eventStore.append [ FlavourSold Vanilla ]
  eventStore.append [ FlavourSold Vanilla ]
  eventStore.append [ FlavourSold Vanilla
                      FlavourWentOutOfStock Vanilla ]
  eventStore.append [ FlavourSold Strawberry ]
  eventStore.append [ FlavourSold Strawberry ]
  let events = eventStore.get()
  events |> printEvents
  let sold : Map<Flavour, int> = events |> project soldFlavours
  printSoldFlavour Vanilla sold
  printSoldFlavour Strawberry sold
  0 // return an integer exit code
