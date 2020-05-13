************************
Internal characteristics 
************************

Model of forgetting
*******************
The counterpart of learning is forgetting. Agents may forget knowledge or information if they are not solicited during the simulation. Using a bit of knowledge during a step is enough to be sure that this bit will not be forget today.
Forgetting has different modes: 

* it could be random 
* based on the age of the information

Randomly selected
=================

You define a probability to forget, combined with the length of the knowledge, it will define the number of bits that will be forget during the day. If this rate = 0, agent will forget any bits of knowledge; if rate =1, every bit will be affected by the knowledge.

.. topic:: Example

    If you don’t use Symu, with a forgetting rate of 0.1, you may loose 0.1* 20 bits = 2 bits of knowledge today.
    
Standard deviation define the level of randomness you want around the forgetting rate.

Oldest knowledge
================

If the oldest knowledge mode is selected, only the oldest knowledge is eligible to be forgotten. It is based on it’s TimeToLive parameter. If set to -1, information will last during the simulation, otherwise it is compared with the age if the information to decide to forget it or not.

Partial forgetting
==================

Then you must define the how every bit of knowledge is affected. If partial forgetting is chosen, the partial forgetting rate define how much an agent loose at each step, otherwise, the bit is completely forgotten.
You can define the minimum level of information that left for each bit.
