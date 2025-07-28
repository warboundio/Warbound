export class Base90 {
  public static readonly Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&()*+,-./:<=>?@[\\]^_`{}~";
  private static readonly Base = 90;
  private static readonly EncodedLength = 3;

  private static readonly CharToValue: Record<string, number> = (() => {
    const map: Record<string, number> = {};
    for (let i = 0; i < Base90.Alphabet.length; i++) {
      map[Base90.Alphabet[i]] = i;
    }
    return map;
  })();

  /**
   * Decodes a Base90-encoded string of specified length (1-5) into a number.
   * Throws if input is not the expected length or contains invalid characters.
   */
  public static decode(input: string, length: number): number {
    if (length < 1 || length > 5) {
      throw new Error("Base90 decode length must be between 1 and 5.");
    }
    if (input.length !== length) {
      throw new Error(`Base90 encoded strings must be ${length} characters.`);
    }
    let result = 0;
    for (let i = 0; i < input.length; i++) {
      const c = input[i];
      const value = Base90.CharToValue[c];
      if (value === undefined) {
        throw new Error(`Invalid Base90 character: '${c}'`);
      }
      result = result * Base90.Base + value;
    }
    return result;
  }
}
