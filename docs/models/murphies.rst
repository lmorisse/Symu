.. index:: murphy

======
Murphy
======

We design an optimal operating system. But under stress conditions, we have a suboptimal operating system.
A murphy is an event that create a stress condition:

 * It's a murphy when the stress has an internal source
 * It's a mayday when the stress has an external source

A murphy contains the conditions for triggering the stress, but also the conditions for unlocking the blocker. It is fully customizable.

This section defines the murphies used by Symu :

 * Unavailability of the agent
 * Incomplete knowledge
 * Incomplete beliefs
 * Incomplete information

Unavailability murphy
*********************

This murphy defines unplanned :index:`unavailability` of agent such as illness, ...
This murphy has an impact on the agent's initial capacity.
It should not be confused with Agent.Cognitive.InteractionPatterns.AgentCanBeIsolated which deals with plannable unavailability (even if it can be randomly generated, such as holidays).

Incomplete knowledge
********************

Agent is task-based. Agent may not have all the knowledge necessary to perform a task.
If so, the task may be blocked or complete incorrectly.

Incomplete beliefs
******************

The agent has beliefs. The agent may not have enough beliefs to complete a task. The agent can seek advice from other agents to confirm or not his convictions. Talking with other agents can influence his opinions.
If so, the task may be cancelled, blocked or terminate incorrectly. This will strengthen his convictions.

Incomplete information
**********************

To perform a task, agent must have enough information. If this is not the case, agent ask the creator of the task more information.
The creator may answer or not, and may take some times to answer. 