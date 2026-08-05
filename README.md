# Aspire Modular AppHosts preview producer

This public fixture is Repo C in the cross-repository preview sample for [Shirubasoft.Aspire.ModularAppHosts](https://github.com/Shirubasoft/aspire-modular-apphosts). It owns a minimal HTTP API and the package that describes the API as an Aspire module. The producer builds the API image, while the consumer selects and runs its immutable registry digest without checking out producer source.

## Projects

- `src/PreviewProducer.Api` serves `preview-from-feature-branch` from `GET /` and exposes `GET /health`.
- `src/PreviewProducer.ModuleContract` produces `Shirubasoft.Aspire.ModularAppHosts.PreviewProducer.Contract` and references the released `Shirubasoft.Aspire.ModularAppHosts` 1.1.0 package.

The contract declares module `preview-producer` and the container resources `preview-producer-api` and `preview-producer-sidecar`. [`module-preview.producer.json`](module-preview.producer.json) describes their expected repositories and the matching contract package. The consumer policy supplies and validates immutable image digests before the AppHost imports the module.

## Publish the preview image

The `Publish preview API image` workflow builds the API from this repository, pushes the commit-tagged image to GHCR, and uploads a provenance record containing the exact `repository@sha256` identity. All actions are pinned to full commits, and the workflow has only `contents: read` and `packages: write` permissions.

The descriptor is declarative metadata. The workflow does not execute commands supplied by it; Docker receives the checked-in API Dockerfile and a fixed build context.

## Build and pack

Install the .NET 10 SDK, then run:

```bash
dotnet restore PreviewProducer.slnx
dotnet build PreviewProducer.slnx --configuration Release --no-restore
dotnet pack src/PreviewProducer.ModuleContract/PreviewProducer.ModuleContract.csproj \
  --configuration Release --no-build --output artifacts
```

This feature branch changes both the API response and the module resource kind relative to `main`. Its contract must therefore be packed from this branch rather than reusing the baseline package. Consumers restore that exact contract and use the workflow's immutable API digest, so they do not build the image or clone this repository during the E2E run.

## License

This sample is licensed under the MIT License. See `LICENSE`.
