#!/usr/bin/env bash
set -euo pipefail

readonly runtime_dir=/tmp/royalvilla

for _ in {1..120}; do
  if curl --fail --silent http://localhost:5000/api/v1/villa/1 >/dev/null 2>&1 \
    && curl --fail --silent http://localhost:5079 >/dev/null 2>&1; then
    echo "RoyalVilla API and Web are ready."
    exit 0
  fi

  sleep 1
done

echo "RoyalVilla did not become ready. Recent logs:" >&2
tail -n 100 "${runtime_dir}/RoyalVilla API.log" 2>/dev/null >&2 || true
tail -n 100 "${runtime_dir}/RoyalVilla Web.log" 2>/dev/null >&2 || true
exit 1