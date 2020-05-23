======================
Development guidelines
======================


Principles
==========

* Symu is written in C#.
* Engine and models are fully separated from visualization. You can easily run a model without visualization.
* It is written today to run on a single laptop. It is designed to be efﬁcient when running in a single process, albeit with multiple threads. It requires a single uniﬁed memory space, and has no facilities for distributing models over multiple processes or multiple computers.
* Symu is modular and consistent. There is a high degree of separation and independence among elements of the system. 
* It also is written in very carefully written C#, with an eye ﬁrmly ﬁxed towards efﬁciency. 