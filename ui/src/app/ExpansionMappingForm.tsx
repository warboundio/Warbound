"use client";
import React, { useState } from "react";

type CollectionCounts = {
  pets: number;
  toys: number;
  mounts: number;
  transmog: number;
  recipes: number;
};

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

  React.useEffect(() => {
    const firstKey = Object.keys(expansionIdToName)[0] || "";
    setExpansionId(firstKey);
  }, [expansionIdToName]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitting(true);
    setStatus(null);
    const res = await fetch("/api/add-item", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ itemId, expansionId, collectionType }),
    });
    if (res.ok) {
      setStatus("success");
      setItemId(""); // Optionally clear the field
    } else {
      setStatus("error");
    }
    setSubmitting(false);
  }

  return (
    <form onSubmit={handleSubmit} className="border rounded p-4 flex flex-col gap-4 w-full max-w-xs bg-white dark:bg-gray-900">
      <h2 className="font-bold mb-2">Attach Object to Expansion</h2>
      <label className="flex flex-col gap-1">
        <span>Item ID</span>
        <input
          type="text"
          value={itemId}
          onChange={e => setItemId(e.target.value)}
          className="border rounded px-2 py-1"
          required
        />
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
      {status === "error" && <div style={{ color: "red" }}>Failed to insert item.</div>}
    </form>
  );
}
