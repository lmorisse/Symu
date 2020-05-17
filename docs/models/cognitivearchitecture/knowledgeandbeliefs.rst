.. index:: Knowledge, Beliefs

*********************
Knowledge and beliefs
*********************

Symu is a multi-agent system for the co-evolution of agents and socio-cultural environments with an interaction model between agents. Each agent defines the way the agent interacts with other agents.
certain types of non-relational agents such as a database are not part of the sphere.
For those who are part of the interaction sphere, the sphere is computed depending on different parameters

Knowledge model
***************

Agents have the capacity to have initial knowledge, during the simulation (learning model). If an agent has initial knowledge, it is initialized randomly depending on the knowledge level. You can choose the level in a list from no knowledge to full knowledge.

Knowledge is defined by an array of bit of information between 0 and 1. The length of this array is a parameter between 0 to 100.

For example, the knowledge to use Symu can be defined as an array of 20 bits. If you don’t know anything about it, the array is filled with 0. If you have a full knowledge of it, it will be filled with 1.
Most of the case, the array will be filled randomly with 0/1 if the Binary Knowledge is chosen, or with float between [0; 1] otherwise.
