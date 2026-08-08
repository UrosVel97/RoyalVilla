# GitHub Codespaces

Create a Codespace from the branch you want to run. The container setup automatically:

1. Starts PostgreSQL in a private sibling container.
2. Restores tools and NuGet packages.
3. Builds the complete solution.
4. Applies EF Core migrations when the API starts.
5. Runs the API on port `5000` and the MVC application on port `5079` as persistent container processes.

Port `5079` is configured for public forwarding so the MVC application can be shared. If a GitHub account or organization policy creates it as private, open the Codespace **Ports** panel, right-click port `5079`, and set **Port Visibility** to **Public**. Port `5000` remains private and provides the Scalar API reference at `/scalar` for the Codespace owner.

Application logs are written inside the app container:

```bash
tail -f /tmp/royalvilla/RoyalVilla\ Web.log
tail -f /tmp/royalvilla/RoyalVilla\ API.log
```

The PostgreSQL credentials in `.devcontainer/compose.yaml` are isolated demo credentials for the disposable Codespace environment. Do not reuse them for deployment.

See the root [README](../README.md#run-in-github-codespaces) for the complete Codespaces walkthrough.
