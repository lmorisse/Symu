********************
Interaction patterns
********************

Symu is a multi-agent system for the co-evolution of agents and socio-cultural environments with an interaction model between agents. Each agent defines the way the agent interacts with other agents.
certain types of non-relational agents such as a database are not part of the sphere.
For those who are part of the interaction sphere, the sphere is computed depending on different parameters

Interaction sphere
******************

It is based on different parameters:

* Relative knowledge
* Relative activities
* Relative beliefs
* Social demographics

Each parameter has an associated weight to calculate the homophily factor.

To interact with another agent, an agent filter the interaction sphere with an interaction strategy based on:

* Relative knowledge
* Relative activities
* Relative beliefs
* Social demographics
* Homophily

If the interaction value between two agents is below the average of the interaction sphere it is considered as a **new interaction**.
In that case, when the two agents try to interact, they go through the **new interaction process**.

Sphere initialization
*********************

This model defines the way the interaction sphere is initialized and can change over time.
The interaction sphere may be initialized in two ways: similarity matching and random generation.

Similarity matching
===================

In that case, the interaction sphere is generated based on known informations about agents (knowledge, activities, beliefs, socio-demographics).

Random generation
=================

This option can be usefull when you don't want to define nowledge, beliefs, ... It is based on parameters such as min/max density of the sphere. This specifies the proportion of agents who can appear in a given agent's interaction sphere.
If an agent appears in the interaction sphere, its weigth is randomly generated.

Sphere evolution
****************
During the simulation, the interaction sphere is updated with the updated agent's informations. As the action is time consuming and based on the fact that knowledge, beliefs, ... don't evolve too frequently, you can adjust the frequency of update every day, every week, every month, enevry year.

The sphere size may change over time if new agents are on boarding or agents are leaving the simulation.

New interaction process
=======================

To be able to initiate a new interaction, an agent must have the right to do it (parameter AllowNewInteractions). The number of new interactions per step may be limited (parameters LimitNumberOfNewInteractions and MaxNumberOfNewInteractions).
When an agent receive a demand for a new interaction, he answer randomly (depending on the parameter ThresholdForNewInteraction).

