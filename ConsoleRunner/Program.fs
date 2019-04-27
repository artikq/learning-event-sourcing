open Domain
open Infrastructure
open Helpers

[<EntryPoint>]
let main _ =
  let eventStore : EventStore<Event> = EventStore.create()
  eventStore.append [ FlavourRestocked(Vanilla, 3) ]
  eventStore.append [ FlavourSold Vanilla ]
  eventStore.append [ FlavourSold Vanilla ]
  eventStore.append [ FlavourSold Vanilla
                      FlavourWentOutOfStock Vanilla ]
  eventStore.get() |> printEvents
  0 // return an integer exit code
