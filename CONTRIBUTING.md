#CONTRIBUTING
First off, thank you for considering contributing to SYMU. There are different ways to contribute to this project.
Contributing start by using the framework and give feedbacks.

## As a user
As a user, every feedback such as questions, bugs, improvments are very important to progress toward a user friendly framework.

## As a developper
As a developper, such a framework brings great challenges to which you can contribute
* Unit test a multi agents system (multi thread, asynchronous messages)
* Code design
* Code quality
* Code performance
* App to use the framework

## As an academic
As an academic, you can bring use cases, models that we will implement for you.
This is very important because the value of this framework will be based on the richness of the library of models it uses.

#HOW TO CONTRIBUTE
If you've noticed a bug or have a feature request, [make one][new issue]! It's
generally best if you get confirmation of your bug or approval for your feature
request this way before starting to code.

If you have a general question about SYMU, If you've noticed a bug or have a feature request, you can [make one][new issue]!
Every contibution is done via issues.
General questions will feed the FAQs.

## Fork & create a branch

If this is something you think you can fix, then [fork SYMU] and create
a branch with a descriptive name.

A good branch name would be ISSUENUMBER-FEATURE-YOU-IMPLEMENT

### Get the test suite running
Support your developments in unit testing.
Make sure that the existing tests of the SymuEngineTests and SymuToolsTests projects still pass with your modifications.

### Implement your fix or feature

At this point, you're ready to make your changes! Feel free to ask for help;
everyone is a beginner at first :smile_cat:

### Get the style right

Your patch should follow the same conventions & pass the same code quality
checks as the rest of the project.
You can install packages Microsoft.CodeAnalysis and Microsoft.CodeQuality to check & fix style issues.

### Add a changelog entry

You'll be asked to add a changelog
entry following the existing changelog format.

The changelog format is the following:

* One line per PR describing your fix or enhancement.
* Entries end with a dot, followed by "[#ISSUENUMBERnumber] by [@github-username]".
* YYY. [#ISSUENUMBER] by [@lmorisse]
* Entries are added under the "Unreleased" section at the top of the file, under
  the "Bug Fixes" or "Enhancements" subsection.
* References to github usernames and pull requests use [shortcut reference links].

### Make a Pull Request

At this point, you should switch back to your master branch and make sure it's
up to date with Symu's master branch, then update your feature branch from your local copy of master, and push it!

Finally, go to GitHub and [make a Pull Request][] 

### Keeping your Pull Request updated

If a maintainer asks you to "rebase" your PR, they're saying that a lot of code
has changed, and that you need to update your branch so it's easier to merge.

To learn more about rebasing in Git, there are a lot of [good][git rebasing]
[resources][interactive rebase] but here's the suggested workflow:

### Merging a PR (maintainers only)

A PR can only be merged into master by a maintainer if:

* It is passing CI.
* It has been approved by at least two maintainers. If it was a maintainer who
  opened the PR, only one extra approval is needed.
* It has no requested changes.
* It is up to date with current master.

Any maintainer is allowed to merge a PR if all of these conditions are
met.

### Shipping a release (maintainers only)

Maintainers need to do the following to push out a release:

* Make sure all pull requests are in and that changelog is current
* Switch to the master branch and make sure it's up to date.

[fork Active Admin]: https://help.github.com/articles/fork-a-repo
[make a pull request]: https://help.github.com/articles/creating-a-pull-request
[git rebasing]: http://git-scm.com/book/en/Git-Branching-Rebasing
[interactive rebase]: https://help.github.com/en/github/using-git/about-git-rebase
[shortcut reference links]: https://github.github.com/gfm/#shortcut-reference-link