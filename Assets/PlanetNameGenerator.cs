using UnityEngine;
using System.Collections.Generic;
using System.Text;

class PlanetNameGenerator {
	static readonly string[] syllables = {
		"axo", "bro", "cle", "da", "ele", "fi", "gnu", "hu", "imi", "jra", "kla", "loo", "mu", "nde", "oxy", "pre", "qu", "rna", "ste", "tlo", "unu", "vere", "who", "xy", "y", "z"
	};

	static readonly string[] suffixes = {
		"Alpha",
		"Beta",
		"Gamma",
		"Delta",
		"Prime",
		"Major",
		"Minor",
		"Z"
	};

	public static string GeneratePlanetName() {
		StringBuilder sb = new StringBuilder();

		int syllableCount = Random.Range(1, 8);
		for (int i = 0; i < syllableCount; i++) {
			sb.Append( syllables[Random.Range(0, syllables.Length)] );
			if (i != syllableCount - 1 && Random.Range(0f, 1f) < 0.2f) {
				sb.Append(' ');
			}
		}

		if (Random.Range(0f, 1f) < 0.4f) {
			// Add a suffix!
			sb.Append(' ');
			sb.Append(suffixes[Random.Range(0, suffixes.Length)]);
		}

		string result = sb.ToString();

		// Uppercase any words after a space
		char[] ca = result.ToCharArray();
		ca[0] = char.ToUpper(ca[0]);
		for (int i = 1; i < ca.Length; i++) {
			if (ca[i - 1] == ' ') {
				ca[i] = char.ToUpper(ca[i]);
			}
		}

		return new string(ca);
	}
}
