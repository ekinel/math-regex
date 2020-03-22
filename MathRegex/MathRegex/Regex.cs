using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using static System.Console;


namespace MathRegex
{
	class Regular
	{
		const string pattern = @"
			(
				(
					(	        (?<= ^ |\= |\+ |\* |(\s+)?\((\s+)? )																	(?<opt> (\s+)?\w+(\s+)?)													(?= \= |\* |\+ |(\s+)?\)(\s+)? |\! )																	)   |
					(	        (?<=  \= |\+ |\* |(\s+)?\((\s+)? )																		(?<opt> (\s+)?\d+(\.\d+((E|e)(\+|\-)\d+)?)?((E|e)(\+|\-)\d+)?(\s+)?)		(?= |\+ |\* |(\s+)?\)(\s+)? |\!)																		)   |
					(	        (?<= ((\s+)?\w+(\s+)?) )																				(?<opt> \=)																	(?= (\s+)?\d+(\.\d+((E|e)(\+|\-)\d+)?)?((E|e)(\+|\-)\d+)?(\s+)? |(\s+)?\w+(\s+)? |(\s+)?\((\s+)?)		)	|
					(	        (?<= (\s+)?\w+(\s+)? |(\s+)?\d+(\.\d+((E|e)(\+|\-)\d+)?)?((E|e)(\+|\-)\d+)?(\s+)? |(\s+)?\)(\s+)? )		(?<opt> \+ | \*)															(?= (\s+)?\d+(\.\d+((E|e)(\+|\-)\d+)?)?((E|e)(\+|\-)\d+)?(\s+)? |(\s+)?\w+(\s+)? |(\s+)?\((\s+)?)		)	|
					(	        (?<= (\s+)?\w+(\s+)? |(\s+)?\d+(\.\d+((E|e)(\+|\-)\d+)?)?((E|e)(\+|\-)\d+)?(\s+)? |(\s+)?\)(\s+)? )		(?<opt> \!)																	(?= $ )																									)	|
					(?<level>	(?<= \= |\* |\+ |(\s+)?\((\s+)?)																		(?<opt> (\s+)?\((\s+)?)														(?= (\s+)?\w+(\s+)? |(\s+)?\d+(\.\d+((E|e)(\+|\-)\d+)?)?((E|e)(\+|\-)\d+)?(\s+)? |(\s+)?\((\s+)?)		)	|
					(?<-level>	(?<=(\s+)?\)(\s+)? |(\s+)?\w+(\s+)? |(\s+)?\d+(\.\d+((E|e)(\+|\-)\d+)?)?((E|e)(\+|\-)\d+)?(\s+)?)		(?<opt> (\s+)?\)(\s+)?)														(?= (\s+)?\)(\s+)? |\+ |\* |\!)																			)
				)+
				(?(level)(?!))
			)";

		string str, polstr = "";

		Stack<char> signs = new Stack<char>();
		Stack<string> code = new Stack<string>();

		static Dictionary<char, int> prior = new Dictionary<char, int>()
		{
			{'=', 0},
			{')', 1},
			{'(', 2},
			{'+', 3},
			{'*', 4},
		};


		/**
		* Constructor
		*/
		public Regular(string text)
		{
			str = text;
			str += '!';
		}

		/**
		* The function works on each symbol and forms the final result
		*/
		public void Calculations()
		{
			Regex r = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);

			MatchCollection m = r.Matches(str);

			if (m.Count == 1 && m[0].Value == str)
			{
				foreach (Match match in m)
				{
					var mm = match;
					while (mm.Success)
					{
						for (int i = 0; i < match.Groups.Count; i++)
						{
							var g = match.Groups[i];
							if (i == 2)
							{
								for (int j = 0; j < g.Captures.Count; j++)
								{
									var ci = g.Captures[j];
									Actions(ci.ToString());
								}
							}
						}

						mm = mm.NextMatch();
					}
				}

			}
			else if (m.Count == 0) WriteLine("Error in position 1'{0}'", str[0]);

			else
			{
				int index = 0;

				for (int i = 0; i < m.Count; i++)
				{
					if (m[i].Index > index) break;
					index = m[i].Index + m[i].Length;
				}

				WriteLine("Error in position {0}'{1}'", index + 1, str[index]);
			}

			WriteLine("Result = \n{0}' \n\nNon-optimized code:", polstr);


			string s = code.Peek();
			WriteLine(code.Pop());

