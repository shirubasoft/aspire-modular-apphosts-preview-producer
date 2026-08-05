# Preview producer module contract

This producer-owned package exposes module `preview-producer`, API container `preview-producer-api`, and preview-only container `preview-producer-sidecar`. Consumers call `PreviewProducerModule.ImportModuleAsync(builder)` after applying a preview manifest containing allowlisted immutable image digests.

The package contains the complete runtime resource graph and does not require a producer Git checkout. Consumers must use a contract packed from the same commit whenever that resource graph changes.
