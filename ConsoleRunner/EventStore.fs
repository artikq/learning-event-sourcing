module EventStore

type EventProducer<'event> = 'event list -> 'event list

type EventStore<'event> =
  { get : unit -> 'event list
    append : 'event list -> unit
    evolve : EventProducer<'event> -> unit }

type Msg<'event> =
  | Append of 'event list
  | Get of AsyncReplyChannel<'event list>
  | Evolve of EventProducer<'event>

let create() : EventStore<'event> =
  let agent =
    MailboxProcessor.Start(fun inbox ->
      let rec loop state =
        async {
          let! msg = inbox.Receive()
          match msg with
          | Append events -> return! loop (state @ events)
          | Get channel ->
            channel.Reply state
            return! loop state
          | Evolve producer ->
            let newEvents = producer state
            return! loop (state @ newEvents)
        }
      loop [])

  let append events = agent.Post(Append events)
  let get() = agent.PostAndReply Get
  let evolve eventProducer = agent.Post(Evolve eventProducer)
  { get = get
    append = append
    evolve = evolve }
