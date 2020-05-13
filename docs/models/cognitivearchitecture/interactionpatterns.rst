********************
Interaction patterns
********************

Each agent defines the way the agent interacts with other agents.
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

To interact with another agent, a filter agent with interaction sphere with an interaction strategy based on:

* Relative knowledge
* Relative activities
* Relative beliefs
* Social demographics
* Homophily

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
During the simulation, the interaction sphere is updated with the updated agent's informations. As the action time consuming and based on the fact that knowledge, beliefs, ... don't evolve too frequently, you can adjust the frequency of update every day, every week, every month, enevry year.

The sphere size may change over time if new agents are on boarding or agents are leaving the simulation.

