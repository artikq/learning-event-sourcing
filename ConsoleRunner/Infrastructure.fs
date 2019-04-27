module Infrastructure

type EventStore<'event> =
  { get : unit -> 'event list
    append : 'event list -> unit }

module EventStore =
  type Msg<'event> =
    | Append of 'event
    | Get of AsyncReplyChannel<'event>

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
          }
        loop [])

    let append events = agent.Post(Append events)
    let get() = agent.PostAndReply Get
    { get = get
      append = append }

type Projection<'state, 'event> =
  { init : 'state
    update : 'state -> 'event -> 'state }
