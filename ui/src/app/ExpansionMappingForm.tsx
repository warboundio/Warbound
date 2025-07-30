"use client";
import React, { useState } from "react";

const collectionTypeOptions = [
  { value: "I", label: "Item" }, 
  { value: "P", label: "Pet" },
  { value: "T", label: "Toy" },
  { value: "M", label: "Mount" },
  { value: "A", label: "Appearance" },
  { value: "R", label: "Recipe" },
];

type Props = {
  expansionIdToName: Record<string, string>;
  loading: boolean;
};

export default function ExpansionMappingForm({ expansionIdToName, loading }: Props) {
  const [itemId, setItemId] = useState("");
  const [expansionId, setExpansionId] = useState(Object.keys(expansionIdToName)[0] || "");
  const [collectionType, setCollectionType] = useState("I");
  const [submitting, setSubmitting] = useState(false);
  const [status, setStatus] = useState<null | "success" | "error">(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  React.useEffect(() => {
    const firstKey = Object.keys(expansionIdToName)[0] || "";
    setExpansionId(firstKey);
  }, [expansionIdToName]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitting(true);
    setStatus(null);
    setErrorMessage(null);

    const parsedIds = itemId.split(";").map(id => id.trim());
    const itemIds = parsedIds.filter(id => id.length > 0);

    if (
      itemIds.length === 0 ||
      !expansionId ||
      !collectionType
    ) {
      setStatus("error");
      setErrorMessage("Missing itemId(s), expansionId, or collectionType.");
      setSubmitting(false);
      return;
    }

    // Send each itemId individually
    let successCount = 0;
    let failCount = 0;
    const failDetails: string[] = []; // use const instead of let

    for (const id of itemIds) {
      const payload = { itemId: id, expansionId, collectionType };
      try {
        const res = await fetch("/api/add-item", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(payload),
        });
        if (res.ok) {
          successCount++;
        } else {
          failCount++;
          try {
            const data = await res.json();
            failDetails.push(`ID ${id}: ${data?.error || JSON.stringify(data)}`);
          } catch {
            const text = await res.text();
            failDetails.push(`ID ${id}: ${text}`);
          }
        }
      } catch (err) {
        failCount++;
        failDetails.push(`ID ${id}: ${err}`);
      }
    }

    if (failCount === 0) {
      setStatus("success");
      setItemId("");
    } else {
      setStatus("error");
      setErrorMessage(`Failed to insert ${failCount} item(s). See details below.`);
    }
    setSubmitting(false);
  }

  return (
    <form onSubmit={handleSubmit} className="border rounded p-4 flex flex-col gap-4 w-full max-w-xs bg-white dark:bg-gray-900">
      <h2 className="font-bold mb-2">Attach Object to Expansion</h2>
      <label className="flex flex-col gap-1">
        <span>
          Item ID
          <span className="text-xs text-gray-500 ml-2">(semicolon-separated for multiple)</span>
        </span>
        <input
          type="text"
          value={itemId}
          onChange={e => setItemId(e.target.value)}
          className="border rounded px-2 py-1"
          required
        />
        <span className="text-xs text-gray-600 mt-1">
          Parsed IDs: {itemId.split(";").map(id => id.trim()).filter(id => id.length > 0).length}
          {itemId.split(";").map(id => id.trim()).some(id => id.length === 0) && itemId.split(";").map(id => id.trim()).length > 1 && (
            <span className="text-red-500 ml-2">Warning: Some empty IDs detected (check for trailing or double semicolons)</span>
          )}
        </span>
      </label>
      <label className="flex flex-col gap-1">
        <span>Collection Type</span>
        <select
          value={collectionType}
          onChange={e => setCollectionType(e.target.value)}
          className="border rounded px-2 py-1"
          required
        >
          {collectionTypeOptions.map(opt => (
            <option key={opt.value} value={opt.value}>{opt.label}</option>
          ))}
        </select>
      </label>
      <label className="flex flex-col gap-1">
        <span>Expansion</span>
        <select
          value={expansionId}
          onChange={e => setExpansionId(e.target.value)}
          className="border rounded px-2 py-1"
          required
          disabled={loading}
        >
          {Object.entries(expansionIdToName).map(([id, name]) => (
            <option key={id} value={id}>{name}</option>
          ))}
        </select>
      </label>
      <button
        type="submit"
        className="bg-blue-600 text-white rounded px-4 py-2 hover:bg-blue-700"
        disabled={loading || submitting}
      >
        {submitting ? "Submitting..." : "Submit"}
      </button>
      {submitting && <div>Loading...</div>}
      {status === "success" && <div style={{ color: "green" }}>Item added to the database!</div>}
      {status === "error" && (
        <div style={{ color: "red" }}>
          Failed to insert item.
          {errorMessage && (
            <pre className="mt-2 whitespace-pre-wrap text-xs">{errorMessage}</pre>
          )}
        </div>
      )}
    </form>
  );
}