using System;
using System.Drawing;
using Vixen.Module.PostFilter;

namespace Vixen.Sys {
	class PostFilterState : IFilterState {
		public PostFilterState(IPostFilterModuleInstance filter) {
			Filter = filter;
		}

		public IPostFilterModuleInstance Filter { get; private set; }

		public float Affect(float value) {
			return Filter.Affect(value);
		}

		public Color Affect(Color value) {
			return Filter.Affect(value);
		}

		public DateTime Affect(DateTime value) {
			return Filter.Affect(value);
		}
	}
}
