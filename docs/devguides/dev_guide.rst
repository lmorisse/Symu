==============================================
Welcome to the technical guide for developpers
==============================================

Symu is a core of discrete-event multiagent simulation library in C#. It is designed to be the foundation for custom-purpose organization simulations. It also to provides enough functionality for many simulation needs. Symu implements agnostic organizations as social groups to target the most general use cases.
There are three main classes to discover this framework:

* :index:`SymuEngine`
* :index:`SymuEnvironment`
* :index:`SymuAgent`

SymuEngine
==========


SymuEnvironment
===============
SymuEnvironment forms the basis for the representation of all environmental dynamics and structure that enables agents inetractions.
Symu is a time-based simulator. At each step, SymuEnvironment randomly send messages about the time to the agents.
It contains a scheduler. The Schedule is your simulation’s representation of time.

Agent
=====

Agent life cycle
################
Agent is created, its state is NotStarted. You can setup its cognitivearchitecture at this stage via SetCognitive() method, or by code when unit testing.
When launching the simulation, Agent.Start() is called.
During the starting phase, a BeforeStart() method is called which is more dedicated to Models via SetModels() method.
Models often are based on the CognitiveArchitecture, that why they are define at the latest moment.
When the agent's state is started, you can manipulate the default and specific models of the agent. 

.. seealso::  for a more functional definition of agent, you can read :doc:`../models/agent`


.. toctree::
    :maxdepth: 2
    :caption: Cognitive architecture:
   
   cognitivearchitecture/interactionpatterns
   cognitivearchitecture/internalcharacteristics
   cognitivearchitecture/knowledgeandbeliefs
   cognitivearchitecture/tasksandperformance
   cognitivearchitecture/interactioncharacteristics