			List<(string, string)> actions_opt = new List<(string, string)>();
			string b = "", c = "";
			bool flag = true;

			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '\n' || s[i] == ' ' || i == s.Length - 1)
				{
					if (flag)
					{
						c = b;
						b = "";
						flag = false;
					}
					else
					{
						flag = true;

						if (i == s.Length - 1)
							b += s[i];

						actions_opt.Add((c, b));
						b = "";
					}
				}
				else b += s[i];
			}

			// Optimization
			flag = true;

			while (flag)
			{
				flag = false;

				for (int i = 1; i < actions_opt.Count; i++)
				{
					if (actions_opt[i - 1].Item1 == "LOAD" && actions_opt[i].Item1 == "ADD")
					{
						for (int j = i; j < actions_opt.Count; j++)
							if (actions_opt[j].Item2 == actions_opt[i].Item2)
								break;

						string po = actions_opt[i].Item2, po1 = actions_opt[i - 1].Item2;

						actions_opt.RemoveAt(i - 1);
						actions_opt.RemoveAt(i - 1);

						actions_opt.Insert(i - 1, ("ADD", po1));
						actions_opt.Insert(i - 1, ("LOAD", po));

					}
				}

				for (int i = 1; i < actions_opt.Count; i++)
				{
					if (actions_opt[i - 1].Item1 == "LOAD" && actions_opt[i].Item1 == "MPY")
					{
						for (int j = i; j < actions_opt.Count; j++)
							if (actions_opt[j].Item2 == actions_opt[i].Item2)
								break;

						string po = actions_opt[i].Item2, po1 = actions_opt[i - 1].Item2;

						actions_opt.RemoveAt(i - 1);
						actions_opt.RemoveAt(i - 1);

						actions_opt.Insert(i - 1, ("MPY", po1));
						actions_opt.Insert(i - 1, ("LOAD", po));

					}
				}

				for (int i = 1; i < actions_opt.Count; i++)
				{
					if (actions_opt[i - 1].Item1 == "STORE" && actions_opt[i].Item1 == "LOAD" && (actions_opt[i - 1].Item2 == actions_opt[i].Item2))
					{
						for (int j = i; j < actions_opt.Count; j++)
						{
							if (actions_opt[j].Item2 == actions_opt[i].Item2)
								if (actions_opt[j].Item1 == "STORE")
								{
									actions_opt.RemoveAt(i - 1);
									actions_opt.RemoveAt(i - 1);
								}
								else break;
						}

						actions_opt.RemoveAt(i - 1);
						actions_opt.RemoveAt(i - 1);

						flag = true;
					}
				}

				for (int i = 1; i < actions_opt.Count - 1; i++)
				{
					if (actions_opt[i - 1].Item1 == "LOAD" && actions_opt[i].Item1 == "STORE" && actions_opt[i + 1].Item1 == "LOAD")
					{
						for (int j = i + 1; j < actions_opt.Count - 1; j++)
							if (actions_opt[j].Item2 == actions_opt[i].Item2)
							{
								string p = actions_opt[j].Item1;
								actions_opt.RemoveAt(j);
								actions_opt.Insert(j, (p, actions_opt[i - 1].Item2));
								break;
							}

						actions_opt.RemoveAt(i - 1);
						actions_opt.RemoveAt(i - 1);

						flag = true;
					}
				}
			}

			WriteLine("\nOptimized code:");

			foreach ((string, string) i in actions_opt)
				WriteLine(i);
		}

		/**
		* The function generates optimized cod
		*/
		public void Actions(string s)
		{
			bool end;
			if (s == "!") end = true;
			else end = false;

			if ((s.Contains("+") || s.Contains("*") || s.Contains("=") || s.Contains("(") || s.Contains(")")) || end)
			{
				char symbol = ' ';
				if (!end || s.Contains(")")) symbol = Convert.ToChar(s.Trim());

				while (signs.Count > 0 && (end || (symbol != '(' && prior[signs.Peek()] >= prior[symbol])))
				{
					char sign = signs.Pop();
					if (sign == '(') break;

					polstr += sign;
					polstr += " ";

					string right = code.Pop();
					string left = code.Pop();

					int num = 1;
					while (right.Contains($"${num}") || left.Contains($"${num}")) num++;
					if (sign == '=') code.Push($"LOAD {right}\nSTORE {left}");
					else if (sign == '+') code.Push($"{right}\nSTORE ${num}\nLOAD {left}\nADD ${num}");
					else code.Push($"{right}\nSTORE ${num}\nLOAD {left}\nMPY ${num}");
				}

				if (symbol != ')' && !end) signs.Push(symbol);
			}
			else
			{
				var str = s;
				var new_s = str.Replace(" ", "");
				code.Push(new_s);
				polstr += new_s;
				polstr += " ";
			}
		}
	}
}
