using System;
using System.Text;

namespace SourceGenerator.Extensions;

internal static class StringBuilderExtensions
{
	public static StringBuilder Indent(this StringBuilder stringBuilder, int count = 1)
	{
		if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

		return count switch
		{
			0 => stringBuilder,

			1 => stringBuilder.Append('\t'),

			_ => stringBuilder.Append('\t', count)
		};
	}
}
