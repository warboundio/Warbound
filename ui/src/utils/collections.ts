import AdmZip from "adm-zip";
import { Base90 } from "./Base90";
import fs from "fs";
import path from "path";

export function getCollectionCountsByExpansion() {
  const zipFilePath = "C:/Applications/Warbound/cached/CollectionsOverall.zip";
  const dataFileName = "CollectionsOverall.data";
  const expansionIdToName = getExpansionIdToNameMap();
  const result: Record<string, { pets: number; toys: number; mounts: number; transmog: number; recipes: number }> = {};

  try {
    const zip = new AdmZip(zipFilePath);
    const zipEntry = zip.getEntry(dataFileName);
    if (!zipEntry) {
      return result;
    }
    const data = zipEntry.getData().toString("utf-8");
    const lines = data.split(/\r?\n/);
    for (const line of lines) {
      let type = "";
      if (line.startsWith("P|")) type = "pets";
      else if (line.startsWith("T|")) type = "toys";
      else if (line.startsWith("M|")) type = "mounts";
      else if (line.startsWith("A|")) type = "transmog";
      else if (line.startsWith("R|")) type = "recipes";
      else continue;

      // Format: X|expansionId|<encoded>
      const parts = line.split("|");
      if (parts.length < 3) continue;
      const expansionId = parts[1];
      const expansionName = expansionIdToName[expansionId] || expansionId;
      const encodedStr = parts.slice(2).join("|"); // in case encoded contains '|'
      const encoded: string[] = [];
      for (let i = 0; i < encodedStr.length; i += 3) {
        const chunk = encodedStr.slice(i, i + 3);
        if (chunk.length === 3) encoded.push(chunk);
      }
      const decoded = encoded.map((val) => Base90.decode(val, 3));
      if (!result[expansionName]) {
        result[expansionName] = { pets: 0, toys: 0, mounts: 0, transmog: 0, recipes: 0 };
      }
      result[expansionName][type as keyof typeof result[string]] += decoded.length;
    }
  } catch {
    // File might not exist or be readable
  }
  return result;
}

/**
 * Reads ExpansionData.data and returns a mapping from expansion ID to name.
 */
export function getExpansionIdToNameMap(): Record<string, string> {
  const expansionFilePath = "C:/Applications/Warbound/cached/ExpansionData.data";
  const map: Record<string, string> = {};
  try {
    const data = fs.readFileSync(expansionFilePath, "utf-8");
    const lines = data.split(/\r?\n/);
    for (const line of lines) {
      if (!line.trim()) continue;
      const [id, name] = line.split("|");
      if (id && name) {
        map[id] = name;
      }
    }
  } catch {
    // File might not exist or be readable
  }
  return map;
}
