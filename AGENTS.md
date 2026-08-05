# Repository instructions

Never synchronously block on async work, including `.GetAwaiter().GetResult()`, `.Result`, or task `.Wait()`. Propagate async to the caller instead.

Backward compatibility is not a constraint. Make the best breaking change directly and mark it with Conventional Commits syntax (for example, `feat!:` or `fix!:`) plus a `BREAKING CHANGE:` footer describing the impact and migration path.

