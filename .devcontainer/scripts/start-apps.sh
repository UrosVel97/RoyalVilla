#!/usr/bin/env bash
set -euo pipefail

readonly workspace=/workspaces/RoyalVilla
readonly runtime_dir=/tmp/royalvilla

mkdir -p "${runtime_dir}"
cd "${workspace}"

is_ready() {
  curl --fail --silent --show-error "$1" >/dev/null 2>&1
}

start_service() {
  local name="$1"
  local url="$2"
  local port="$3"
  local project="$4"
  local log_file="${runtime_dir}/${name}.log"
  local pid_file="${runtime_dir}/${name}.pid"

  if is_ready "${url}"; then
    echo "${name} is already running."
    return
  fi

  if [[ -f "${pid_file}" ]]; then
    local previous_pid
    previous_pid=$(cat "${pid_file}")
    if kill -0 "${previous_pid}" 2>/dev/null; then
      kill "${previous_pid}"
    fi
  fi

  nohup env ASPNETCORE_URLS="http://0.0.0.0:${port}" \
    dotnet run --project "${project}" --no-launch-profile \
    >"${log_file}" 2>&1 </dev/null &
  echo $! >"${pid_file}"

  for _ in {1..60}; do
    if is_ready "${url}"; then
      echo "${name} is ready at ${url}"
      return
    fi
    sleep 1
  done

  echo "${name} failed to start. Recent log output:"
  tail -n 50 "${log_file}"
  return 1
}

start_service \
  "RoyalVilla API" \
  "http://localhost:5000/api/v1/villa/1" \
  "5000" \
  "RoyalVilla_API/RoyalVilla_API.csproj"

start_service \
  "RoyalVilla Web" \
  "http://localhost:5079" \
  "5079" \
  "RoyalVillaWeb/RoyalVillaWeb.csproj"

echo "Logs are available in ${runtime_dir}."