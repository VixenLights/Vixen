using Vixen.Rule;

namespace VixenApplication.PatchingControls {
	interface IHasPatchRule {
		IPatchingRule Rule { get; }
	}
}
