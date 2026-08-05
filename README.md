# Aspire Modular AppHosts preview producer

This public fixture is Repo C in the cross-repository preview sample for [Shirubasoft.Aspire.ModularAppHosts](https://github.com/Shirubasoft/aspire-modular-apphosts). It owns a minimal HTTP API and the package that describes the API as an Aspire module. A separate consumer repository packs this contract at an exact Git commit and imports the API source from the same immutable commit.

## Projects

- `src/PreviewProducer.Api` serves `preview-from-feature-branch` from `GET /` and exposes `GET /health`.
- `src/PreviewProducer.ModuleContract` produces `Shirubasoft.Aspire.ModularAppHosts.PreviewProducer.Contract` and references the released `Shirubasoft.Aspire.ModularAppHosts` 1.1.0 package.

The contract declares module `preview-producer`, project resource `preview-producer-api`, and preview-only container resource `preview-producer-sidecar`. Its repository-aware factory constructs the API project path from `context.RepositoryPath`, so an importing AppHost can clone and pin this repository independently from the contract assembly.

## Build and pack

Install the .NET 10 SDK, then run:

```bash
dotnet restore PreviewProducer.slnx
dotnet build PreviewProducer.slnx --configuration Release --no-restore
dotnet pack src/PreviewProducer.ModuleContract/PreviewProducer.ModuleContract.csproj \
  --configuration Release --no-build --output artifacts
```

This feature branch changes both the API response and the module resource graph relative to `main`. Its contract must therefore be packed from this branch rather than reusing the baseline package.

## License

This sample is licensed under the MIT License. See `LICENSE`.
