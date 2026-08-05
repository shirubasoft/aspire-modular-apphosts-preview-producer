# Preview producer module contract

This producer-owned package exposes module `preview-producer`, API container
`preview-producer-api`, and preview-only container `preview-producer-sidecar`. Consumers call
`PreviewProducerModule.ImportModuleAsync(builder)` after applying a preview resolution containing
allowlisted immutable image digests.

The producer workflow packs and publishes a unique contract version for every attempt using
`2.0.0-preview.<run>.<attempt>`. It publishes to the credential-free GitHub Packages source
`https://nuget.pkg.github.com/shirubasoft/index.json`; credentials are supplied separately by the
producer and consumer workflows.

The committed `module-preview.producer.json` descriptor declares this package ID without a version.
Only after publication does CI pass the computed exact version to
`preview produce --contract-version`, producing `module-preview.published-contract.json`. It also
uses the contract-free `module-preview.image-only.producer.json` descriptor to produce
`module-preview.image-only.json`. Both manifests contain the already-built immutable API and sidecar
image digests and are uploaded as workflow artifacts.

The consumer owns the trusted package source in its policy. For the published-contract manifest, it
restores the exact package ID and version, verifies the `.nuspec` identity and SHA-256, and records
the package source and local path in its resolution. It does not check out this repository or pack
producer source. An image-only manifest does not materialize a preview contract package; the
consumer continues using its own exact baseline contract.

The package contains the complete runtime resource graph and does not require a producer Git
checkout. Consumers must use a contract packed from the same commit whenever that graph changes.
When triggering the trusted consumer workflow locally, `preview trigger --wait` prints
`workflow_run_id` and `workflow_run_url`, then watches that exact run through completion.
