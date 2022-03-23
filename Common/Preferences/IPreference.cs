using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Preferences
{
	public interface IPreference
	{
		void SetStandardValues();

		void Save(string path);

	}
}
