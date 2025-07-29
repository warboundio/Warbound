import type { NextApiRequest, NextApiResponse } from "next";
import { Client } from "pg";
import { ApplicationSettings } from "../../../core/ApplicationSettings";

function npgsqlToUri(connStr: string): string {
  // Example: Host=192.168.86.39;Port=5432;Database=postgres;Username=p;Password=p
  const parts = Object.fromEntries(
    connStr.split(";")
      .filter(Boolean)
      .map(s => s.split("=").map(x => x.trim()))
  );
  const user = encodeURIComponent(parts.Username || "");
  const pass = encodeURIComponent(parts.Password || "");
  const host = parts.Host || "localhost";
  const port = parts.Port || "5432";
  const db = parts.Database || "";
  return `postgres://${user}:${pass}@${host}:${port}/${db}`;
}

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method !== "POST") {
    return res.status(405).json({ error: "Method not allowed" });
  }

  const { itemId, expansionId, collectionType } = req.body;
  if (!itemId || !expansionId || !collectionType || expansionId === -1) {
    return res.status(400).json({ error: "Missing itemId, expansionId, or collectionType" });
  }

  const connectionString = ApplicationSettings.Instance.PostgresConnection;
  if (!connectionString) {
    console.error("PostgresConnection is missing from config.");
    return res.status(500).json({ error: "Database configuration missing" });
  }

  const pgUri = npgsqlToUri(connectionString);

  const client = new Client({
    connectionString: pgUri,
  });

  try {
    await client.connect();
    await client.query(
      `INSERT INTO wow.object_expansion ("Id", "CollectionType", "ExpansionId") VALUES ($1, $2, $3)`,
      [itemId, collectionType, expansionId]
    );
    await client.end();
    res.status(200).json({ success: true });
  } catch (err) {
    console.error("DB insert error:", err);
    res.status(500).json({ error: "Database insert failed" });
  }
}