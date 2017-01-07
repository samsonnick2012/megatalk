using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Innostar.UI.ViewModels
{
	public class TagWordViewModel
	{
		public string Text
		{
			get;
			set;
		}

		public int Weight
		{
			get;
			set;
		}

		public int Rate
		{
			get;
			set;
		}

		public Color Color
		{
			get;
			set;
		}

		public double FontSize
		{
			get;
			set;
		}

		public string ColorName
		{
			get
			{
				return GetColorName(Color);
			}
		}

		public static double GetFontSize(double index, int allCount)
		{
			double result = (index + 1) / allCount * 5;

			if (result < 2)
			{
				result = 2;
			}

			return result;
		}

		private string GetColorName(Color color)
		{
			var result = string.Format("#{0}", color.Name);

			if (color.Name.Length == 5)
			{
				result = string.Format("#0{0}", color.Name);
			}

			return result;
		}

		public TagWordViewModel()
		{
			Rate = 1;
			Weight = 1;
		}

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
		public static IList<ColorInfo> GetGradients(Color start, Color end, int steps)
		{
			var result = new List<ColorInfo>
			{
				new ColorInfo
				{
					Position = 0,
					Color = start
				}
			};

			double stepA = (double)(end.A - start.A) / steps;
			double stepR = (double)(end.R - start.R) / steps;
			double stepG = (double)(end.G - start.G) / steps;
			double stepB = (double)(end.B - start.B) / steps;

			for (int i = 1; i < steps - 1; i++)
			{
				var a = (byte)(start.A + (int)(stepA * i));
				var r = (byte)(start.R + (int)(stepR * i - 3));
				var g = (byte)(start.G + (int)(stepG * i - 2));
				var b = (byte)(start.B + (int)(stepB * i - 1));

				result.Add(new ColorInfo
				{
					Position = i,
					Color = Color.FromArgb(a, r, g, b)
				});
			}

			result.Add(new ColorInfo
			{
				Position = result.Count + 1,
				Color = end
			});

			return result;
		}
	}
}