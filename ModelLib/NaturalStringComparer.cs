using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelLib
{
	public static class NaturalStringComparer
	{
		private static Regex regex = new Regex(@"(\d+|\D+)+");
		private static Regex numericRegex = new Regex(@"\d+");

		public static int Compare(object A, object B)
		{
			Match matchA, matchB;
			int count, result;
			string partA, partB;
			int npartA, npartB;
			string stringA, stringB;

			if (A == null)
			{
				if (B == null) return 0;
				return 1;
			}
			else if (B == null)
			{
				return -1;
			}

			stringA = A.ToString(); stringB = B.ToString();

			if (stringB == stringA) return 0;

			matchA = regex.Match(stringA);
			matchB = regex.Match(stringB);
			if ((!matchA.Success) || (!matchB.Success)) return stringA.CompareTo(stringB);

			count = Math.Min(matchA.Groups[1].Captures.Count, matchB.Groups[1].Captures.Count);
			for (int t = 0; t < count; t++)
			{
				partA = matchA.Groups[1].Captures[t].Value;
				partB = matchB.Groups[1].Captures[t].Value;

				if (numericRegex.Match(partA).Success && numericRegex.Match(partB).Success)
				{
					npartA = Convert.ToInt32(partA);
					npartB = Convert.ToInt32(partB);

					result = npartA.CompareTo(npartB);
				}
				else
				{
					result = partA.CompareTo(partB);
				}
				if (result != 0) return result;


			}

			if (matchA.Groups[1].Captures.Count > matchB.Groups[1].Captures.Count) return 1; else return -1;

		}
	}

}
