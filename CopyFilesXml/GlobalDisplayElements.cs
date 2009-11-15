using System.Text;

namespace CopyFilesXml
{
	public class GlobalDisplayElements
	{

		public int BannerWidth
		{
			get
			{
				return 70;
			}
		}

		public string AppName
		{
			get
			{
				return "CopyFilesXml";
			}
		}

		public string BannerEqualChar
		{
			get
			{
				var sb = new StringBuilder();

				for(int i=0; i < BannerWidth; i++)
				{
					sb.Append("=");
				}

				return sb.ToString();
			}
		} 

		public string BannerDashChar
		{
			get
			{
				var sb = new StringBuilder();

				for(int i=0; i < BannerWidth; i++)
				{
					sb.Append("-");
				}

				return sb.ToString();
			}
			
		}

		public string BannerAsterikChar
		{
			get
			{
				var sb = new StringBuilder();

				for(int i=0; i < BannerWidth; i++)
				{
					sb.Append("*");
				}

				return sb.ToString();
			}
			
		}

		public string GetLeftSpacer(string textLine)
		{
			var sNum = BannerWidth - textLine.Length;

			if(sNum < 4)
			{
				return string.Empty;
			}

			var half = (sNum/2) - 1;

			var sb = new StringBuilder();

			for(int i= 0; i < half; i++ )
			{
				sb.Append(" ");
			}

			return sb.ToString();
		}
	}
}