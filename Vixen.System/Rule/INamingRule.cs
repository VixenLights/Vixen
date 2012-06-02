namespace Vixen.Rule {
	public interface INamingRule {
		string Name { get; }
		string[] GenerateNames(int channelCount);
	}
}
