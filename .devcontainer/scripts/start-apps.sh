#!/usr/bin/env bash
set -euo pipefail

readonly workspace=/workspaces/RoyalVilla
readonly runtime_dir=/tmp/royalvilla
readonly api_log="${runtime_dir}/RoyalVilla API.log"
readonly web_log="${runtime_dir}/RoyalVilla Web.log"

mkdir -p "${runtime_dir}"
cd "${workspace}"

wait_for_url() {
  local name="$1"
  local url="$2"
  local process_id="$3"
  local log_file="$4"

  for _ in {1..90}; do
    if curl --fail --silent "${url}" >/dev/null 2>&1; then
      echo "${name} is ready at ${url}"
      return
    fi

    if ! kill -0 "${process_id}" 2>/dev/null; then
      break
    fi

    sleep 1
  done

  echo "${name} failed to start. Recent log output:" >&2
  tail -n 100 "${log_file}" >&2
  return 1
}

stop_services() {
  kill "${api_pid:-}" "${web_pid:-}" 2>/dev/null || true
}

trap stop_services EXIT INT TERM

dotnet tool restore
dotnet restore RoyalVilla.slnx
dotnet build RoyalVilla.slnx --no-restore

env ASPNETCORE_URLS=http://0.0.0.0:5000 \
  dotnet run --project RoyalVilla_API/RoyalVilla_API.csproj --no-build --no-launch-profile \
  >"${api_log}" 2>&1 &
api_pid=$!

wait_for_url \
  "RoyalVilla API" \
  "http://localhost:5000/api/v1/villa/1" \
  "${api_pid}" \
  "${api_log}"

env ASPNETCORE_URLS=http://0.0.0.0:5079 \
  dotnet run --project RoyalVillaWeb/RoyalVillaWeb.csproj --no-build --no-launch-profile \
  >"${web_log}" 2>&1 &
web_pid=$!

wait_for_url \
  "RoyalVilla Web" \
  "http://localhost:5079" \
  "${web_pid}" \
  "${web_log}"

echo "RoyalVilla is running. Logs are available in ${runtime_dir}."
wait "${web_pid}"