using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;



namespace MathRegex
{
	class Program
	{
		static void Main(string[] args)
		{
			string str = "cost = ((price + 2E-4 + 1) * 0.98 * x + 2) * qq * ww + ee + rr";
			//string str = "cost = qq * ww + ee + rr * ((price + 2E-4 + 1) *0.98 * x + 2)";

			Regular R = new Regular(str);
			R.Calculations();

			Console.ReadKey();
			return;
		}
	}



}