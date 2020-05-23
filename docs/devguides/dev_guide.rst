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

SymuAgent
=========

.. seealso::  for a more functional definition of agent, you can read :doc:`../models/agent`


.. toctree::
    :maxdepth: 2
    :caption: Cognitive architecture:
   
   cognitivearchitecture/interactionpatterns
   cognitivearchitecture/internalcharacteristics
   cognitivearchitecture/knowledgeandbeliefs
   cognitivearchitecture/tasksandperformance
   cognitivearchitecture/interactioncharacteristics