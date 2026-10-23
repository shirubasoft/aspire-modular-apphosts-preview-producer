# Aspire Modular AppHosts preview producer

> [!IMPORTANT]
> This repository is archived. It was a fixture for the cross-repository module preview sample of
> `aspire-modular-apphosts`, whose packages continue as the
> [Multirepo extension](https://github.com/shirubasoft/aspire-extensions/tree/main/extensions/Shirubasoft.Aspire.Extensions.Multirepo)
> in shirubasoft/aspire-extensions. The module-preview feature that this fixture exercised was
> removed from `aspire-modular-apphosts` before the migration and is not available in Multirepo.

This public fixture is Repo C in the cross-repository preview sample for [Shirubasoft.Aspire.ModularAppHosts](https://github.com/Shirubasoft/aspire-modular-apphosts). It owns a minimal HTTP API and the package that describes the API as an Aspire module. The producer builds the API image, while the consumer selects and runs its immutable registry digest without checking out producer source.

## Projects

- `src/PreviewProducer.Api` serves `preview-from-feature-branch` from `GET /` and exposes `GET /health`.
- `src/PreviewProducer.ModuleContract` produces `Shirubasoft.Aspire.ModularAppHosts.PreviewProducer.Contract`. Local builds default to the released `Shirubasoft.Aspire.ModularAppHosts` 1.1.0 package; preview CI compiles against the exact trusted runtime commit and package version also used by the consumer.

The contract declares module `preview-producer` and the container resources
`preview-producer-api` and `preview-producer-sidecar`.
[`module-preview.producer.json`](module-preview.producer.json) describes their expected repositories
and contract package ID, while
[`module-preview.image-only.producer.json`](module-preview.image-only.producer.json) describes the
same images without requesting a contract. The consumer policy validates immutable image digests
before the AppHost imports the module.

## Publish preview artifacts

The `Publish preview artifacts` workflow does all producer-owned build and publication work once:

1. It packs the pinned `aspire-modular-apphosts` commit as
   `1.2.0-preview.f54233a`, then restores the contract against that exact runtime ABI.
2. It packs the contract with the unique CI-computed version
   `2.0.0-preview.<run>.<attempt>`, where `<run>` is `GITHUB_RUN_NUMBER` and `<attempt>` is
   `GITHUB_RUN_ATTEMPT`.
3. It publishes that exact package to GitHub Packages at the credential-free source
   `https://nuget.pkg.github.com/shirubasoft/index.json`.
4. It builds the API from this repository and pushes the commit-tagged image to GHCR.
5. It captures the registry-reported API digest and combines it with the pinned nginx digest.
6. It generates and uploads both preview requests plus package and image provenance.

All actions are pinned to full commits, and the workflow has only `contents: read` and
`packages: write` permissions. The package source contains no embedded credentials; publication
uses the workflow token, and the consumer independently owns authentication for restoring it.

The normal descriptor intentionally declares only the package ID:

```json
"contract": {
  "packageId": "Shirubasoft.Aspire.ModularAppHosts.PreviewProducer.Contract"
}
```

After publishing the package, CI passes its computed version to the tool instead of modifying the
committed descriptor:

```bash
dotnet modular-apphosts preview produce \
  --descriptor module-preview.producer.json \
  --contract-version "$CONTRACT_VERSION" \
  --image "preview-producer-api=${IMAGE_REPOSITORY}@${IMAGE_DIGEST}" \
  --image "preview-producer-sidecar=${NGINX_REPOSITORY}@${NGINX_DIGEST}" \
  --output "$RUNNER_TEMP/module-preview.published-contract.json"
```

The image-only descriptor omits `contract` entirely, so its `preview produce` invocation does not
accept or need `--contract-version`. The workflow uploads both generated manifests in the
`preview-producer-<commit>` artifact:

- `module-preview.published-contract.json` requests the exact published contract version and both
  immutable images.
- `module-preview.image-only.json` has an empty contract collection and requests the same immutable
  images.

The descriptors are declarative metadata. The workflow does not execute commands supplied by them;
Docker receives the checked-in API Dockerfile and a fixed build context. The package source is also
absent from producer descriptors and requests. Repo D trusts
`https://nuget.pkg.github.com/shirubasoft/index.json` through its reviewed consumer policy, restores
the exact requested package, and verifies its identity and hash without checking out or packing
producer source.

## Build and pack

Install the .NET 10 SDK, then run:

```bash
dotnet restore PreviewProducer.slnx
dotnet build PreviewProducer.slnx --configuration Release --no-restore
dotnet pack src/PreviewProducer.ModuleContract/PreviewProducer.ModuleContract.csproj \
  --configuration Release --no-build --output artifacts
```

This feature branch changes both the API response and the module resource kind relative to `main`.
Its contract must therefore be packed from this branch rather than reusing the baseline package.
Consumers restore that exact contract and use the workflow's immutable API digest, so they do not
build the image or clone this repository during the E2E run. The image-only request demonstrates
the alternative path for a consumer policy that makes the contract optional.

## Trigger the trusted consumer workflow

After downloading either generated manifest from the producer workflow artifact, authenticate the
GitHub CLI and trigger Repo D's workflow definition from its currently reviewed fixture branch:

```bash
gh auth login
dotnet modular-apphosts preview trigger \
  --manifest artifacts/module-preview.published-contract.json \
  --repo shirubasoft/aspire-modular-apphosts-preview-consumer \
  --workflow module-preview-e2e.yml \
  --ref codex/tool-owned-preview-materialization \
  --wait
```

The trigger immediately prints the concrete run identity:

```text
workflow_run_id=123456789
workflow_run_url=https://github.com/shirubasoft/aspire-modular-apphosts-preview-consumer/actions/runs/123456789
```

The bare `--wait` flag then watches that run and returns its final status. The producer feature branch
is represented by the manifest's immutable commit, package version, and image digests;
`--ref codex/tool-owned-preview-materialization` selects the consumer-owned workflow definition and
must not be changed to the producer branch. Switch the reviewed ref to `main` after the consumer PR
merges.

## License

This sample is licensed under the MIT License. See `LICENSE`.
