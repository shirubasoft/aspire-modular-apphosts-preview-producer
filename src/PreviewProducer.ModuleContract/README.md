# Preview producer module contract

This producer-owned package exposes module `preview-producer`. Consumers call `PreviewProducerModule.ImportModuleAsync(builder)` after selecting an immutable repository revision through Aspire Modular AppHosts configuration.

The package contains the resource graph. The configured Git checkout contains the API source and build context. Consumers must use a contract packed from the same commit whenever that resource graph changes.

