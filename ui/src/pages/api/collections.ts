import type { NextApiRequest, NextApiResponse } from "next";
import { getCollectionCountsByExpansion, getExpansionIdToNameMap } from "../../utils/collections";

export default function handler(req: NextApiRequest, res: NextApiResponse) {
  try {
    const countsByExpansion = getCollectionCountsByExpansion();
    const expansionIdToName = getExpansionIdToNameMap();
    res.status(200).json({ countsByExpansion, expansionIdToName });
  } catch (err) {
    console.error("API error:", err);
    res.status(500).json({ error: "Failed to load data" });
  }
}
