using System;
using Vixen.Common.ValueTypes;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.ImageGrid
{
	public class Descriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{1B93A46D-4804-4D4C-81DE-ADF186584848}");
		internal Guid _gridPropertyId = new Guid("{DE3B4EC9-4E7F-4690-B0FE-3780B17A85EC}");
		private Guid[] _dependencies;
		private Guid[] _propertyDependencies;

		private ParameterSignature _parameterSignature = new ParameterSignature(
			new ParameterSpecification("Image File Path", typeof (FilePath))
			);

		public Descriptor()
		{
			_dependencies = new[]
			                	{
			                		_gridPropertyId
			                	};
			_propertyDependencies = new[]
			                        	{
			                        		_gridPropertyId
			                        	};
		}

		public override string TypeName
		{
			get { return "Image Grid"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Module); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (Data); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Displays an image on an RGB grid"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "ImageGrid"; }
		}

		public override Guid[] Dependencies
		{
			get { return _dependencies; }
		}

		public override ParameterSignature Parameters
		{
			get { return _parameterSignature; }
		}

		public override Guid[] PropertyDependencies
		{
			get { return _propertyDependencies; }
		}
	}
}