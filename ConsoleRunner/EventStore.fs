module EventStore

type EventProducer<'event> = 'event list -> 'event list

type Aggregate = System.Guid

type EventStore<'event> =
  { get : unit -> Map<Aggregate, 'event list>
    getStream : Aggregate -> 'event list
    append : Aggregate -> 'event list -> unit
    evolve : Aggregate -> EventProducer<'event> -> unit }

type Msg<'event> =
  | Append of Aggregate * 'event list
  | GetStream of Aggregate * AsyncReplyChannel<'event list>
  | Get of AsyncReplyChannel<Map<Aggregate, 'event list>>
  | Evolve of Aggregate * EventProducer<'event>

let findEvents aggregate state =
  state |> Map.tryFind aggregate |> Option.defaultValue []

let addEvents aggregate state events =
  let existing = state |> findEvents aggregate
  Map.add aggregate (existing @ events) state

let create() : EventStore<'event> =
  let agent =
    MailboxProcessor.Start(fun inbox ->
      let rec loop state =
        async {
          let! msg = inbox.Receive()
          match msg with
          | Append(aggregate, events) -> return! loop (addEvents aggregate state events)
          | GetStream(aggregate, channel) ->
            let events = findEvents aggregate state
            channel.Reply events
            return! loop state
          | Get channel ->
            channel.Reply state
            return! loop state
          | Evolve(aggregate, produceEvents) ->
            let newState = state |> findEvents aggregate |> produceEvents |> addEvents aggregate state
            return! loop (newState)
        }
      loop Map.empty)

  let append aggregate events = agent.Post(Append(aggregate, events))
  let get() = agent.PostAndReply Get
  let evolve aggregate eventProducer = agent.Post(Evolve(aggregate, eventProducer))
  let getStream aggregate = agent.PostAndReply(fun channel -> GetStream(aggregate, channel))
  { get = get
    getStream = getStream
    append = append
    evolve = evolve }
