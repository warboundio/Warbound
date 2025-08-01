"use client";
import Image from "next/image";
import { useState, useEffect } from "react";
import ExpansionMappingForm from "./ExpansionMappingForm";

type CollectionCounts = {
  pets: number;
  toys: number;
  mounts: number;
  transmog: number;
  recipes: number;
};

export default function Home() {
  // --- State for fetched data ---
  const [countsByExpansion, setCountsByExpansion] = useState<Record<string, CollectionCounts>>({});
  const [expansionIdToName, setExpansionIdToName] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(true);

  // --- Fetch data from API on mount ---
  useEffect(() => {
    fetch("/api/collections")
      .then(res => res.json())
      .then(data => {
        setCountsByExpansion(data.countsByExpansion);
        setExpansionIdToName(data.expansionIdToName);
        setLoading(false);
      });
  }, []);

  return (
    <div className="font-sans grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20">
      <main className="flex flex-col gap-[32px] row-start-2 items-center sm:items-start">
        <Image
          className="dark:invert"
          src="/next.svg"
          alt="Next.js logo"
          width={180}
          height={38}
          priority
        />
        <ol className="font-mono list-inside list-decimal text-sm/6 text-center sm:text-left">
          <li className="mb-2 tracking-[-.01em]">
            Get started by editing{" "}
            <code className="bg-black/[.05] dark:bg-white/[.06] font-mono font-semibold px-1 py-0.5 rounded">
              src/app/page.tsx
            </code>
            .
          </li>
          <li className="tracking-[-.01em]">
            Save and see your changes instantly.
          </li>
        </ol>

        <div className="mb-6">
          <h2 className="font-bold mb-2">Collection Counts by Expansion</h2>
          {loading ? (
            <div className="text-sm">Loading...</div>
          ) : Object.keys(countsByExpansion).length === 0 ? (
            <div className="text-sm">No data found.</div>
          ) : (
            <div className="flex flex-col gap-4">
              {Object.entries(countsByExpansion).map(([expansionId, counts]) => (
                <div key={expansionId} className="border rounded p-3">
                  <div className="font-semibold mb-1">
                    Expansion: {expansionIdToName[expansionId] || expansionId}    
                  </div>
                  <ul className="list-disc pl-5 text-sm">
                    <li>Pets: {counts.pets}</li>
                    <li>Toys: {counts.toys}</li>
                    <li>Mounts: {counts.mounts}</li>
                    <li>Transmog: {counts.transmog}</li>
                    <li>Recipes: {counts.recipes}</li>
                  </ul>
                </div>
              ))}
            </div>
          )}
        </div>

        <ExpansionMappingForm
          expansionIdToName={expansionIdToName}
          loading={loading}
        />

        <div className="flex gap-4 items-center flex-col sm:flex-row">
          <a
            className="rounded-full border border-solid border-transparent transition-colors flex items-center justify-center bg-foreground text-background gap-2 hover:bg-[#383838] dark:hover:bg-[#ccc] font-medium text-sm sm:text-base h-10 sm:h-12 px-4 sm:px-5 sm:w-auto"
            href="https://vercel.com/new?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
            target="_blank"
            rel="noopener noreferrer"
          >
            <Image
              className="dark:invert"
              src="/vercel.svg"
              alt="Vercel logomark"
              width={20}
              height={20}
            />
            Deploy now
          </a>
          <a
            className="rounded-full border border-solid border-black/[.08] dark:border-white/[.145] transition-colors flex items-center justify-center hover:bg-[#f2f2f2] dark:hover:bg-[#1a1a1a] hover:border-transparent font-medium text-sm sm:text-base h-10 sm:h-12 px-4 sm:px-5 w-full sm:w-auto md:w-[158px]"
            href="https://nextjs.org/docs?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
            target="_blank"
            rel="noopener noreferrer"
          >
            Read our docs
          </a>
        </div>
      </main>
      <footer className="row-start-3 flex gap-[24px] flex-wrap items-center justify-center">
        <a
          className="flex items-center gap-2 hover:underline hover:underline-offset-4"
          href="https://nextjs.org/learn?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
          target="_blank"
          rel="noopener noreferrer"
        >
          <Image
            aria-hidden
            src="/file.svg"
            alt="File icon"
            width={16}
            height={16}
          />
          Learn
        </a>
        <a
          className="flex items-center gap-2 hover:underline hover:underline-offset-4"
          href="https://vercel.com/templates?framework=next.js&utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
          target="_blank"
          rel="noopener noreferrer"
        >
          <Image
            aria-hidden
            src="/window.svg"
            alt="Window icon"
            width={16}
            height={16}
          />
          Examples
        </a>
        <a
          className="flex items-center gap-2 hover:underline hover:underline-offset-4"
          href="https://nextjs.org?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
          target="_blank"
          rel="noopener noreferrer"
        >
          <Image
            aria-hidden
            src="/globe.svg"
            alt="Globe icon"
            width={16}
            height={16}
          />
          Go to nextjs.org →
        </a>
      </footer>
    </div>
  );
}