using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Module.SequenceType;
using Vixen.Sys;
using VixenModules.Sequence.Timed;
using VixenModules.SequenceType.Vixen2x;
using System;
using System.Collections.Generic;
using Vixen.Module;
using Vixen.Services;
using Vixen.Module.App;

namespace VixenModules.Sequence.Vixen2x
{
	public class Vixen2xSequenceTypeModule : SequenceTypeModuleInstanceBase
	{
		private Vixen2xSequenceStaticData _mappingData;

		public Dictionary<string, List<ChannelMapping>> Vixen2xMappings
		{
			get { return _mappingData.Vixen2xMappings; }
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _mappingData; }
			set { _mappingData = value as Vixen2xSequenceStaticData; }
		}

		public override ISequence CreateSequence()
		{
			return new TimedSequence();
		}

		public override ISequenceCache CreateSequenceCache()
		{
			throw new NotImplementedException();
		}

		public override IContentMigrator CreateMigrator()
		{
			return new Vixen2xSequenceMigrator();
		}

		public override ISequenceExecutor CreateExecutor()
		{
			return new Executor();
		}

		public override bool IsCustomSequenceLoader
		{
			get { return true; }
		}

		public override ISequence LoadSequenceFromFile(string Vixen2File)
		{
			try {
				using (Vixen2xSequenceImporterForm v2ImporterForm = new Vixen2xSequenceImporterForm(Vixen2File, StaticModuleData)) {
					if (v2ImporterForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
						return v2ImporterForm.Sequence;
					}
					else {
						//This will return a null sequence not sure we can do that.
						return null;
					}
				}
			}
			catch (Exception) {
				return null;
			}
		}
	}
}