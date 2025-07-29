import { readFileSync } from "fs";
import path from "path";
import { createDecipheriv } from "crypto";

// DTO interface
export interface SettingsDto {
  PostgresConnection: string;
  DiscordWarboundToken: string;
  BattleNetClientId: string;
  BattleNetSecretId: string;
  GithubToken: string;
  GithubClassicPAT: string;
}

// AES-GCM constants
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

function decrypt(encryptedBase64: string): string {
  const encrypted = Buffer.from(encryptedBase64, "base64");
  if (encrypted.length < NONCE_SIZE + TAG_SIZE) throw new Error("Invalid encrypted config file");

  const nonce = encrypted.subarray(0, NONCE_SIZE);
  const tag = encrypted.subarray(encrypted.length - TAG_SIZE);
  const ciphertext = encrypted.subarray(NONCE_SIZE, encrypted.length - TAG_SIZE);

  const key = getKey();
  const decipher = createDecipheriv("aes-256-gcm", key, nonce);
  decipher.setAuthTag(tag);

  const decrypted = Buffer.concat([decipher.update(ciphertext), decipher.final()]);
  return decrypted.toString("utf8");
}

export class ApplicationSettings {
  private static _instance: ApplicationSettings;
  private _configPath: string;
  private _encryptedConfig: string;

  private constructor(configPath?: string) {
    this._configPath = configPath || path.join(process.cwd(), "core", "config.data");
    this._encryptedConfig = readFileSync(this._configPath, "utf8");
  }

  public static get Instance(): ApplicationSettings {
    if (!this._instance) {
      this._instance = new ApplicationSettings();
    }
    return this._instance;
  }

  private getDecryptedSettings(): SettingsDto {
    const decryptedJson = decrypt(this._encryptedConfig);
    return JSON.parse(decryptedJson) as SettingsDto;
  }

  get PostgresConnection(): string {
    return this.getDecryptedSettings().PostgresConnection;
  }
  get DiscordWarboundToken(): string {
    return this.getDecryptedSettings().DiscordWarboundToken;
  }
  get BattleNetClientId(): string {
    return this.getDecryptedSettings().BattleNetClientId;
  }
  get BattleNetSecretId(): string {
    return this.getDecryptedSettings().BattleNetSecretId;
  }
  get GithubToken(): string {
    return this.getDecryptedSettings().GithubToken;
  }
  get GithubClassicPAT(): string {
    return this.getDecryptedSettings().GithubClassicPAT;
  }

  // Optionally: expose all settings at once
  public getAll(): SettingsDto {
    return this.getDecryptedSettings();
  }
}
