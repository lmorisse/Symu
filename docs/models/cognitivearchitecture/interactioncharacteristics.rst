***************************
Interaction characteristics
***************************

Messaging is the only way agents communicate to each other. So messaging model represents the interaction mechanism of agents. This is one of the means of ensuring the autonomy of agents. A message can have different types: it can be a phone call, a meeting, an email, …
Interaction characteritics model deinfes the way an agent will interact with other agents.
There are two main aspects of the model that you can configure: 

* limit
* cost

Limit messages
**************

You can limit the total number of messages an agent can send or received during a step; you can also specify the number of messages sent or the number of receptions per agent per step.
It can be useful when you want to test the limitation of interaction between agents, whatever the interaction.

Cost of the messages
********************

Sending or receiving a message can have a cost that depends of the type of the message. The cost may be a fraction of the capacity of an agent. For example, for an agent with a capacity of 1, going to a meeting of two hours, the cost to send the message may be equal to 0 (sending invitations) and the cost to receive the message may be equal to 0.25 (doing the meeting).
