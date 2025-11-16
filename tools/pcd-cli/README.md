# Motion-First GraphQL CLI

This lightweight CLI turns the GraphQL mutation/query pattern described in the Motion-First Predictive Contract Design guide into a contract-driven workflow.

## Usage

```
dotnet run --project tools/pcd-cli/pcd-cli.csproj -- \
  --contract contracts/graphql.operation.yaml \
  --configs configs/graphql \
  --output src/Api/GraphQL/Generated
```

- `--contract` – shared template describing runtime defaults, entry service, and response wrapper rules.
- `--configs` – small per-operation YAML files.
- `--output` – folder receiving the generated partial `Mutation` members.

Each run rewrites the generated files so every mutation/query stays consistent.
