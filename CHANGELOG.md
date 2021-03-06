# Changelog

## Unreleased

## 1.1.0 [☰](https://github.com/lmorisse/symu/compare/v1.1.0..v1.0.0) by [lmorisse](https://github.com/lmorisse)
* Extract system dynamics feature to new Symu.SysDyn project

## 1.0.0 [☰](https://github.com/lmorisse/symu/compare/v1.0.0..v0.9.1) by [lmorisse](https://github.com/lmorisse)
* SymuTools refactored in Symu.Common solution
* Extract organizational network analysis to new Symu.DNA project
* Extract organizational modeling to new Symu.OrgMod project

## 0.9.1 [☰](https://github.com/lmorisse/symu/compare/v0.9.1..v0.9.0) by [lmorisse](https://github.com/lmorisse)
* Split Agent into ReactiveAgent and CognitiveAgent

## 0.9.0 [☰](https://github.com/lmorisse/symu/compare/v0.9.0..v0.8.0) by [lmorisse](https://github.com/lmorisse)
* Remove InteractionSphereModel.FrequencyOfSphereUpdate
* Schedule Bug fixe 
* AgentEntity.Parent is now an AgentId
* Knowledge and Beliefs result are in percentage to be easily understandable. Symu examples are updated
* Update Task.Cancel
* InternalCharacteristics.RiskAversionLevel is non a GenericLevel to make it easier, and the model is a little bit more tolerant

## 0.8.0 [☰](https://github.com/lmorisse/symu/compare/v0.8.0..v0.7.0) by [lmorisse](https://github.com/lmorisse)
* Refactoring IterationResult
* Add LearningModel.OnAfterLearning event
* Refactoring Agent constructor and CommunicationTemplates
* Add CyclicalIsolation)
* Add PromoterTemplate
* Add Iterations and Charts in SymuMessageAndTask example 
* Refactoring BlockerCollection, BlockerResults and TasksResults 
* Add MessageResults

## 0.7.0 [☰](https://github.com/lmorisse/symu/compare/v0.7.0..v0.6.0) by [lmorisse](https://github.com/lmorisse)
* Renaming namespaces 
* Update SymuForm, IterationResult for multiple iterations
* Add new project example: [SymuScenariosAndEvents](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuScenariosAndEvents)
* Add new project test: [SymuScenariosAndEventsTests](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuScenariosAndEventsTests)


## 0.6.0 [☰](https://github.com/lmorisse/symu/compare/v0.6.0..v0.5.0) by [lmorisse](https://github.com/lmorisse)
* Add MurphyIncompleteInformation
* Renaming SimulationEngine (SymuEngine), TimeStep (Schedule)
* Refactoring Murphies
* Add Agent.TimeSpent
* Add new project example: [SymuMurphiesAndBlockers](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuMurphiesAndBlockers)
* Add new project test: [SymuMurphiesAndBlockersTests](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuMurphiesAndBlockersTests)

## 0.5.0 [☰](https://github.com/lmorisse/symu/compare/v0.5.0..v0.4.0) by [lmorisse](https://github.com/lmorisse)
* Add new models : knowledge, Beliefs, Influence 
* Add SymuTask.SetKnowledgesBits
* Refactoring NetworkBeliefs
* Add new project example: [SymuGroupAndInteraction](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuBeliefsAndInfluence)
* Add new project test: [SymuGroupAndInteractionTests](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuBeliefsAndInfluenceTests)

## 0.4.0 [☰](https://github.com/lmorisse/symu/compare/v0.4.0..v0.3.0) by [lmorisse](https://github.com/lmorisse)
* Add new project example: [SymuGroupAndInteraction](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuGroupAndInteraction)
* Add new project test: [SymuGroupAndInteractionTests](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuGroupAndInteractionTests)
* SymuEngine - add InteractionSphereModel
* SymuEngine - update InteractionPatterns

## 0.3.0 [☰](https://github.com/lmorisse/symu/compare/v0.3.0..v0.2.0) by [lmorisse](https://github.com/lmorisse)
* Add new project example: [SymuMessageAndTask](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuMessageAndTask)
* Add new project test: [SymuMessageAndTaskTest](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuMessageAndTaskTests)
* SymuEngine - delete EnvironmentEntity => OrganizationModels

## 0.2.0 [☰](https://github.com/lmorisse/symu/compare/v0.2.0..v0.1.0) by [lmorisse](https://github.com/lmorisse)
* Add new project example: [SymuLearnAndForget](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuLearnAndForget)
* Add new project test: [SymuLearnAndForgetTest](https://github.com/lmorisse/Symu/tree/master/Symu%20examples/SymuLearnAndForgetTests)
