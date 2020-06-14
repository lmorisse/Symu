# Symu
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/lmorisse/symu?style=flat-square)](https://github.com/lmorisse/Symu/releases/latest)
[![Documentation Status](https://readthedocs.org/projects/symu/badge/?version=latest)](https://symu.readthedocs.io/en/latest/?badge=latest)

``Develop your own application to run virtual experiments with organizations`` and capture their dynamic behaviors, their evolutions.<br>
``Symu`` is a core of discrete-event multiagent simulation library in C#. It is designed to be the foundation for custom-purpose ``human-centered organization simulation``. Symu implements agnostic organizations as social groups to target the most general use cases.

## Organization as complex systems
Symu models groups and organizations as complex systems. It captures the variability in human, technological and organizational factors through heterogeneity in information processing capabilities, but also in knowledge and resources. Also, the non-linearity of the model generates complex temporal behavior due to dynamic relationships among agents.
Symu targets agnostic organizations as social groups and does not plan to implement functionality for specific types of organizations.<br>

Some useful links:
* [Website : symu.org](https://symu.org/)
* [Documentation : docs.symu.org](http://docs.symu.org/)
* [Code : github.symu.org](http://github.symu.org/)
* [Issues : github.symu.org/issues](http://github.symu.org/issues/)
* [Twitter : symuorg](https://twitter.com/symuorg)

## How it works

``Symu`` models groups and organizations as complex systems and captures the variability in human, technological and organizational factors through heterogeneity in information processing capabilities, knowledge and resources.<br>
The non-linearity of the model generates complex temporal behavior due to dynamic relationships among agents.

## What it is

``Symu`` is a multi-agent system, time based with discrete events, for the co-evolution of agents and socio-cultural environments.
Agents are decision-making units and can represent various levels of analysis such as individuals, groups or organizations.<br>
Agents are autonomous, rationally bounded and tasks based.<br>
They interact simultaneously in a shared environment that interacts in turn with the agents, via asynchronous messages.

## Why open source

Because we believe that such a framework is valuable for organizations and academics.

### Academic program

``Symu`` is based on a library of theoretical models:
organization theory, social networks, socio-cultural environment, information diffusion, socio-technical environment…

With our **academic program**, we will first implement models that you want to use for you.

## Getting Started
The main project is [Symu](https://github.com/lmorisse/Symu/tree/master/Symu%20source%20code/Symu). This is the framework you'll use to build your own application in batch. You can use [SymuForm](https://github.com/lmorisse/Symu/tree/master/Symu%20source%20code/SymuForm) for a GUI mode.

### Installing

Symu works only on Windows for the moment.

### Building

* [Math.net](https://www.math.net/)
* Some examples are using [SyncFusion](https://www.syncfusion.com/)

### Running

As it is a core library, you can't run Symu as is. The sample projects are interesting to understand what the framework can do. They are divided into models, for example [learning and forgetting models](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuLearnAndForget). They allow you to discover, understand and configure each model, in this sense it is a good entry point.
You can find executables files of those examples in the [latest release](https://github.com/lmorisse/Symu/releases/latest). 

## Contributors

See the list of [CONTRIBUTORS](CONTRIBUTORS.md) who participated in this project.

## Contributing

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on how you can contribute and the process for contributing.

## Code of conduct

Please read [CODE_OF_CONDUCT](CODE_OF_CONDUCT.md) for details on our code of conduct if you want to contribute.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/lmorisse/Symu/releases). 

## License

This project is licensed under the GNU General Public License v2.0 - see the [LICENSE](LICENSE) file for details

## Support

Paid consulting and support options are available from the corporate sponsors. See [Symu services](https://symu.org/services/)

## Integration

Symu is used in projects:
- [``Symu.biz``](https://symu.biz): an enterprise level implementation
