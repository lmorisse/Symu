.. index:: murphy

======
Murphy
======

We design an optimal operating system. But under stress conditions, we have a suboptimal operating system.
A murphy is an event that create a stress condition:

 * It's a murphy when the stress has an internal source
 * It's a mayday when the stress has an external source

A murphy contains the conditions for triggering the murphy, but also the conditions for unlocking the blocker.

This section defines the murphies used by Symu :
 * Unavailability
 * Incomplete knowledge
 * Incomplete beliefs

Unavailability murphy
*********************

This murphy defines unplanned :index:`unavailability` of agent such as illness, ...
This murphy has an impact on the agent's initial capacity.
It should not be confused with Agent.Cognitive.InteractionPatterns.AgentCanBeIsolated which deals with plannable unavailability (even if it can be randomly generated, such as holidays).

Incomplete knowledge
********************

Agent is task-based. Agent may not have all the knowledge necessary to perform a task.
If so, the task may be blocked or complete incorrectly.