import fs from "fs";

export function getCollectionCounts() {
  const filePath = "C:/Applications/Warbound/cached/CollectionsOverall.data";
  let counts = {
    pets: 0,
    toys: 0,
    mounts: 0,
    transmog: 0,
    recipes: 0,
  };

  try {
    const data = fs.readFileSync(filePath, "utf-8");
    const lines = data.split(/\r?\n/);
    for (const line of lines) {
      if (line.startsWith("P|")) {
        counts.pets += line.slice(2).split(";").filter(Boolean).length;
      } else if (line.startsWith("T|")) {
        counts.toys += line.slice(2).split(";").filter(Boolean).length;
      } else if (line.startsWith("M|")) {
        counts.mounts += line.slice(2).split(";").filter(Boolean).length;
      } else if (line.startsWith("A|")) {
        counts.transmog += line.slice(2).split(";").filter(Boolean).length;
      } else if (line.startsWith("R|")) {
        counts.recipes += line.slice(2).split(";").filter(Boolean).length;
      }
    }
  } catch (e) {
    // File might not exist or be readable
  }
  return counts;
}
