**********
Simulation
**********

How it work
***********
The simulation method is known as an "agent-based model" or multiagent system which is a type of computer simulation approach that is used in biological sciences and the social sciences as a way to model systems more holistically.
The agents in the model could be cells, teams, individuals, or they could be companies. The agents interact with one another.
In this approach, properties can emerge of the system during the simulation. These result from many discrete local interactions between the agents that add up to a new system state.

.. seealso:: :doc:`agent`

Engine
******
The engine of this simulator offers the possibility to performe or multiple iterations. Multiple iterations is useful in a Monte Carlo approach.
This method is aiming to calculate an approximate numerical value using random methods.

Scenarios
*********
To run a simulation, you must at least define one scenario. it will basically allow to define which parameter allows to stop the simulation and for which value.
The pre-defined scenarios are:
* Time based
* Tasks based
* Messages based
You can overload your own scenario.
A simulation may be define with multiple scenarios. In that case, the simulation will stop when all the scenarios will be stopped.

Events
******
To add variability, you can schedule events during the simulation. You will find by design one shot, cyclical and random events.

.. topic:: Examples

    You can add a new agent every 100 steps, or randomly add an event that affect the beliefs of the agents.

You can only use scenarios if you choose. But the scenarios and the events are different by design. The scenario is an agent, has its own life cycle, interacts with other agents via asynchronous messages; where events is a simple class that can be used directly by the agent via an EventHandler.

At least one scenario is required to run a simulation, then you choose what best suits your needs for variability between scenarios and events.

.. seealso:: :doc:`../userguides/symuscenariosandevents`  

Models
******
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