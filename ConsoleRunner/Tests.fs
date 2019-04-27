module Tests

open Expecto
open Expecto.Expect
open Domain
open Behaviour

module Impl =
  let Given = id
  let When action input = action input
  let Then expected message result = equal expected result message
  let tests =
    testList "Sell Flavour" [ test "FlavourSold happy path" {
                                Given [ FlavourRestocked(Vanilla, 3) ]
                                |> When(sellFlavour Vanilla)
                                |> Then [ FlavourSold Vanilla ]
                                     "new events should equal expected events"
                              }
                              test "FlavourSold fails with empty stock" {
                                Given []
                                |> When(sellFlavour Vanilla)
                                |> Then [ FlavourWasNotInStock Vanilla ] ""
                              } ]

let run() = runTests defaultConfig Impl.tests |> ignore
