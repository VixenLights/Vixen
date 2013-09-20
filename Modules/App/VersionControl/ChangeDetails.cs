using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl {
	public class ChangeDetails {
		public string Hash { get; set; }
		public DateTimeOffset ChangeDate { get; set; }
		public string FileName { get; set; }
		public string UserName { get; set; }
		public string Message { get; set; }
	}
}
