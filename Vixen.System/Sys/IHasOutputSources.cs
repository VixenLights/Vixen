namespace Vixen.Sys {
	interface IHasOutputSources {
		void AddSource(IOutputStateSource source);
		void RemoveSource(IOutputStateSource source);
		void ClearSources();
	}
}
