namespace VixenModules.App.CustomPropEditor.Persistence.Legacy.LiteDb5021;

/// <summary>
/// Holds the raw records required to prove that a LiteDB v4 Custom Prop file can be read without opening a database engine.
/// </summary>
internal sealed record LegacyLiteDbRawDocument(byte[] PropDocument, byte[] Image, string ImageEntryName);
