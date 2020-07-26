import { useMemo, useCallback } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { v4 as uuid } from "uuid";

declare global {
  interface Window {
    apiBaseUrl: string;
  }

  namespace NodeJS {
    interface Global {
      apiBaseUrl: string;
    }
  }
}

function getBaseUrl() {
  return (window || global).apiBaseUrl;
}

function abortPromise(signal: AbortSignal) {
  return new Promise((_, reject) =>
    signal.addEventListener("abort", () => reject())
  );
}

export type FetchHelper = (url: string, init?: RequestInit) => Promise<Response>;

async function getConnection(fetchHelper: FetchHelper, signal: AbortSignal) {
  const response = await fetchHelper(`/negotiate`, {
    method: "POST",
    signal,
  });
  if (response.status !== 200) throw new Error("Could not negotiate SignalR");
  const info = await response.json();

  // make compatible with old and new SignalRConnectionInfo
  info.accessToken = info.accessToken || info.accessKey;
  info.url = info.url || info.endpoint;

  const hub = new HubConnectionBuilder()
    .withUrl(info.url, {
      accessTokenFactory: () => info.accessToken,
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

  const promises = [abortPromise(signal).catch(() => hub.stop()), hub.start()];
  await new Promise((...args) => promises.forEach((p) => p.then(...args))).then(
    () => {},
    () => {}
  );
  return hub;
}

export function useNewGameConnection() {
  const gamerId = useMemo(() => uuid(), []);
  const cancellation = useMemo(() => new AbortController(), []);
  const fetchHelper = useCallback(function (url: string, { headers, ...init }: RequestInit = {}) {
    return fetch(getBaseUrl() + url, { headers: { "X-Gamer-Id": gamerId, ...headers }, ...init });
  }, [ gamerId ]);

  const connectionPromise = useMemo(() => getConnection(fetchHelper, cancellation.signal), [
    cancellation.signal,
    fetchHelper,
  ]);

  return { connectionPromise, fetchHelper };
}
