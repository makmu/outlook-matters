# How to contribute

We love pull requests from everyone. By participating in this project, you agree to our [Code of Conduct](#code-of-conduct)

## Making changes
Some things that will increase the chance that your pull request is accepted:

* Write unit tests and run _all_ the tests to assure nothing else was accidentally broken. We strongly value a high test coverage.
* Refrain from writing comments anywhere in the code (except for public APIs). Try to use meaningful names for methods, classes, interfaces, etc. instead.
* Write your commit message in the imperative: "Fix bug" and not "Fixed bug"
or "Fixes bug."  This convention matches up with commit messages generated
by commands like git merge and git revert.
* Try to follow the [Microsoft C# Coding Conventions](https://msdn.microsoft.com/en-us/library/ff926074.aspx).


## Merging and Version Management
We apply an **upstream first** merging policy. That is, the master branch should always contain all new features and bugfixes that are to be deployed in the next release. In addition we use stable release branches such as `v1.0-stable`. The stable branch uses master as a starting point and is created as late as possible to minimize the time we have to apply bugfixes to multiple branches. After a release branch is created, only serious bug fixes are included in the release branch. If possible these bug fixes are first merged into master and then cherry-picked into the release branch.

More information can be found on
* https://about.gitlab.com/2014/09/29/gitlab-flow/
* http://semver.org/


## Code of Conduct
Harassment in code and discussion or violation of physical boundaries is completely unacceptable anywhere in codebases, issue trackers, and any other form of communication and collaboration.
Harassment includes offensive verbal comments related to gender identity, gender expression, sexual orientation, disability, physical appearance, body size, race, religion, sexual images, deliberate intimidation, stalking, sustained disruption, and unwelcome sexual attention.
Violators will be warned by the core team. Repeat violations will result in being blocked or banned by the core team immediately.

Finally, don't forget that it is human to make mistakes! We all do. Let’s work together to help each other, resolve issues, and learn from the mistakes that we will all inevitably make from time to time.
