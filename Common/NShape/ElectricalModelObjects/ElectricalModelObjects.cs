/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using Dataweb.NShape.Advanced;
using Dataweb.NShape.GeneralModelObjects;


namespace Dataweb.NShape.ElectricalModelObjects {

	public enum StateEnum { On, Off, Blocked, Defect, Unknown };


	public class Connector : GenericModelObject {

		public Connector(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 2;
		}


		public Connector(Connector source)
			: base(source) {
			this.terminalCount = 2;
		}


		/// <override></override>
		public override IModelObject Clone() {
			Connector result = new Connector(this);
			return result;
		}
	}


	public class Feeder : ValueDevice {

		internal Feeder(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 1;
		}


		public Feeder(Feeder source)
			: base(source) {
			this.terminalCount = 1;
		}


		/// <override></override>
		public override IModelObject Clone() {
			Feeder result = new Feeder(this);
			return result;
		}

	}


	public class Earth : GenericModelObject {

		public Earth(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 1;
		}

		public Earth(Earth source)
			: base(source) {
			this.terminalCount = 1;
		}

		/// <override></override>
		public override IModelObject Clone() {
			Earth result = new Earth(this);
			return result;
		}
	}


	public class Line : GenericModelObject {

		public Line(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 2;

		}


		public Line(Line source)
			: base(source) {
			this.terminalCount = 2;
		}


		/// <override></override>
		public override IModelObject Clone() {
			Line result = new Line(this);
			return result;
		}
	}


	public class Disconnector : GenericModelObject {

		public Disconnector(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			terminalCount = 2;
		}


		public Disconnector(Disconnector source)
			: base(source) {
			this.terminalCount = 2;
		}


		/// <override></override>
		public override IModelObject Clone() {
			Disconnector result = new Disconnector(this);
			return result;
		}
	}


	public class Transformer : ValueDevice {

		public Transformer(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 2;
		}


		public Transformer(Transformer source)
			: base(source) {
			this.terminalCount = 2;
		}


		/// <override></override>
		public override IModelObject Clone() {
			Transformer result = new Transformer(this);
			return result;
		}
	}


	public class Switch : GenericModelObject {

		public Switch(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 4;
		}


		public Switch(Switch source)
			: base(source) {
			this.terminalCount = 4;
		}


		/// <override></override>
		public override IModelObject Clone() {
			Switch result = new Switch(this);
			return result;
		}
	}


	public class AutoSwitch : GenericModelObject {

		public AutoSwitch(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 2;
		}


		public AutoSwitch(AutoSwitch source)
			: base(source) {
			this.terminalCount = 2;
		}


		/// <override></override>
		public override IModelObject Clone() {
			AutoSwitch result = new AutoSwitch(this);
			return result;
		}
	}


	public class AutoDisconnector : GenericModelObject {

		public AutoDisconnector(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			terminalCount = 2;
		}


		public AutoDisconnector(AutoDisconnector source)
			: base(source) {
			this.terminalCount = 2;
		}


		/// <override></override>
		public override IModelObject Clone() {
			AutoDisconnector result = new AutoDisconnector(this);
			return result;
		}
	}


	public class Measurement : ValueDevice {

		public Measurement(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 4;
		}


		public Measurement(Measurement source)
			: base(source) {
			this.terminalCount = 4;
		}


		/// <override></override>
		public override IModelObject Clone() {
			Measurement result = new Measurement(this);
			return result;
		}
	}


	public class Label : GenericModelObject {

		public Label(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 0;
		}


		public Label(Label source)
			: base(source) {
		}


		/// <override></override>
		public override IModelObject Clone() {
			Label result = new Label(this);
			return result;
		}
	}


	public class Picture : GenericModelObject {
		public Picture(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 0;
		}


		public Picture(Picture source)
			: base(source) {
		}


		/// <override></override>
		public override IModelObject Clone() {
			return new Picture(this);
		}
	}


	public class BusBar : GenericModelObject {
		public BusBar(ModelObjectType modelObjectType)
			: base(modelObjectType) {
			this.terminalCount = 1;
		}


		public BusBar(BusBar source)
			: base(source) {
		}


		/// <override></override>
		public override IModelObject Clone() {
			return new BusBar(this);
		}


		// Fields
		private const int terminalDistance = 50;
	}


	public static class NShapeLibraryInitializer {

		public static void Initialize(IRegistrar registrar) {
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);

			registrar.RegisterModelObjectType(new GenericModelObjectType("AutoDisconnector", namespaceName, categoryTitle,
				modelObjectType => new AutoDisconnector(modelObjectType),
				AutoDisconnector.GetPropertyDefinitions, 2));
			registrar.RegisterModelObjectType(new GenericModelObjectType("BusBar", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new BusBar(modelObjectType); },
				BusBar.GetPropertyDefinitions, 1));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Disconnector", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Disconnector(modelObjectType); },
				Disconnector.GetPropertyDefinitions, 2));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Earth", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Earth(modelObjectType); },
				Earth.GetPropertyDefinitions, 1));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Feeder", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Feeder(modelObjectType); },
				Feeder.GetPropertyDefinitions, 1));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Title", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Label(modelObjectType); },
				Label.GetPropertyDefinitions, 0));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Line", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Line(modelObjectType); },
				Line.GetPropertyDefinitions, 1));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Measurement", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Measurement(modelObjectType); },
				Measurement.GetPropertyDefinitions, 0));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Picture", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Picture(modelObjectType); },
				Picture.GetPropertyDefinitions, 0));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Switch", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Switch(modelObjectType); },
				Switch.GetPropertyDefinitions, 2));
			registrar.RegisterModelObjectType(new GenericModelObjectType("Transformer", namespaceName, categoryTitle,
				delegate(ModelObjectType modelObjectType) { return new Transformer(modelObjectType); },
				Transformer.GetPropertyDefinitions, 3));
		}

		private const string namespaceName = "ElectricalModelObjects";
		private const string categoryTitle = "Electrical";
		private const int preferredRepositoryVersion = 3;
	}
}