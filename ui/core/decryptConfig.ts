import { readFileSync } from "fs";
import { createDecipheriv, randomBytes } from "crypto";
import path from "path";

const ENV_VAR = "WARBOUND";
const NONCE_SIZE = 12;
const TAG_SIZE = 16;
const KEY_SIZE = 32;

function getKey(): Buffer {
  const keyBase64 = process.env[ENV_VAR];
  if (!keyBase64) throw new Error(`Missing ${ENV_VAR} env variable`);
  const key = Buffer.from(keyBase64, "base64");
  if (key.length !== KEY_SIZE) throw new Error(`Key must be 32 bytes`);
  return key;
}

export function decryptConfigFile(configPath: string): any {
  const encrypted = Buffer.from(readFileSync(configPath, "utf8"), "base64");
  if (encrypted.length < NONCE_SIZE + TAG_SIZE) throw new Error("Invalid encrypted config file");

  const nonce = encrypted.subarray(0, NONCE_SIZE);
  const tag = encrypted.subarray(encrypted.length - TAG_SIZE);
  const ciphertext = encrypted.subarray(NONCE_SIZE, encrypted.length - TAG_SIZE);

  const key = getKey();
  const decipher = createDecipheriv("aes-256-gcm", key, nonce);
  decipher.setAuthTag(tag);

  const decrypted = Buffer.concat([decipher.update(ciphertext), decipher.final()]);
  return JSON.parse(decrypted.toString("utf8"));
}

// Example usage:
// const config = decryptConfigFile(path.join(process.cwd(), "config.data"));
// console.log(config.PostgresConnection);
