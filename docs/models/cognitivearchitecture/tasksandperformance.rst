*********************
Tasks and performance
*********************

Tasking model
*************

In Symu, agents are task-based. A task is an action with a cost. Some of them don’t perform task, such as a book or a static website; others can perform task such as workers. You can specify if the agent can perform task on weekends or not, that can be useful when your organization is an enterprise.
There are also two main aspects of the model that you can configure: 

* limit
* capacity

Limit tasks
===========

You can limit the total number of tasks an agent can perform during the entire simulation.
You can also limit the number of simultaneous tasks an agent is performing. If you want to avoid multi-tasking, you can set the limit to one. It is also an easy way to create a pull-system.
When you allow multi-tasking, you can define the impact of context switching between tasks on capacity. In that case, multiple tasks may be in progress at the same time.

Cost of task and capacity of the agent
======================================

A task has a cost. To perform a task, an agent has capacity, re initialized at each new step. Each time an agent is performing a task, the capacity of the agent is decreased.

Learning model
**************
Agents have the capacity to learn new knowledge or information during the simulation.
There are different means to learn new knowledge: 

* learning from a source of information
* by interacting with another agent 
* by doing by itself.

This model is mainly defined by a rate of learning (learn rate and learn by doing rate). It defines how quickly an agent will learn new knowledge when interacting with other agents. 
With a rate of 0.01, if an agent has initially a knowledge of 0.5, after a learning, its knowledge will be 0.5 +0.01 = 0.51.

Learning by doing
=================

A special case is when an agent is doing by itself and gain knowledge. For that, agent must have a minimum of initial knowledge to do it by itself (knowledge threshold for doing) and it has a cost (cost factor), the associated task will take longer than if he already knew it, that is the cost of learning.