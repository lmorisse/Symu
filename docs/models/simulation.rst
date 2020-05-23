**********
Simulation
**********

How it work
***********
The simulation method is known as an "agent-based model" or multiagent system which is a type of computer simulation approach that is used in biological sciences and the social sciences as a way to model systems more holistically.
The agents in the model could be cells, teams, individuals, or they could be companies. The agents interact with one another.
In this approach, properties can emerge of the system during the simulation. These result from many discrete local interactions between the agents that add up to a new system state.

.. seealso:: :doc:`agent`

Models and mechanisms
*********************
``Symu`` is a time-based.  It contains a discrete event schedule on which you can schedule various agents to be called at some specific time.
But it is ont space-based, it has no 2D nor 3D features to localize agents in the space.
Of course, Simulation is at the heart of this framework. It is based on two asynchronous models:

* Messaging between agents
* Tasks for each agent

Messaging model
===============
Messaging is the only way agents communicate to each other. So messaging model represents the interaction mechanism of agents. This is one of the means of ensuring the autonomy of agents. A message can have different types: it can be a phone call, a meeting, an email, …

Each type of message can be configure via templates.

.. note:: to be done

.. toctree::
   :maxdepth: 2
   :caption: Each agent can configure its messaging model:
   
   cognitivearchitecture/messagecontent
   cognitivearchitecture/interactioncharacteristics

Tasking model
=============

In Symu, agents are task-based. A task is an action with a cost. Some of them don’t perform task, such as a book or a static website; others can perform task such as workers. You can specify if the agent can perform task on weekends or not, that can be useful when your organization is an enterprise.

.. toctree::
   :maxdepth: 2
   :caption: Each agent can configure its tasking model:
   
   cognitivearchitecture/tasksandperformance