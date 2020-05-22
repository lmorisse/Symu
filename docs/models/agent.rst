.. index:: ! Agent, cognitive architecture

*****
Agent
*****
The agents in the model could be cells, teams, individuals, or they could be companies. The agents interact with one another.
In an agent-based simulation, you define agents, their characteristics like their behaviours, what information they can know and what state of the environment they inherit before they can act.

IN that way, each agent as a :doc:`cognitivearchitecture/cognitivearchitecture` which allow to set up agent's models.
This model is inspired by Construct specifications [#f1]_.

Each model can be turn off or on. When the model is on, you can choose the percentage of agents that are impacted by the model. If 0 is chosen, any agent will be impacted, if 1 is chosen every agent will be impacted.

Agent's models
**************

* Learning model
* Forgetting model
* Influence model
* Knowledge model
* Belief model

Learning Model
==============
Agents have the capacity to learn new knowledge or information during the simulation.
There are different means to learn new knowledge: 

* learning from a source of information
* by interacting with another agent
* by doing by itself

Forgetting model
================
The counterpart of learning is forgetting. Agents may forget knowledge or information if they are not solicited during the simulation. Using a bit of knowledge during a step is enough to be sure that this bit will not be forget today.

.. seealso::  :doc:`cognitivearchitecture/internalcharacteristics`

Influence model
===============
This model define how an agent will reinforce its belief or change its belief from influencer. Agents can accumulate beliefs during the simulation.
INfluence model is associated with the risk aversion that prevent an agent from acting on a particular belief.

.. seealso::  :doc:`cognitivearchitecture/knowledgeandbeliefs`

Knowledge model
===============

.. note:: to be done

Beliefs model
===============

.. note:: to be done



.. rubric:: Footnotes

.. [#f1] "Specifying agents in Construct", Carley, 2007