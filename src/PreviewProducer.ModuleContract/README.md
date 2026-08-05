# Preview producer module contract

This producer-owned package exposes module `preview-producer`, API resource `preview-producer-api`, and preview-only container resource `preview-producer-sidecar`. Consumers call `PreviewProducerModule.ImportModuleAsync(builder)` after selecting an immutable repository revision through Aspire Modular AppHosts configuration.

The package contains the resource graph. The configured Git checkout contains the API source and build context. Consumers must use a contract packed from the same commit whenever that resource graph changes.
